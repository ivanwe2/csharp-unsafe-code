namespace TradingPlatform;
public class User(int id, string name, double balance = 0)
{
    public int UserId { get; set; } = id;
    public string UserName { get; set; } = name;
    public double Balance { get; set; } = balance;
    public List<double> BuyTargetPrices { get; private set; } = [];
    public List<double> SellTargetPrices { get; private set; } = [];

    public void SubscribeToBuyPrice(double target)
    {
        BuyTargetPrices.Add(target);
    }
    public void SubscribeToSellPrice(double target)
    {
        SellTargetPrices.Add(target);
    }

    public void OnPriceNotification(object sender, PriceNotificationEventArgs e)
    {
        if (e.IsBuyOrder)
        {
            for (int i = BuyTargetPrices.Count - 1; i >= 0; i--)
            {
                if (e.Price >= BuyTargetPrices[i] && e.Price != 0)
                {
                    Console.WriteLine($"Notification for user {UserId} ({UserName}): Current highest buy price has risen to {e.Price}.");
                    BuyTargetPrices.RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            for (int i = SellTargetPrices.Count - 1; i >= 0; i--)
            {
                if (e.Price >= SellTargetPrices[i] && e.Price != 0)
                {
                    Console.WriteLine($"Notification for user {UserId} ({UserName}): Current highest buy price has risen to {e.Price}.");
                    SellTargetPrices.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

public class PriceNotificationEventArgs(double price, bool isBuyOrder)
{
    public double Price { get; set; } = price;
    public bool IsBuyOrder { get; set; } = isBuyOrder;
}