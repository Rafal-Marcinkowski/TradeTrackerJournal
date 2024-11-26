using DataAccess.Data;
using Infrastructure.DataFilters;
using Infrastructure.Events;
using Serilog;
using SharedProject.Models;
using SharedProject.Views;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using ValidationComponent.Events;

namespace EventTracker.MVVM.ViewModels;

public class AddEventViewModel : BindableBase
{
    private readonly ICompanyData companyData;
    private readonly IEventData eventData;
    private readonly IEventAggregator eventAggregator;
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
            SetProperty(ref searchBoxText, value, () => FilterCompanies());
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
        if (FilteredCompanies is not null)
        {
            FilteredCompanies = FilteredCompanies.Count > 0 ? ObservableCollectionFilter.FilterCompaniesViaTextBoxText(companies, SearchBoxText) : [];
        }
    }

    private void OrderFilteredCompanies()
    {
        FilteredCompanies = ObservableCollectionFilter.OrderByDescendingEventCount(FilteredCompanies);
    }

    public AddEventViewModel(ICompanyData companyData, IEventAggregator eventAggregator, IEventData eventData)
    {
        this.eventData = eventData;
        this.eventAggregator = eventAggregator;
        this.companyData = companyData;
        GetAllCompanies();
    }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await companyData.GetAllCompaniesAsync();
            companies = new ObservableCollection<Company>(companyList.OrderByDescending(q => q.EventCount));
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

        SearchBoxText = string.Empty;
    });

    private DateTime ParseEntryDate(string input)
    {
        var normalizedInput = input.Replace(" ", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Trim();

        if (normalizedInput.Length == 12)
        {
            string datePart = normalizedInput[..8];
            string timePart = normalizedInput.Substring(8, 4);

            string formattedDateTime = $"{datePart[..2]}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)} {timePart[..2]}:{timePart.Substring(2, 2)}";

            if (DateTime.TryParseExact(formattedDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
            {
                return fullDate;
            }
        }

        else if (normalizedInput.Length == 8)
        {
            string datePart = normalizedInput.Substring(0, 8);

            string formattedDate = $"{datePart[..2]}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)}";

            if (DateTime.TryParseExact(formattedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            {
                return dateOnly;
            }
        }

        else if (normalizedInput.Length == 4 || normalizedInput.Length == 3)
        {
            string timePart = normalizedInput.Length == 4 ? normalizedInput : $"0{normalizedInput[..1]}{normalizedInput.Substring(1, 2)}";

            if (TimeSpan.TryParseExact(timePart, "hhmm", CultureInfo.InvariantCulture, out var timeOnly))
            {
                return DateTime.Now.Date.Add(timeOnly);
            }
        }

        return new DateTime(1900, 1, 1, 0, 0, 0);
    }

    public ICommand AddEventCommand => new DelegateCommand(async () =>
    {
        Event e = await FillNewEventProperties();

        if (!await ValidateNewEventProperties(e))
        {
            return;
        }

        try
        {
            e.CompanyID = await companyData.GetCompanyID(SelectedCompanyName);

            if (!await CheckEventValidity(e))
            {
                return;
            }

            await ConfirmEvent(e);
        }

        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    });

    private async Task ConfirmEvent(Event e)
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = $"Czy dodać zdarzenie? \n" +
                  $"{e.CompanyName}\n" +
                  $"{e.EntryDate}\n" +
                  $"Początkowy kurs: {e.EntryPrice}\n"
        };

        dialog.ShowDialog();

        if (dialog.Result)
        {
            await eventData.InsertEventAsync(e);
            var company = await companyData.GetCompanyAsync(e.CompanyID);
            company.EventCount++;
            await companyData.UpdateCompanyAsync(company.ID, SelectedCompanyName, company.TransactionCount, company.EventCount);
            var updatedCompany = FilteredCompanies.FirstOrDefault(c => c.ID == company.ID);

            if (updatedCompany != null)
            {
                updatedCompany.EventCount = company.EventCount;
            }

            ClearFieldsCommand.Execute(null);
            OrderFilteredCompanies();
            await eventData.UpdateEventAsync(e);
            e.ID = await eventData.GetID(e);
            eventAggregator.GetEvent<EventAddedEvent>().Publish(e);
        }
    }

    private async Task<bool> CheckEventValidity(Event e)
    {
        if (await new CheckEvent(eventData).IsExisting(e))
        {
            var errorDialog = new ErrorDialog()
            {
                DialogText = $"Zdarzenie już istnieje w bazie danych!"
            };

            errorDialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateNewEventProperties(Event e)
    {
        var validator = new AddEventValidator();
        var results = validator.Validate(e);

        if (!results.IsValid)
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));

            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };

            dialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<Event> FillNewEventProperties()
    {
        Event e = new()
        {
            CompanyName = SelectedCompanyName,
            EntryDate = ParseEntryDate(EntryDate),
            EntryPrice = decimal.TryParse(EntryPrice.Replace(".", ",").Where(x => !char.IsWhiteSpace(x))
                  .ToArray(), out var entryPrice) ? entryPrice : 0,
            InformationLink = InformationLink,
            InitialDescription = InitialDescription,
        };

        return e;
    }

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {
        EntryDate = string.Empty;
        EntryPrice = string.Empty;
        InformationLink = string.Empty;
        InitialDescription = string.Empty;
        SearchBoxText = string.Empty;
        SelectedCompanyName = string.Empty;
    });
}
