using DataAccess.Data;
using SharedProject.Models;

namespace ValidationComponent.DailyDataValidation;

public class CheckDailyData(IDailyDataProvider dailyDataProvider)
{
    public async Task<bool> IsExisting(DailyData dailyData)
    {
        var comparer = new DailyDataComparer();
        var data = await dailyDataProvider.GetDailyDataForTransactionAsync(dailyData.TransactionID ?? 0);
        return data.Any(q => comparer.Equals(q, dailyData));
    }
}
