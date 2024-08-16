using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TradeTracker.DataFilters;
using TradeTracker.MVVM.Models;

namespace TradeTracker.MVVM.ViewModels;

public class AddTransactionViewModel : BindableBase
{
    private readonly IRegionManager regionManager;

    private ObservableCollection<Company> filteredCompanies;

    private ObservableCollection<Company> companies;

    private string selectedCompanyName;
    public string SelectedCompanyName
    {
        get => selectedCompanyName;
        set => SetProperty(ref selectedCompanyName, value);
    }

    public ObservableCollection<Company> FilteredCompanies
    {
        get => filteredCompanies;
        set
        {
            SetProperty(ref filteredCompanies, value);
        }
    }

    private string searchBoxText;
    public string SearchBoxText
    {
        get => searchBoxText;
        set
        {
            SetProperty(ref searchBoxText, value);
            FilterCompanies();
        }
    }

    private void FilterCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText);
    }

    public AddTransactionViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
        companies = Initialize.FillCompanies();
        FilteredCompanies = new ObservableCollection<Company>(companies);
    }

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        SelectedCompanyName = selectedCompany.CompanyName;
    });

    public ICommand AddTransactionCommand => new DelegateCommand(() =>
    {

    });
}
