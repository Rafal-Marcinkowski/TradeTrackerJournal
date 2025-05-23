﻿using SharedProject.Models;

namespace Infrastructure.Calculations;

public class ArchivedTurnoverMedian
{
    public static async Task<decimal> Calculate(IEnumerable<DataRecord> records)
    {
        if (!records.Any())
            return 0;

        var turnoverValues = records
            .Select(r => r.Turnover)
            .Order()
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
