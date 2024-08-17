using Infrastructure.DataFilters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewMenuViewModel : BindableBase
{
    private readonly IRegionManager regionManager;

    private ObservableCollection<Company> filteredCompanies;

    private ObservableCollection<Company> companies;

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

    public TransactionsOverviewMenuViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
        companies = Initialize.FillCompanies();
        FilteredCompanies = new ObservableCollection<Company>(companies);
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters();
        parameters.Add("OP", "OpenPositions");
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView), parameters);
    });

    public ICommand TransactionsOverviewForCompanyCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        //var parameters = new NavigationParameters();
        //// dla danej spolki
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });

    public ICommand LastXTransactionsOverviewCommand => new DelegateCommand<string>((nrOfLastTransactionsToShow) =>
    {
        if (int.TryParse(nrOfLastTransactionsToShow, out int transactionsToShow))
        {
            if (transactionsToShow > 0)
            {
                var parameters = new NavigationParameters();
                regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView), parameters);
            }
        }
    });

    public ICommand TransactionsForCalendarDateCommand => new DelegateCommand<DateTime?>((calendarDate) =>
    {
        //var parameters = new NavigationParameters();
        //// ostatnie X transakcji
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });
}
