﻿using SharedProject.Models;

namespace DataAccess.Data;
public interface IDailyDataProvider
{
    Task DeleteDailyDataAsync(int id);
    Task<IEnumerable<DailyData>> GetAllDailyDataAsync();
    Task<DailyData> GetDailyDataAsync(int id);
    Task<IEnumerable<DailyData>> GetDailyDataForEventAsync(int eventId);
    Task<IEnumerable<DailyData>> GetDailyDataForTransactionAsync(int transactionId);
    Task InsertDailyDataAsync(DailyData dailyData);
    Task UpdateDailyDataAsync(DailyData dailyData);
}