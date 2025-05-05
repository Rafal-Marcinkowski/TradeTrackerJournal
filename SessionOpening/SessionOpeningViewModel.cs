using DataAccess.Data;
using Serilog;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SessionOpening;

public class SessionOpeningViewModel : BaseListViewModel<Company>
{
    private readonly ICompanyData companyData;

    private string selectedCompanyName;
    public string SelectedCompanyName
    {
        get => selectedCompanyName;
        set => SetProperty(ref selectedCompanyName, value);
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
        OpeningCompanies = [];
        OpeningItems = [];
        this.companyData = companyData;
        _ = GetAllCompanies();
    }

    protected override void OnCollectionFiltered()
    {
        ItemsSource = [.. ItemsSource.OrderBy(q => q.CompanyName)];
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

    public ObservableCollection<OpeningItem> OpeningItems { get; set; }
    public ObservableCollection<OpeningCompany> OpeningCompanies { get; set; }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await companyData.GetAllCompaniesAsync();
            ItemsSource = [.. companyList.OrderByDescending(q => q.TransactionCount)];
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading companies.");
        }
    }

    public ICommand AddNewItemCommand => new DelegateCommand(() => IsNewItemBeingAdded = !IsNewItemBeingAdded);

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

    public ICommand DeleteOpeningCompanyCommand => new DelegateCommand<OpeningCompany>((item) =>
    {
        if (item is not null)
        {
            OpeningCompanies.Remove(item);
        }
    });

    public ICommand LinkCommand => new DelegateCommand<OpeningItem>((item) =>
    {
        if (item is null) return;

        foreach (var text in item.Text.Split(" "))
        {
            var url = text;
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
        if (item is null) return;

        OpeningCompany openingCompany = new()
        {
            CompanyName = item.CompanyName
        };

        OpeningCompanies.Add(openingCompany);
        SearchKeyword = string.Empty;
    });
}
