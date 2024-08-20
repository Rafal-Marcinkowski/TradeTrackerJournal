using DataAccess.Data;
using Infrastructure.Events;
using Infrastructure.GetDataFromHtml;
using Prism.Events;
using Serilog;
using SharedModels.Models;
using ValidationComponent.DailyDataValidation;

namespace Infrastructure;

public class DailyTradeTracker
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private List<Transaction> failedTransactions;

    public DailyTradeTracker(ITransactionData transactionData, IDailyDataProvider dailyDataProvider, IEventAggregator eventAggregator)
    {
        this.transactionData = transactionData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;
    }

    public async Task StartTracker(bool IsRetrying = false)
    {
        if (!IsRetrying)
        {
            failedTransactions = [];
        }
        var transactions = await transactionData.GetAllTransactionsAsync();
        if (IsRetrying)
        {
            transactions = failedTransactions;
        }
        if (transactions.Any())
        {
            transactions = transactions.Where(q => q.IsTracking);
            foreach (var transaction in transactions)
            {
                try
                {
                    DailyData dailyData = new();
                    dailyData = await GetDailyData.GetAsync(transaction);
                    dailyData.TransactionID = transaction.ID;
                    var checkDailyData = new CheckDailyData(transactionData, dailyDataProvider);
                    if (!await checkDailyData.IsExisting(dailyData))
                    {
                        await dailyDataProvider.InsertDailyDataAsync(dailyData);
                        eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(dailyData);
                        await Task.Delay(5000);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Błąd przy pobieraniu dziennych danych z Biznesradaru");
                    failedTransactions.Add(transaction);
                }
            }
            if (failedTransactions.Count != 0)
            {
                await Task.Delay(10000);
                await Task.Run(async () =>
                {
                    Log.Information($"Ponawiam próbę pobrania danych z biznesradaru, ilość transakcji z błędami: {failedTransactions.Count}");
                    await StartTracker(true);
                });
            }
        }
    }
}

///sprobuj pobrac pierw z biznesradaru pozniej ze stooq, kilka prob itd itp.