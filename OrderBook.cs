using System.Drawing;
using System.Runtime.InteropServices;
using TradingPlatform;

public unsafe class OrderBook
{
    private readonly int _size;
    private Order* buyOrders;
    private Order* sellOrders;
    private double highestBuyPrice;
    private double lowestSellPrice;


    public delegate void PriceNotificationEventHandler(object sender, PriceNotificationEventArgs args);
    
    public event PriceNotificationEventHandler PriceNotification;
    public OrderBook(int size = 10)
    {
        _size = size;
        buyOrders = (Order*)NativeMemory.Alloc((nuint)(_size * sizeof(Order)));
        sellOrders = (Order*)NativeMemory.Alloc((nuint)(_size * sizeof(Order)));

        for (int i = 0; i < _size; i++)
        {
            buyOrders[i] = new Order() { Id = 0 };
            sellOrders[i] = new Order() { Id = 0 };
        }

        highestBuyPrice = double.MinValue;
        lowestSellPrice = double.MaxValue;
    }

    ~OrderBook()
    {
        NativeMemory.Free(buyOrders);
        NativeMemory.Free(sellOrders);
    }

    public unsafe void AddOrder(Order newOrder)
    {
        Order* orders = newOrder.IsBuyOrder ? buyOrders : sellOrders;

        for (int i = 0; i < _size; i++)
        {
            if (orders[i].Id == 0)
            {
                orders[i] = newOrder;
                UpdateAndNotify();
                break;
            }
        }
    }

    public unsafe void RemoveOrder(int orderId)
    {
        Order* orders = null;
        int orderIndex = -1;

        for (int i = 0; i < _size; i++)
        {
            if (buyOrders[i].Id == orderId)
            {
                orders = buyOrders;
                orderIndex = i;
                break;
            }
            else if (sellOrders[i].Id == orderId)
            {
                orders = sellOrders;
                orderIndex = i;
                break;
            }
        }

        if (orderIndex is -1)
        {
            return;
        }

        orders[orderIndex] = new Order() { Id = 0 };
    }

    public unsafe void PrintOrders()
    {
        for (int i = 0; i < _size; i++)
        {
            Console.WriteLine(
                $"Buy Order {buyOrders[i].Id}: Price={buyOrders[i].Price}, Quantity={buyOrders[i].Quantity}"
            );
        }

        for (int i = 0; i < _size; i++)
        {
            Console.WriteLine(
                $"Sell Order {sellOrders[i].Id}: Price={sellOrders[i].Price}, Quantity={sellOrders[i].Quantity}"
            );
        }
    }

    public unsafe void ModifyOrder(int orderId, double newPrice, int newQuantity)
    {
        for (int i = 0; i < _size; i++)
        {
            if (buyOrders[i].Id == orderId)
            {
                buyOrders[i].Price = newPrice;
                buyOrders[i].Quantity = newQuantity;
                UpdateAndNotify();
                break;
            }
            else if (sellOrders[i].Id == orderId)
            {
                sellOrders[i].Price = newPrice;
                sellOrders[i].Quantity = newQuantity;
                UpdateAndNotify();
                break;
            }
        }
    }

    public unsafe double GetLowestSellPrice(out int lowestSellIndex)
    {
        lowestSellIndex = -1;
        double lowestSellPrice = double.MaxValue;

        for (int i = 0; i < _size; i++)
        {
            if (sellOrders[i].Id != 0 && sellOrders[i].Price < lowestSellPrice)
            {
                lowestSellPrice = sellOrders[i].Price;
                lowestSellIndex = i;
            }
        }

        return lowestSellPrice == double.MaxValue ? -1 : lowestSellPrice;
    }

    public unsafe bool BuyAtLowestSellPrice(int buyQuantity, double maxPrice, User buyer)
    {
        double lowestSellPrice = GetLowestSellPrice(out int lowestSellIndex);

        if (lowestSellIndex == -1 || lowestSellPrice == double.MaxValue)
        {
            Console.WriteLine("No suitable sell orders available");
            return false;
        }

        double totalCost = lowestSellPrice * buyQuantity;
        if (buyer.Balance < totalCost)
        {
            Console.WriteLine("Insufficient balance");
            return false;
        }

        byte* sellPtrByte = (byte*)sellOrders;
        Order* matchedSellOrder = (Order*)sellPtrByte + (lowestSellIndex * sizeof(Order));

        int matchedQuantity = Math.Min(buyQuantity, matchedSellOrder->Quantity);
        matchedSellOrder->Quantity -= matchedQuantity;
        buyer.Balance -= totalCost;

        if (matchedSellOrder->Quantity == 0)
        {
            RemoveOrder(matchedSellOrder->Id);
        }

        Console.WriteLine("Order fulfilled");
        return false;
    }

    public unsafe void BulkCancelOrders(OrderCancellationRequest[] requests)
    {
        Span<int> buffer = stackalloc int[requests.Length];

        for (int i = 0; i < requests.Length; i++)
        {
            buffer[i] = requests[i].OrderId;
        }

        RemoveOrders(buffer);
    }


    private void RemoveOrders(Span<int> orderIds)
    {
        fixed (int* orderIdsPtr = orderIds)
        {
            for (int i = 0; i < orderIds.Length; i++)
            {
                int orderId = orderIdsPtr[i];
                int orderIndex = -1;
                Order* orders = null;

                for (int j = 0; j < _size; j++)
                {
                    if (buyOrders[j].Id == orderId)
                    {
                        orders = buyOrders;
                        orderIndex = j;
                        break;
                    }
                    else if (sellOrders[j].Id == orderId)
                    {
                        orders = sellOrders;
                        orderIndex = j;
                        break;
                    }
                }

                if (orderIndex != -1)
                {
                    orders[orderIndex] = new Order { Id = 0 };
                }
            }
        }
    }

    protected virtual void OnPriceNotification(PriceNotificationEventArgs args)
    {
        PriceNotification?.Invoke(this, args);
    }

    private unsafe void UpdateAndNotify()
    {
        fixed (double* fixedHighestBuyPrice = &highestBuyPrice, fixedLowestSellPrice = &lowestSellPrice)
        {
            *fixedHighestBuyPrice = double.MinValue;
            *fixedLowestSellPrice = double.MaxValue;

            for (int i = 0; i < _size; i++)
            {
                if (buyOrders[i].Price > *fixedHighestBuyPrice)
                {
                    *fixedHighestBuyPrice = buyOrders[i].Price;
                    OnPriceNotification(new PriceNotificationEventArgs(*fixedHighestBuyPrice, isBuyOrder: true));
                }

                if (sellOrders[i].Price < *fixedLowestSellPrice)
                {
                    *fixedLowestSellPrice = sellOrders[i].Price;
                    OnPriceNotification(new PriceNotificationEventArgs(*fixedLowestSellPrice, isBuyOrder: false));
                }
            }
        }
    }
}
