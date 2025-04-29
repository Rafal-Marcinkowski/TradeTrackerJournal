using Infrastructure.DataFilters;
using Infrastructure.Interfaces;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsOverviewMenuViewModel : BaseListViewModel<Company>
{
    public TransactionsOverviewMenuViewModel(ITradeTrackerFacade facade)
    {
        this.facade = facade;
        _ = FillTransactionCounts();
        _ = GetAllCompanies();
    }

    private readonly ITradeTrackerFacade facade;

    private Dictionary<DateTime, int> transactionCounts;
    public Dictionary<DateTime, int> TransactionCounts
    {
        get => transactionCounts;
        set => SetProperty(ref transactionCounts, value);
    }
    protected override void OnCollectionFiltered()
    {
        ItemsSource = ObservableCollectionFilter.OrderByDescendingTransactionCount(ItemsSource);
    }

    private async Task GetAllCompanies()
    {
        var companyList = await facade.CompanyManager.GetAllCompanies();
        ItemsSource = [.. companyList.OrderByDescending(q => q.TransactionCount)];
    }

    private async Task FillTransactionCounts()
    {
        var transactions = await facade.TransactionManager.GetAllTransactions();
        var transactionsByDay = from trans in transactions
                                group trans by trans.EntryDate into transGroups
                                select new
                                {
                                    Day = transGroups.Key,
                                    TransactionCount = transGroups.Count()
                                };
        TransactionCounts = transactionsByDay.ToDictionary(t => t.Day, t => t.TransactionCount);
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters
        {
            { "op", "op" }
        };
        facade.ViewManager.NavigateTo(nameof(TransactionsOverviewView), parameters);
    });

    public ICommand TransactionsOverviewForCompanyCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany != null)
        {
            var parameters = new NavigationParameters
        {
            {"selectedCompany", selectedCompany.ID }
        };
            facade.ViewManager.NavigateTo(nameof(TransactionsOverviewView), parameters);
        }
    });

    public ICommand LastXTransactionsOverviewCommand => new DelegateCommand<string>((nrOfLastTransactionsToShow) =>
    {
        if (int.TryParse(nrOfLastTransactionsToShow, out int transactionsToShow))
        {
            if (transactionsToShow > 0)
            {
                var parameters = new NavigationParameters()
                {
                    {"lastx", transactionsToShow },
                };
                facade.ViewManager.NavigateTo(nameof(TransactionsOverviewView), parameters);
            }
        }
    });

    public ICommand TransactionsForCalendarDateCommand => new DelegateCommand<DateTime?>((calendarDate) =>
    {
        facade.ViewManager.NavigateTo(nameof(TransactionsOverviewView), new NavigationParameters
        {
            { "calendarDate", calendarDate }
        });
    });
}
