using System.Collections.ObjectModel;
using TradeTracker.MVVM.Models;

namespace TradeTracker.DataFilters;

class ObservableCollectionFilter
{
    public static ObservableCollection<Company> FilterCompaniesViaTextBoxText(ObservableCollection<Company> collection, string filterText)
    {
        var results = collection.Where(item => item.CompanyName.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);
        return new ObservableCollection<Company>(results);
    }
}
