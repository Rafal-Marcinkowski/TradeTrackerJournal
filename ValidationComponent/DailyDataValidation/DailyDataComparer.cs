using SharedModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace ValidationComponent.DailyDataValidation;

public class DailyDataComparer : IEqualityComparer<DailyData>
{
    public bool Equals(DailyData? x, DailyData? y)
    {
        if (x == null || y == null) return false;

        return x.Date == y.Date
            && x.OpenPrice == y.OpenPrice
            && x.ClosePrice == y.ClosePrice
            && x.MinPrice == y.MinPrice
            && x.MaxPrice == y.MaxPrice;
    }

    public int GetHashCode([DisallowNull] DailyData obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        unchecked
        {
            int hash = 17;
            hash = hash * 23 + obj.Date.GetHashCode();
            hash = hash * 23 + obj.OpenPrice.GetHashCode();
            hash = hash * 23 + obj.ClosePrice.GetHashCode();
            hash = hash * 23 + obj.MinPrice.GetHashCode();
            hash = hash * 23 + obj.MaxPrice.GetHashCode();
            return hash;
        }
    }
}
