using DataAccess.Data;
using Infrastructure.GetDataFromHtml;
using SharedModels.Models;

namespace Infrastructure;

public class DailyTradeTracker
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;

    public DailyTradeTracker(ITransactionData transactionData, IDailyDataProvider dailyDataProvider)
    {
        this.transactionData = transactionData;
        this.dailyDataProvider = dailyDataProvider;
    }

    public async Task StartTracker()
    {
        var transactions = await transactionData.GetAllTransactionsAsync();
        if (transactions.Any())
        {
            transactions = transactions.Where(q => q.IsTracking);
            foreach (var transaction in transactions)
            {
                DailyData dailyData = new();
                dailyData = await GetDailyData.GetAsync(transaction);
                dailyData.TransactionID = transaction.ID;
                await dailyDataProvider.InsertDailyDataAsync(dailyData);
                await Task.Delay(5000);
            }
        }
    }
}
