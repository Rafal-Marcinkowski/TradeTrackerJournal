using SharedProject.Models;

namespace Infrastructure.GetDataFromHtml;

public class PrepareDailyData
{
    public async static Task<DailyData> GetAsync(DataRecord record)
    {
        DailyData dailyData = new()
        {
            Turnover = record.Turnover,
            MaxPrice = record.Max,
            MinPrice = record.Min,
            OpenPrice = record.Open,
            ClosePrice = record.Close,
            Date = record.Date,
            Volume = record.Volume
        };
        return dailyData;
    }
}
