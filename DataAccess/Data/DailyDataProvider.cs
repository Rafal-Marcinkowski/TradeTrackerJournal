using DataAccess.DBAccess;
using SharedModels.Models;

namespace DataAccess.Data;

public class DailyDataProvider : IDailyDataProvider
{
    private readonly ISQLDataAccess dBAccess;

    public DailyDataProvider(ISQLDataAccess dBAccess)
    {
        this.dBAccess = dBAccess;
    }

    public async Task<IEnumerable<DailyData>> GetAllDailyDataAsync()
    {
        return await dBAccess.LoadDataAsync<DailyData, dynamic>("GetAllDailyData", new { });
    }

    public async Task<DailyData> GetDailyDataAsync(int id)
    {
        var dailyDataList = await dBAccess.LoadDataAsync<DailyData, dynamic>("GetDailyData", new { ID = id });
        return dailyDataList.FirstOrDefault();
    }

    public async Task<IEnumerable<DailyData>> GetDailyDataForTransactionAsync(int transactionId)
    {
        return await dBAccess.LoadDataAsync<DailyData, dynamic>("GetDailyDataForTransaction", new { TransactionID = transactionId });
    }

    public async Task InsertDailyDataAsync(DailyData dailyData)
    {
        await dBAccess.SaveDataAsync("InsertDailyData", new
        {
            dailyData.TransactionID,
            dailyData.Date,
            dailyData.OpenPrice,
            dailyData.ClosePrice,
            dailyData.Volume,
            dailyData.MinPrice,
            dailyData.MaxPrice,
            dailyData.PriceChange,
            dailyData.VolumeChange
        });
    }

    public async Task UpdateDailyDataAsync(DailyData dailyData)
    {
        await dBAccess.SaveDataAsync("UpdateDailyData", new
        {
            dailyData.ID,
            dailyData.TransactionID,
            dailyData.Date,
            dailyData.OpenPrice,
            dailyData.ClosePrice,
            dailyData.Volume,
            dailyData.MinPrice,
            dailyData.MaxPrice,
            dailyData.PriceChange,
            dailyData.VolumeChange
        });
    }

    public async Task DeleteDailyDataAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteDailyData", new { ID = id });
    }
}
