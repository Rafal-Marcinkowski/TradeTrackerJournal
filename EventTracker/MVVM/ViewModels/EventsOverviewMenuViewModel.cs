using DataAccess.Data;
using EventTracker.MVVM.Views;
using Infrastructure.DataFilters;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsOverviewMenuViewModel : BindableBase
{
    private readonly IRegionManager regionManager;
    private readonly ICompanyData companyData;
    private readonly IEventData eventData;

    private ObservableCollection<Company> companies;

    private ObservableCollection<Company> filteredCompanies;
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
            SetProperty(ref searchBoxText, value, () => FilterCompanies());
        }
    }

    private void FilterCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText);
    }

    public EventsOverviewMenuViewModel(IRegionManager regionManager, ICompanyData companyData, IEventData eventData)
    {
        this.eventData = eventData;
        this.regionManager = regionManager;
        this.companyData = companyData;
        GetAllCompanies();
    }

    private async Task GetAllCompanies()
    {
        var companyList = await companyData.GetAllCompaniesAsync();
        companies = new ObservableCollection<Company>(companyList.OrderByDescending(q => q.EventCount));
        FilteredCompanies = new ObservableCollection<Company>(companies);
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters();
        parameters.Add("op", "op");
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(EventsOverviewView), parameters);
    });

    public ICommand EventsOverviewForCompanyCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany != null)
        {
            var parameters = new NavigationParameters
        {
            { "selectedCompany", selectedCompany.ID }
        };
            var region = regionManager.Regions["MainRegion"];
            region.RemoveAll();
            regionManager.RequestNavigate("MainRegion", nameof(EventsOverviewView), parameters);
        }
    });

    public ICommand LastXEventsOverviewCommand => new DelegateCommand<string>((nrOfLastTransactionsToShow) =>
    {
        if (int.TryParse(nrOfLastTransactionsToShow, out int transactionsToShow))
        {
            if (transactionsToShow > 0)
            {
                var parameters = new NavigationParameters()
                {
                    {"lastx", transactionsToShow },
                };
                var region = regionManager.Regions["MainRegion"];
                region.RemoveAll();
                regionManager.RequestNavigate("MainRegion", nameof(EventsOverviewView), parameters);
            }
        }
    });

    public ICommand TransactionsForCalendarDateCommand => new DelegateCommand<DateTime?>((calendarDate) =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        // regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });
}
