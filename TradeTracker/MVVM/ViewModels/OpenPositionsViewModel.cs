using Infrastructure.Events;
using Infrastructure.Interfaces;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class OpenPositionsViewModel : BindableBase
{
    private readonly ITradeTrackerFacade facade;

    public OpenPositionsViewModel(ITradeTrackerFacade facade)
    {
        this.facade = facade;
        this.facade.EventAggregator.GetEvent<TransactionClosedEvent>().Subscribe(async (transaction) => await HandleTransactionClosed(transaction));
        _ = LoadTransactionsAsync();
    }

    public ObservableCollection<Transaction> Transactions { get; set; }

    public ICommand SetAvgSellPriceCommand => new DelegateCommand<Transaction>(transaction =>
    {
        string normalizedText = (new string([.. transaction.AvgSellPriceText.Replace(',', '.').Trim().Where(q => !char.IsWhiteSpace(q))]));

        if (decimal.TryParse(normalizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal avgSellPrice))
        {
            avgSellPrice = Math.Round(avgSellPrice, 2);
            transaction.AvgSellPrice = avgSellPrice;
        }

        transaction.AvgSellPriceText = string.Empty;
    });

    public ICommand CloseTransactionCommand => new DelegateCommand<Transaction>(async transaction => await facade.TransactionManager.CloseTransaction(transaction));

    private async Task HandleTransactionClosed(Transaction transaction)
    {
        Transactions.Remove(transaction);
        RaisePropertyChanged(nameof(Transactions));
    }

    private async Task LoadTransactionsAsync()
    {
        try
        {
            Transactions = [.. await facade.TransactionManager.LoadAndSetOpenTransactions()];

            RaisePropertyChanged(nameof(Transactions));
        }

        catch (Exception ex)
        {
            MessageBox.Show($"Error loading transactions: {ex.Message}");
        }
    }
}
