using DataAccess.Data;
using Infrastructure;
using Infrastructure.DataFilters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using TradeTracker.MVVM.Views;
using ValidationComponent.Transactions;

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

    private string entryDate = string.Empty;
    public string EntryDate
    {
        get => entryDate;
        set => SetProperty(ref entryDate, value);
    }

    private string entryPrice = string.Empty;
    public string EntryPrice
    {
        get => entryPrice;
        set => SetProperty(ref entryPrice, value);
    }

    private string positionSize = string.Empty;
    public string PositionSize
    {
        get => positionSize;
        set => SetProperty(ref positionSize, value);
    }

    private string numberOfShares = string.Empty;
    public string NumberOfShares
    {
        get => numberOfShares;
        set => SetProperty(ref numberOfShares, value);
    }

    private string avgSellPrice = string.Empty;
    public string AvgSellPrice
    {
        get => avgSellPrice;
        set => SetProperty(ref avgSellPrice, value);
    }

    private string initialDescription = string.Empty;
    public string InitialDescription
    {
        get => initialDescription;
        set => SetProperty(ref initialDescription, value);
    }

    private string informationLink = string.Empty;
    public string InformationLink
    {
        get => informationLink;
        set => SetProperty(ref informationLink, value);
    }

    private void FilterCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText);
    }

    private void OrderFilteredCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.OrderByDescendingTransactionCount(FilteredCompanies);
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
            companies = new ObservableCollection<Company>(companyList.OrderByDescending(q => q.TransactionCount));
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
        if (selectedCompany is not null)
        {
            SelectedCompanyName = selectedCompany.CompanyName;
        }
    });

    private DateTime ParseEntryDate(string input)
    {
        var normalizedInput = input.Replace(" ", "").Replace(",", "").Replace(".", "").Replace(";", "").Trim();

        if (normalizedInput.Length == 12)
        {
            string datePart = normalizedInput.Substring(0, 8);
            string timePart = normalizedInput.Substring(8, 4);

            string formattedDateTime = $"{datePart.Substring(0, 2)}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)} {timePart.Substring(0, 2)}:{timePart.Substring(2, 2)}";

            if (DateTime.TryParseExact(formattedDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
            {
                return fullDate;
            }
        }
        else if (normalizedInput.Length == 8)
        {
            string datePart = normalizedInput.Substring(0, 8);

            string formattedDate = $"{datePart.Substring(0, 2)}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)}";

            if (DateTime.TryParseExact(formattedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            {
                return dateOnly;
            }
        }
        else if (normalizedInput.Length == 4 || normalizedInput.Length == 3)
        {
            string timePart = normalizedInput.Length == 4 ? normalizedInput : $"0{normalizedInput.Substring(0, 1)}{normalizedInput.Substring(1, 2)}"; // Dodaj zero przed minutą, jeśli jest podana jako jedna cyfra

            if (TimeSpan.TryParseExact(timePart, "hhmm", CultureInfo.InvariantCulture, out var timeOnly))
            {
                return DateTime.Now.Date.Add(timeOnly);
            }
        }

        return new DateTime(1900, 1, 1, 0, 0, 0);
    }

    public ICommand AddTransactionCommand => new DelegateCommand(async () =>
    {
        if (!String.IsNullOrEmpty(SelectedCompanyName))
        {
            Transaction transaction = new()
            {
                CompanyName = SelectedCompanyName,
                EntryDate = ParseEntryDate(EntryDate),
                //EntryDate = DateTime.Now.Date.Add(TimeSpan.TryParse(EntryDate.Replace(",", ":").Replace(".", ":")
                //  .Replace(";", ":"), out var timeComponent) ? timeComponent : TimeSpan.Zero),
                EntryPrice = decimal.TryParse(EntryPrice.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out var entryPrice) ? entryPrice : 0,
                NumberOfShares = int.TryParse(NumberOfShares.Where(x => !char.IsWhiteSpace(x)).ToArray(), out var numberOfShares) ? numberOfShares : 0,
                PositionSize = decimal.TryParse(PositionSize.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out decimal positionSize) ? positionSize : 0,
                InformationLink = InformationLink,
                AvgSellPrice = decimal.TryParse(AvgSellPrice.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out var avgSellPrice) ? avgSellPrice : (decimal?)null,
                EntryMedianTurnover = int.Parse(await TurnoverMedianTable.GetTurnoverMedianForCompany(SelectedCompanyName)),
                InitialDescription = InitialDescription,
            };

            var validator = new AddTransactionValidator();
            var results = validator.Validate(transaction);

            if (!results.IsValid)
            {
                var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));
                MessageBox.Show(validationErrors, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (transaction.AvgSellPrice != null)
                {
                    transaction.CloseDate = DateTime.Now;
                    transaction.IsClosed = true;
                    transaction.ClosingDescription = "Transakcja zamknięta przy dodaniu.";
                }
                ///sprawdzic czy podobna transakcja istnieje?
                transaction.CompanyID = await companyData.GetCompanyID(SelectedCompanyName);
                var dialog = new ConfirmationDialog()
                {
                    DialogText = $"Czy dodać transakcję? \n{transaction.CompanyName}\n{transaction.EntryDate}\nCena kupna: {transaction.EntryPrice}\n" +
                    $"Ilość akcji: {transaction.NumberOfShares}\nWielkość pozycji: {transaction.PositionSize}"
                };
                dialog.ShowDialog();

                if (dialog.Result)
                {
                    await transactionData.InsertTransactionAsync(transaction);
                    var company = await companyData.GetCompanyAsync(transaction.CompanyID);
                    company.TransactionCount++;
                    await companyData.UpdateCompanyAsync(company.ID, SelectedCompanyName, company.TransactionCount);
                    var updatedCompany = FilteredCompanies.FirstOrDefault(c => c.ID == company.ID);
                    if (updatedCompany != null)
                    {
                        updatedCompany.TransactionCount = company.TransactionCount;
                    }

                    ClearFieldsCommand.Execute(null);
                    OrderFilteredCompanies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    });

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {
        EntryDate = string.Empty;
        EntryPrice = string.Empty;
        PositionSize = string.Empty;
        NumberOfShares = string.Empty;
        InformationLink = string.Empty;
        InitialDescription = string.Empty;
        SearchBoxText = string.Empty;
        SelectedCompanyName = string.Empty;
        AvgSellPrice = string.Empty;
    });
}
