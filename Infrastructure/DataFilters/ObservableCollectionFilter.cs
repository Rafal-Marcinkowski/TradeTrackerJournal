using SharedModels.Models;
using System.Collections.ObjectModel;

namespace Infrastructure.DataFilters;

public class ObservableCollectionFilter
{
    public static ObservableCollection<Company> FilterCompaniesViaTextBoxText(ObservableCollection<Company> collection, string filterText)
    {
        var results = collection.Where(item => item.CompanyName.Contains(filterText, StringComparison.OrdinalIgnoreCase));
        return new ObservableCollection<Company>(results);
    }

    public static ObservableCollection<Company> OrderByDescendingTransactionCount(ObservableCollection<Company> collection)
    {
        var results = collection.OrderByDescending(item => item.TransactionCount);
        return new ObservableCollection<Company>(results);
    }
}

