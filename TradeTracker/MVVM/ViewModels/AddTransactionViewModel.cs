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

    public TransactionEventViewModel Transaction { get; set; } = new();

    public AddTransactionViewModel(ITradeTrackerFacade facade, IDialogCoordinator dialogCoordinator)
    {
        this.facade = facade;
        this.dialogCoordinator = dialogCoordinator;
        facade.EventAggregator.GetEvent<TransactionAddedEvent>().Subscribe(_ => ClearFieldsCommand.Execute(null));
        facade.EventAggregator.GetEvent<TransactionAddedEvent>().Subscribe(async (transaction) => await UpdateUI(transaction.CompanyID));
        _ = GetAllCompanies();
    }

    protected override void OnCollectionFiltered()
    {
        ItemsSource = ObservableCollectionFilter.OrderByDescending(ItemsSource, c => c.TransactionCount);
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

    private async Task UpdateUI(int companyId)
    {
        var updatedCompany = ItemsSource.FirstOrDefault(c => c.ID == companyId);

        if (updatedCompany != null)
        {
            updatedCompany.TransactionCount++;
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
            await dialogCoordinator.ShowMessageAsync(this, "Sukces", "Transakcja pomyślnie dodana");
        }
    });

    public ICommand ClearFieldsCommand => new DelegateCommand(() =>
    {
        Transaction = new();
        OnPropertyChanged(nameof(Transaction));
        SearchKeyword = string.Empty;
    });
}
