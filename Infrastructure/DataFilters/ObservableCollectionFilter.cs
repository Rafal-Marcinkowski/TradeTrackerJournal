﻿using System.Collections.ObjectModel;

namespace Infrastructure.DataFilters;

public static class ObservableCollectionFilter
{
    public static ObservableCollection<T> OrderByDescending<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> keySelector)
    {
        return [.. collection.OrderByDescending(keySelector)];
    }
}
