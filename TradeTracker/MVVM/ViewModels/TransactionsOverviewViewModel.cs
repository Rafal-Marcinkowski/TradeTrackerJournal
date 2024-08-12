using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewViewModel : BindableBase
{


    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>
    {
        new Transaction
    {
        CompanyName = "Columbus",
        EntryDate = DateTime.Now,
        EntryPrice = 100,
        MedianVolume = 500,
        OpenPrice = new List<decimal>{ 89, 23, 54},
        EndOfDayPrice = new List<decimal> { 131, 170, 14 },
        DayPriceChange = new List<decimal> { 2, -83, 65 },
        DayVolume = new List<decimal> { 520, 54222, 1342 },
        DayVolumeChange = new List<decimal> { 4, 7300, -32 },
        DayMin = new List<decimal> { 98, 101, 132 },
        DayMax = new List<decimal> { 105, 324, 98}
    },
         new Transaction
    {
        CompanyName = "Polwax",
        EntryDate = DateTime.Now,
        EntryPrice = 79,
        MedianVolume = 123678,
        OpenPrice = new List<decimal>{ 89, 23, 54},
        EndOfDayPrice = new List<decimal> { 131, 170, 14 },
        DayPriceChange = new List<decimal> { -8, 17, -123 },
        DayVolume = new List<decimal> { 11234, 2132, 765 },
        DayVolumeChange = new List<decimal> { 1082, 708, 1232 },
        DayMin = new List<decimal> { 67, 71, 98 },
        DayMax = new List<decimal> { 105, 121, -131 }
    }
    };
}

