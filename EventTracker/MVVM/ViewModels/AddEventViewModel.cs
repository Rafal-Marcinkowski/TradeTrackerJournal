using Infrastructure.DataFilters;
using Infrastructure.Events;
using Infrastructure.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class AddEventViewModel : BaseListViewModel<Company>
{
    public AddEventViewModel(ITradeTrackerFacade facade, IDialogCoordinator dialogCoordinator)
    {
        this.facade = facade;
        this.dialogCoordinator = dialogCoordinator;

        this.facade.EventAggregator.GetEvent<EventAddedEvent>().Subscribe(_ => ClearFieldsCommand.Execute(null));
        this.facade.EventAggregator.GetEvent<EventAddedEvent>().Subscribe(async (ev) => await UpdateUI(ev.CompanyID));

        _ = GetAllCompanies();
    }

    public TransactionEventViewModel Event { get; set; } = new();

    private readonly ITradeTrackerFacade facade;
    private readonly IDialogCoordinator dialogCoordinator;

    protected override void OnCollectionFiltered()
    {
        ItemsSource = ObservableCollectionFilter.OrderByDescending(ItemsSource, c => c.EventCount);
    }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await facade.CompanyManager.GetAllCompanies();
            ItemsSource = [.. companyList.OrderByDescending(q => q.EventCount)];
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading companies: {ex.Message}");
            Log.Error(ex, "Error loading companies.");
        }
    }

    private async Task UpdateUI(int companyId)
    {
        var updatedCompany = ItemsSource.FirstOrDefault(c => c.ID == companyId);
        if (updatedCompany != null)
        {
            updatedCompany.EventCount++;
        }
    }

    public ICommand ConfirmCompanySelectionCommand => new DelegateCommand<Company>((selectedCompany) =>
    {
        if (selectedCompany is not null)
        {
            Event.SelectedCompanyName = selectedCompany.CompanyName;
        }
    });

    public ICommand AddEventCommand => new DelegateCommand(async () =>
    {
        if (await facade.EventManager.TryAddEvent(Event))
        {
            await dialogCoordinator.ShowMessageAsync(this, "Sukces", "Wydarzenie pomyślnie dodane");
        }
    });

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {
        Event = new();
        OnPropertyChanged(nameof(Event));
        SearchKeyword = string.Empty;
    });
}
