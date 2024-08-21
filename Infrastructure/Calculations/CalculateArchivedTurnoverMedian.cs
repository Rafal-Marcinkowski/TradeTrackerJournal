using Infrastructure.GetDataFromHtml;
using SharedModels.Models;

namespace Infrastructure.Calculations;

public class CalculateArchivedTurnoverMedian
{
    public static decimal Calculate(List<DataRecord> records)
    {
        if (records.Count == 0)
            return 0;

        var turnoverValues = records
            .Select(r => r.Turnover)
            .ToList();

        return turnoverValues.Average();
    }

    public static async Task<decimal> GetTurnoverAsync(string companyCode, DateTime entryDate)
    {
        var records = await GetDataRecords.GetRecordsForAverageTurnoverCalculation(companyCode, entryDate);
        return Calculate(records.ToList());
    }
}
