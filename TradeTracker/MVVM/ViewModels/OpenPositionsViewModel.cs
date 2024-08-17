using Prism.Commands;
using Prism.Mvvm;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class OpenPositionsViewModel : BindableBase
{
    public ICommand SetAvgSellPriceCommand => new DelegateCommand<Transaction>(transaction =>
    {
        string normalizedText = transaction.AvgSellPriceText.Replace(',', '.').Trim();
        if (double.TryParse(normalizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out double avgSellPrice))
        {
            avgSellPrice = Math.Round(avgSellPrice, 2);
            transaction.AvgSellPrice = avgSellPrice;
        }
        transaction.AvgSellPriceText = string.Empty;
    });

    public ICommand CloseTransactionCommand => new DelegateCommand<Transaction>(transaction =>
    {
        if (transaction.AvgSellPrice > 0)
        {
            var dialog = new ConfirmationDialog()
            {
                DialogText = "Czy na pewno chcesz zamknąć transakcję?\n" +
                $"{transaction.CompanyName} {transaction.EntryDate}\nŚrednia cena sprzedaży ustawiona na: {transaction.AvgSellPrice}"
            };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                Transactions.Remove(transaction);
            }
        }
    });

    public ObservableCollection<Transaction> Transactions { get; set; } =
    [

        new Transaction
    {
        CompanyName = "Columbus",
        InitialDescription = "Komentarz otwierający Columbus",
        ClosingDescription = "halohalo",
        EntryDate = DateTime.Now,
        AvgSellPrice = 13.23,
        NumberOfShares = 1182,
        PositionSize = 18723,
        EntryPrice = 100,
        EntryMedianVolume = 500,
        IsClosed = false,
        CloseDate = DateTime.Now,
        DayOpenPrice = new List<double>{ 89, 23, 54},
        EndOfDayPrice = new List<double> { 131, 170, 14 },
        DayPriceChange = new List<double> { 2, -83, 65 },
        DayVolume = new List<double> { 520, 54222, 1342 },
        DayVolumeChange = new List<double> { 4, 7300, -32 },
        DayMin = new List<double> { 98, 101, 132 },
        DayMax = new List<double> { 105, 324, 98}
    },
         new Transaction
    {
        CompanyName = "Polwax",
        InitialDescription = "Komentarz otwierający Polwax",
        EntryDate = DateTime.Now,
        EntryPrice = 79,
        NumberOfShares = 182,
        PositionSize = 1223,
        IsClosed= false,
        EntryMedianVolume = 123678,
        DayOpenPrice = new List<double>{ 89, 23, 54},
        EndOfDayPrice = new List<double> { 131, 170, 14 },
        DayPriceChange = new List<double> { -8, 17, -123 },
        DayVolume = new List<double> { 11234, 2132, 765 },
        DayVolumeChange = new List<double> { 1082, 708, 1232 },
        DayMin = new List<double> { 67, 71, 98 },
        DayMax = new List<double> { 105, 121, -131 },
        Comments = new ObservableCollection<TransactionComment>
        {
            new TransactionComment()
        {
            EntryDate = DateTime.Now,
            CommentText = "pierwszy komentarz, jakiś tam"
        }  ,
        new TransactionComment()
        {
            EntryDate= DateTime.Now,
            CommentText  = "drugi komentatrez dla wyswietlen"
        }
        }
    }
    ];
}
