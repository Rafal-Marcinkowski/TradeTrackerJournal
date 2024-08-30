using DataAccess.Data;
using Infrastructure.DataFilters;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SessionOpening;

public class SessionOpeningViewModel : BindableBase
{
    private readonly ICompanyData companyData;
    private ObservableCollection<Company> companies;

    private string selectedCompanyName;
    public string SelectedCompanyName
    {
        get => selectedCompanyName;
        set => SetProperty(ref selectedCompanyName, value);
    }

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
            SetProperty(ref searchBoxText, value);
            FilterCompanies();
        }
    }
    private string newItemText = string.Empty;
    public string NewItemText
    {
        get => newItemText;
        set => SetProperty(ref newItemText, value);
    }

    private bool isNewItemBeingAdded = false;
    public bool IsNewItemBeingAdded
    {
        get => isNewItemBeingAdded;
        set
        {
            if (SetProperty(ref isNewItemBeingAdded, value))
                Vis = IsNewItemBeingAdded ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private Visibility visibility = Visibility.Collapsed;

    public SessionOpeningViewModel(ICompanyData companyData)
    {
        this.companyData = companyData;
        GetAllCompanies();
    }

    public Visibility Vis
    {
        get
        {
            if (IsNewItemBeingAdded)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        set => SetProperty(ref visibility, value);
    }

    public ObservableCollection<OpeningItem> OpeningItems { get; set; } = [];
    public ObservableCollection<OpeningCompany> OpeningCompanies { get; set; }

    private void FilterCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText);
    }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await companyData.GetAllCompaniesAsync();
            companies = new ObservableCollection<Company>(companyList.OrderByDescending(q => q.TransactionCount));
            FilteredCompanies = new ObservableCollection<Company>(companies);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading companies.");
        }
    }

    public ICommand AddNewItemCommand => new DelegateCommand(() =>
    {
        IsNewItemBeingAdded = !IsNewItemBeingAdded;
    });

    public ICommand ConfirmNewItemCommand => new DelegateCommand(() =>
    {
        if (!String.IsNullOrEmpty(NewItemText))
        {
            OpeningItem item = new()
            {
                Text = NewItemText,
            };

            OpeningItems.Add(item);
            NewItemText = string.Empty;
            IsNewItemBeingAdded = !IsNewItemBeingAdded;
        }
    });

    public ICommand DeleteItemCommand => new DelegateCommand<OpeningItem>((item) =>
    {
        if (item is not null)
        {
            OpeningItems.Remove(item);
        }
    });

    public ICommand LinkCommand => new DelegateCommand<OpeningItem>((item) =>
    {
        if (item is not null)
        {
            var url = item.Text;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }
    });

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((item) =>
    {
        OpeningCompanies ??= [];

        if (item is not null)
        {
            OpeningCompany openingCompany = new();
            openingCompany.CompanyName = item.CompanyName;
            OpeningCompanies.Add(openingCompany);
        }
    });
}
