using System.Runtime.InteropServices;

namespace TradingPlatform;
public struct Trade
{
    public int TradeId;
    public int OrderId;
    public double Price;
    public int Quantity;
    public DateTime Timestamp;
}

public unsafe class TradeLogger
{
    private const int BufferSize = 100;
    private Trade* tradeBuffer;
    private int tradeCount;

    public TradeLogger()
    {
        unsafe
        {
            tradeBuffer = (Trade*)NativeMemory.Alloc((nuint)(BufferSize * sizeof(Trade)));
            tradeCount = 0;
        }
    }

    ~TradeLogger()
    {
        NativeMemory.Free(tradeBuffer);
    }

    public unsafe void LogTrade(Trade trade)
    {
        if (tradeCount < BufferSize)
        {
            tradeBuffer[tradeCount] = trade;
            tradeCount++;
        }
        else
        {
            // If buffer is full, process and clear it
            ProcessAndClearBuffer();
            tradeBuffer[0] = trade;
            tradeCount = 1;
        }
    }

    private unsafe void ProcessAndClearBuffer()
    {
        // Process the buffer (e.g., aggregate data, generate report)
        // For simplicity, we just write to a file here
        using (StreamWriter writer = new StreamWriter("TradeReport.txt"))
        {
            for (int i = 0; i < tradeCount; i++)
            {
                Trade trade = tradeBuffer[i];
                writer.WriteLine($"TradeId: {trade.TradeId}, OrderId: {trade.OrderId}, Price: {trade.Price}, Quantity: {trade.Quantity}, Timestamp: {trade.Timestamp}");
            }
        }

        // Clear the buffer
        tradeCount = 0;
    }

    public unsafe void FinalizeLogging()
    {
        if (tradeCount > 0)
        {
            ProcessAndClearBuffer();
        }
    }
}
