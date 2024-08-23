using SharedModels.Models;

namespace Infrastructure.Calculations;

public class ArchivedTurnoverMedian
{
    public static async Task<decimal> Calculate(IEnumerable<DataRecord> records)
    {
        if (records.Count() == 0)
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
}
