using SharedProject.Models;
using System.Collections.ObjectModel;

namespace Infrastructure.DataFilters;

public static class ObservableCollectionFilter
{
    public static ObservableCollection<Company> FilterCompaniesViaTextBoxText(ObservableCollection<Company> collection, string filterText)
    {
        return [.. collection.Where(item => item.CompanyName.Contains(filterText, StringComparison.OrdinalIgnoreCase))];
    }

    public static ObservableCollection<T> OrderByDescending<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> keySelector)
    {
        return [.. collection.OrderByDescending(keySelector)];
    }
}
