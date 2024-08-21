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
}
