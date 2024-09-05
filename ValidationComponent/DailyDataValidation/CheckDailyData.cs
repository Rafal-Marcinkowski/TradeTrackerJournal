using DataAccess.Data;
using SharedProject.Models;

namespace ValidationComponent.DailyDataValidation;

public class CheckDailyData
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;

    public CheckDailyData(ITransactionData transactionData, IDailyDataProvider dailyDataProvider)
    {
        this.transactionData = transactionData;
        this.dailyDataProvider = dailyDataProvider;
    }

    public async Task<bool> IsExisting(DailyData dailyData)
    {
        var comparer = new DailyDataComparer();
        var data = await dailyDataProvider.GetDailyDataForTransactionAsync(dailyData.TransactionID ?? 0);
        return data.Any(q => comparer.Equals(q, dailyData));
    }
}
