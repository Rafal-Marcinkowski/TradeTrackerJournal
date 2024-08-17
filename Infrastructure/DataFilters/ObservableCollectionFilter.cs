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
}

