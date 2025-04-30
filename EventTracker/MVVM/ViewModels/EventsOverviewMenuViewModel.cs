using DataAccess.Data;
using EventTracker.MVVM.Views;
using Infrastructure.DataFilters;
using Infrastructure.Services;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsOverviewMenuViewModel : BaseListViewModel<Company>
{
    private readonly ViewManager viewManager;
    private readonly ICompanyData companyData;

    public EventsOverviewMenuViewModel(ViewManager viewManager, ICompanyData companyData)
    {
        this.viewManager = viewManager;
        this.companyData = companyData;
        _ = GetAllCompanies();
    }

    private async Task GetAllCompanies()
    {
        var companyList = await companyData.GetAllCompaniesAsync();
        ItemsSource = [.. companyList.OrderByDescending(q => q.EventCount)];
    }

    protected override void OnCollectionFiltered()
    {
        ItemsSource = ObservableCollectionFilter.OrderByDescending(ItemsSource, c => c.EventCount);
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters
        {
            { "op", "op" }
        };

        viewManager.NavigateTo(nameof(EventsOverviewView), parameters);
    });

    public ICommand EventsOverviewForCompanyCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany != null)
        {
            var parameters = new NavigationParameters
        {
            { "selectedCompany", selectedCompany.ID }
        };
            viewManager.NavigateTo(nameof(EventsOverviewView), parameters);
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
                viewManager.NavigateTo(nameof(EventsOverviewView), parameters);
            }
        }
    });

    public ICommand EventsForCalendarDateCommand => new DelegateCommand<DateTime?>((calendarDate) =>
    {
        if (calendarDate == null) return;

        var parameters = new NavigationParameters
        {
            { "calendarDate", calendarDate }
        };

        viewManager.NavigateTo(nameof(EventsOverviewView), parameters);
    });
}
