using SharedProject.Models;
using System.Collections.ObjectModel;

namespace Infrastructure.DataFilters;

public class ObservableCollectionFilter
{
    public static ObservableCollection<Company> FilterCompaniesViaTextBoxText(ObservableCollection<Company> collection, string filterText)
    {
        return [.. collection.Where(item => item.CompanyName.Contains(filterText, StringComparison.OrdinalIgnoreCase))];
    }

    public static ObservableCollection<Company> OrderByDescendingTransactionCount(ObservableCollection<Company> collection)
    {
        return [.. collection.OrderByDescending(item => item.TransactionCount)];
    }

    public static ObservableCollection<Company> OrderByDescendingEventCount(ObservableCollection<Company> collection)
    {
        return [.. collection.OrderByDescending(item => item.EventCount)];
    }
}

