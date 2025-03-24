using DataAccess.Data;
using EventTracker.MVVM.Views;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsOverviewMenuViewModel : BaseListViewModel<Company>
{
    private readonly IRegionManager regionManager;
    private readonly ICompanyData companyData;

    public EventsOverviewMenuViewModel(IRegionManager regionManager, ICompanyData companyData)
    {
        this.regionManager = regionManager;
        this.companyData = companyData;
        _ = GetAllCompanies();
    }

    private async Task GetAllCompanies()
    {
        var companyList = await companyData.GetAllCompaniesAsync();
        ItemsSource = [.. companyList.OrderByDescending(q => q.EventCount)];
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters
        {
            { "op", "op" }
        };

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
