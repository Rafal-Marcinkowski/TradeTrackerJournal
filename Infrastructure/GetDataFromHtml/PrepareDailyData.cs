using SharedModels.Models;

namespace Infrastructure.GetDataFromHtml;

public class PrepareDailyData
{
    public async static Task<DailyData> GetAsync(DataRecord record)
    {
        DailyData dailyData = new();
        dailyData.Turnover = record.Turnover;
        dailyData.MaxPrice = record.Max;
        dailyData.MinPrice = record.Min;
        dailyData.OpenPrice = record.Open;
        dailyData.ClosePrice = record.Close;
        dailyData.Date = record.Date;
        dailyData.Volume = record.Volume;
        return dailyData;
    }
}
