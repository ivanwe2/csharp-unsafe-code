// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

//unsafe
//{
//    const int size = 10;
//    Order* buyOrders = stackalloc Order[size];
//    Order* sellOrders = stackalloc Order[size];

//    for (int i = 0; i < size; i++)
//    {
//        buyOrders[i] = new Order
//        {
//            Id = i,
//            Price = 100 + i,
//            Quantity = 10 * (i + 1),
//            IsBuyOrder = true,
//        };
//        sellOrders[i] = new Order
//        {
//            Id = i + size,
//            Price = 100 + i,
//            Quantity = 5 * (i + 1),
//            IsBuyOrder = false,
//        };
//    }

//    for (int i = 0; i < size; i++)
//    {
//        Console.WriteLine(
//            $"Buy Order {buyOrders[i].Id}: Price={buyOrders[i].Price}, Quantity={buyOrders[i].Quantity}"
//        );
//        Console.WriteLine(
//            $"Sell Order {sellOrders[i].Id}: Price={sellOrders[i].Price}, Quantity={sellOrders[i].Quantity}"
//        );
//    }
//}

//static void SimulateIncomingOrders(OrderBook orderBook)
//{
//    int size = 100;

//}

//using TradingPlatform;

//int size = 10;

//OrderBook orderBook = new OrderBook(size);

//// Simulate incoming buy and sell orders
//SimulateIncomingOrders(orderBook, size);

//// Modify orders
//ModifyOrders(orderBook);

//// Create a user
//User buyer = new User(1, "John Doe"); // Example buyer with 2000 balance
//buyer.Balance = 10000;

//// Get the lowest sell price
//int lowestSellIndex;
//double lowestSellPrice = orderBook.GetLowestSellPrice(out lowestSellIndex);
//Console.WriteLine($"Current lowest sell price: {lowestSellPrice}");

//// Print the order book to verify orders
//orderBook.PrintOrders();

//// Attempt to buy at the lowest sell price
//bool purchaseSuccess = orderBook.BuyAtLowestSellPrice(50, 150, buyer); // Example max price: 150

//// Print the order book to verify orders
//orderBook.PrintOrders();

//static void SimulateIncomingOrders(OrderBook orderBook, int size)
//{
//    Order[] orders = new Order[size];

//    unsafe
//    {
//        fixed (Order* ordersPtr = orders)
//        {
//            for (int i = 0; i < size; i++)
//            {
//                if (i < size / 2)
//                {
//                    ordersPtr[i] = new Order { Id = i + 1, Price = 100 + i, Quantity = 10 + i, IsBuyOrder = true };
//                }
//                else
//                {
//                    ordersPtr[i] = new Order { Id = i + 1, Price = 105 + i, Quantity = 15 + i, IsBuyOrder = false };
//                }
//                orderBook.AddOrder(ordersPtr[i]);
//            }
//        }
//    }
//}

//static void ModifyOrders(OrderBook orderBook)
//{
//    // Modify an existing order to demonstrate the modify functionality
//    orderBook.ModifyOrder(1, 103.0, 120); // Modify Buy Order with Id 1
//    orderBook.ModifyOrder(6, 107.0, 50); // Modify Sell Order with Id 6
//}

//using TradingPlatform;

//TradeLogger tradeLogger = new TradeLogger();

//// Simulate trades
//Random rand = new Random();
//for (int i = 0; i < 10; i++)
//{
//    Trade trade = new Trade
//    {
//        TradeId = i + 1,
//        OrderId = (i % 50) + 1,
//        Price = 100 + (i % 20),
//        Quantity = 10 + (i % 5),
//        Timestamp = DateTime.Now.AddDays(-rand.Next(0, 30)) // Random timestamp from the last 30 days
//    };

//    tradeLogger.LogTrade(trade);
//}

//// Finalize logging to ensure all trades are processed
//tradeLogger.FinalizeLogging();

using TradingPlatform;

OrderBook orderBook = new OrderBook(10);

// Simulate incoming orders
SimulateIncomingOrders(orderBook, 10);

// Define cancellation requests
OrderCancellationRequest[] cancellations = new OrderCancellationRequest[]
{
            new OrderCancellationRequest { OrderId = 1 },
            new OrderCancellationRequest { OrderId = 5 },
            new OrderCancellationRequest { OrderId = 7 }
};

// Print orders to verify cancellations
orderBook.PrintOrders();

// Cancel orders
orderBook.BulkCancelOrders(cancellations);

// Print the order book to verify orders
orderBook.PrintOrders();

static void SimulateIncomingOrders(OrderBook orderBook, int size)
{
    Order[] orders = new Order[size];

    unsafe
    {
        fixed (Order* ordersPtr = orders)
        {
            for (int i = 0; i < size; i++)
            {
                if (i < size / 2)
                {
                    ordersPtr[i] = new Order { Id = i + 1, Price = 100 + i, Quantity = 10 + i, IsBuyOrder = true };
                }
                else
                {
                    ordersPtr[i] = new Order { Id = i + 1, Price = 105 + i, Quantity = 15 + i, IsBuyOrder = false };
                }
                orderBook.AddOrder(ordersPtr[i]);
            }
        }
    }
}

