using SharedProject.Models;
using System.Diagnostics.CodeAnalysis;

namespace ValidationComponent.Events;

internal class EventComparer : IEqualityComparer<Event>
{
    public bool Equals(Event? x, Event? y)
    {
        if (x == null || y == null) return false;
        return x.EntryDate == y.EntryDate
            && x.CompanyID == y.CompanyID
            && x.EntryPrice == y.EntryPrice;
    }

    public int GetHashCode([DisallowNull] Event obj)
    {
        return HashCode.Combine(obj.EntryDate, obj.CompanyID);
    }
}
