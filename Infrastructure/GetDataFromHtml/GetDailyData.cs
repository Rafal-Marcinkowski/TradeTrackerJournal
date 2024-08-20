using Infrastructure.Calculations;
using SharedModels.Models;

namespace Infrastructure.GetDataFromHtml;

public class GetDailyData
{
    public async static Task<DailyData> GetAsync(Transaction transaction)
    {
        var data = await GetRelevantNodes.PrepareData(transaction.CompanyName);
        DailyData dailyData = new();
        dailyData.Date = (DateTime)data["Date"];
        dailyData.OpenPrice = (decimal)data["OpenPrice"];
        dailyData.ClosePrice = (decimal)data["Price"];
        dailyData.MinPrice = (decimal)data["MinPrice"];
        dailyData.MaxPrice = (decimal)data["MaxPrice"];
        dailyData.Volume = (decimal)data["Volume"];
        dailyData.Turnover = (decimal)data["Turnover"];
        dailyData.TransactionCount = (int)data["Transactions"];
        dailyData.TurnoverChange = await CalculateDailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianVolume, dailyData.Turnover);
        dailyData.PriceChange = await CalculateDailyDataProperties.CalculateTurnoverChange(transaction.EntryPrice, dailyData.ClosePrice);
        return dailyData;
    }
}
