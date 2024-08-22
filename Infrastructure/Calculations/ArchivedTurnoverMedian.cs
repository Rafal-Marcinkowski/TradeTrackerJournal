using Infrastructure.GetDataFromHtml;
using SharedModels.Models;

namespace Infrastructure.Calculations;

public class ArchivedTurnoverMedian
{
    public static decimal Calculate(List<DataRecord> records)
    {
        if (records.Count == 0)
            return 0;

        var turnoverValues = records
            .Select(r => r.Turnover)
            .OrderBy(t => t)
            .ToList();

        int count = turnoverValues.Count;
        if (count % 2 == 1)
        {
            return turnoverValues[count / 2];
        }
        else
        {
            decimal middle1 = turnoverValues[(count / 2) - 1];
            decimal middle2 = turnoverValues[count / 2];
            return (middle1 + middle2) / 2;
        }
    }

    public static async Task<decimal> GetTurnoverAsync(string companyCode, DateTime entryDate)
    {
        var records = await GetDataRecords.GetRecordsForMedianTurnoverCalculation(companyCode, entryDate);
        return Calculate(records.ToList());
    }
}
