using SharedModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace ValidationComponent.Transactions;

internal class TransactionComparer : IEqualityComparer<Transaction>
{
    public bool Equals(Transaction? x, Transaction? y)
    {
        if (x == null || y == null) return false;
        return x.EntryDate == y.EntryDate
            && x.EntryPrice == y.EntryPrice;
    }

    public int GetHashCode([DisallowNull] Transaction obj)
    {
        return HashCode.Combine(obj.EntryDate, obj.EntryPrice);
    }
}
