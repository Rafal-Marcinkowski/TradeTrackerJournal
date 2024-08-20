namespace Infrastructure.Calculations;

public class CalculateDailyDataProperties
{
    public static async Task<decimal> CalculateTurnoverChange(decimal entryTurnover, decimal dailyTurnover)
    {
        return ((dailyTurnover - entryTurnover) / entryTurnover) * 100;
    }

    public static async Task<decimal> CalculatePriceChange(decimal entryPrice, decimal dailyPrice)
    {
        return ((dailyPrice - entryPrice) / entryPrice) * 100;
    }
}
