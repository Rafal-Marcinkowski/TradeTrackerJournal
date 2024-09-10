using DataAccess.Data;
using Infrastructure.DataFilters;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsOverviewMenuViewModel : BindableBase
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

    public TransactionsOverviewMenuViewModel(IRegionManager regionManager, ICompanyData companyData, ITransactionData transactionData)
    {
        this.transactionData = transactionData;
        this.regionManager = regionManager;
        this.companyData = companyData;
        GetAllCompanies();
        FillTransactionCounts();
    }

    private async Task GetAllCompanies()
    {
        var companyList = await companyData.GetAllCompaniesAsync();
        companies = new ObservableCollection<Company>(companyList.OrderByDescending(q => q.TransactionCount));
        FilteredCompanies = new ObservableCollection<Company>(companies);
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
        var parameters = new NavigationParameters();
        parameters.Add("op", "op");
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
