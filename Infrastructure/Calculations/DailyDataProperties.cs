namespace Infrastructure.Calculations;

public class DailyDataProperties
{
    public static async Task<decimal> CalculateTurnoverChange(decimal entryTurnover, decimal dailyTurnover)
    {
        return Math.Round(dailyTurnover / entryTurnover, 2);
    }

    public static async Task<decimal> CalculatePriceChange(decimal entryPrice, decimal dailyPrice)
    {
        return Math.Round(((dailyPrice - entryPrice) / entryPrice) * 100, 2);
    }
}
