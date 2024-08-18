using DataAccess.Data;
using Infrastructure.DataFilters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class AddTransactionViewModel : BindableBase
{
    private readonly IRegionManager regionManager;
    private readonly ITransactionData transactionData;
    private readonly ICompanyData companyData;

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

    private string entryDate;
    public string EntryDate
    {
        get => entryDate;
        set => SetProperty(ref entryDate, value);
    }

    private string entryPrice;
    public string EntryPrice
    {
        get => entryPrice;
        set => SetProperty(ref entryPrice, value);
    }

    private string positionSize;
    public string PositionSize
    {
        get => positionSize;
        set => SetProperty(ref positionSize, value);
    }

    private string numberOfShares;
    public string NumberOfShares
    {
        get => numberOfShares;
        set => SetProperty(ref numberOfShares, value);
    }

    private string avgSellPrice;
    public string AvgSellPrice
    {
        get => avgSellPrice;
        set => SetProperty(ref avgSellPrice, value);
    }

    private string initialDescription;
    public string InitialDescription
    {
        get => initialDescription;
        set => SetProperty(ref initialDescription, value);
    }

    private string informationLink;
    public string InformationLink
    {
        get => informationLink;
        set => SetProperty(ref informationLink, value);
    }


    private void FilterCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText);
    }

    public AddTransactionViewModel(IRegionManager regionManager, ITransactionData transactionData, ICompanyData companyData)
    {
        this.regionManager = regionManager;
        this.transactionData = transactionData;
        this.companyData = companyData;
        GetAllCompanies();
    }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await companyData.GetAllCompaniesAsync();
            companies = new ObservableCollection<Company>(companyList);
            FilteredCompanies = new ObservableCollection<Company>(companies);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading companies: {ex.Message}");
            Log.Error(ex, "Error loading companies.");
        }
    }

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        SelectedCompanyName = selectedCompany.CompanyName;
    });

    public ICommand AddTransactionCommand => new DelegateCommand(() =>
    {
        Transaction transaction = new();
        DateTime currentDate = DateTime.Now.Date;

        if (TimeSpan.TryParse(EntryDate, out TimeSpan timeComponent))
        {
            DateTime combinedDateTime = currentDate + timeComponent;
            transaction.EntryDate = combinedDateTime;
            if (double.TryParse(EntryPrice, out double value))
            {
                transaction.EntryPrice = value;
            }
            transaction.PositionSize = int.Parse(positionSize);
            transaction.InitialDescription = InitialDescription;
            transaction.InformationLink = InformationLink;
            transaction.CompanyID = 1;
        }
        try
        {

            transactionData.InsertTransactionAsync(transaction);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    });

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {

    });
}
