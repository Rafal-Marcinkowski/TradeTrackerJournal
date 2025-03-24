using DataAccess.Data;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsOverviewMenuViewModel : BaseListViewModel<Company>
{
    private readonly IRegionManager regionManager;
    private readonly ICompanyData companyData;
    private readonly ITransactionData transactionData;

    private Dictionary<DateTime, int> transactionCounts;
    public Dictionary<DateTime, int> TransactionCounts
    {
        get => transactionCounts;
        set => SetProperty(ref transactionCounts, value);
    }

    public TransactionsOverviewMenuViewModel(IRegionManager regionManager, ICompanyData companyData, ITransactionData transactionData)
    {
        this.transactionData = transactionData;
        this.regionManager = regionManager;
        this.companyData = companyData;
        _ = GetAllCompanies();
        _ = FillTransactionCounts();
    }

    private async Task GetAllCompanies()
    {
        var companyList = await companyData.GetAllCompaniesAsync();
        ItemsSource = [.. companyList.OrderByDescending(q => q.TransactionCount)];
    }

    private async Task FillTransactionCounts()
    {
        var transactions = await transactionData.GetAllTransactionsAsync();
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

        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView), parameters);
    });

    public ICommand TransactionsOverviewForCompanyCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany != null)
        {
            var parameters = new NavigationParameters
        {
            {"selectedCompany", selectedCompany.ID }
        };
            var region = regionManager.Regions["MainRegion"];
            region.RemoveAll();
            regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView), parameters);
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
                var region = regionManager.Regions["MainRegion"];
                region.RemoveAll();
                regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView), parameters);
            }
        }
    });

    public ICommand TransactionsForCalendarDateCommand => new DelegateCommand<DateTime?>((calendarDate) =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });
}
