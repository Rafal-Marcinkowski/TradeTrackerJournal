using DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using TradeTracker.MVVM.Views;
using ValidationComponent.Transactions;

namespace TradeTracker.MVVM.ViewModels;

public class OpenPositionsViewModel : BindableBase
{
    private readonly ITransactionData transactionData;

    public OpenPositionsViewModel(ITransactionData transactionData)
    {
        this.transactionData = transactionData;
        LoadTransactionsAsync();
    }

    public ICommand SetAvgSellPriceCommand => new DelegateCommand<Transaction>(transaction =>
    {
        string normalizedText = (new string(transaction.AvgSellPriceText.Replace(',', '.').Trim()
        .Where(q => !char.IsWhiteSpace(q)).ToArray()));
        if (decimal.TryParse(normalizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal avgSellPrice))
        {
            avgSellPrice = Math.Round(avgSellPrice, 2);
            transaction.AvgSellPrice = avgSellPrice;
        }
        transaction.AvgSellPriceText = string.Empty;
    });

    public ICommand CloseTransactionCommand => new DelegateCommand<Transaction>(async transaction =>
    {
        var validator = new CloseTransactionValidation();
        var results = validator.Validate(transaction);

        if (!results.IsValid)
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));
            MessageBox.Show(validationErrors, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (transaction.AvgSellPrice > 0)
        {
            var dialog = new ConfirmationDialog()
            {
                DialogText = "Czy na pewno chcesz zamknąć transakcję?\n" +
                $"{transaction.CompanyName} {transaction.EntryDate}\nŚrednia cena sprzedaży: {transaction.AvgSellPrice}"
            };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                var dialog2 = new FinalizeTransactionDialog();
                dialog2.ShowDialog();

                if (dialog2.IsConfirmed)
                {
                    var closingComment = dialog2.ClosingComment;
                    transaction.ClosingDescription = closingComment;
                }
                transaction.IsClosed = true;
                transaction.CloseDate = DateTime.Now;
                await transactionData.UpdateTransactionAsync(transaction);
                Transactions.Remove(transaction);
            }
        }
    });

    private async Task LoadTransactionsAsync()
    {
        try
        {
            var allTransactions = await transactionData.GetAllTransactionsAsync();
            Transactions = new ObservableCollection<Transaction>(allTransactions.Where(q => !q.IsClosed));
            foreach (var transaction in Transactions)
            {
                await SetDuration(transaction);
            }
            RaisePropertyChanged(nameof(Transactions));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading transactions: {ex.Message}");
        }
    }

    private async Task SetDuration(Transaction transaction)
    {
        TimeSpan timeSpan = DateTime.Now.Date - transaction.EntryDate.Date;
        transaction.Duration = (int)timeSpan.TotalDays;
    }

    public ObservableCollection<Transaction> Transactions { get; set; }
}
