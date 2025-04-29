using Infrastructure.DataFilters;
using Infrastructure.Events;
using Infrastructure.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using SharedProject.Models;
using SharedProject.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class AddTransactionViewModel : BaseListViewModel<Company>
{
    private readonly ITradeTrackerFacade facade;
    private readonly IDialogCoordinator dialogCoordinator;

    public TransactionViewModel Transaction { get; set; } = new();

    public AddTransactionViewModel(ITradeTrackerFacade facade, IEventAggregator eventAggregator
         , IDialogCoordinator dialogCoordinator)
    {
        this.facade = facade;
        this.dialogCoordinator = dialogCoordinator;
        facade.EventAggregator.GetEvent<TransactionAddedEvent>().Subscribe(_ => ClearFieldsCommand.Execute(null));
        _ = GetAllCompanies();
    }

    protected override void OnCollectionFiltered()
    {
        ItemsSource = ObservableCollectionFilter.OrderByDescendingTransactionCount(ItemsSource);
    }

    private async Task GetAllCompanies()
    {
        try
        {
            var companyList = await facade.CompanyManager.GetAllCompanies();
            ItemsSource = [.. companyList.OrderByDescending(q => q.TransactionCount)];
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
            Transaction.SelectedCompanyName = selectedCompany.CompanyName;
        }
    });

    public ICommand AddTransactionCommand => new DelegateCommand(async () =>
    {
        if (await facade.TransactionManager.TryAddTransaction(Transaction))
        {
            await dialogCoordinator.ShowMessageAsync(this, "Sukces", "transakcja pomyślnie dodana");
        }
    });

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {
        Transaction = new();
        OnPropertyChanged(nameof(Transaction));
        SearchKeyword = string.Empty;
    });
}
