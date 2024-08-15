using Prism.Mvvm;
using TradeTracker.MVVM.Models;

public class Transaction : BindableBase
{
    private bool isDetailsVisible;
    public bool IsDetailsVisible
    {
        get => isDetailsVisible;
        set
        {
            isDetailsVisible = value;
            RaisePropertyChanged();
        }
    }
    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal EntryPrice { get; set; }
    public int EntryMedianVolume { get; set; }
    public bool IsClosed { get; set; }

    public string InitialDescription { get; set; }
    public string ClosingDescription { get; set; }

    public List<decimal> DayOpenPrice { get; set; }
    public List<decimal> EndOfDayPrice { get; set; }
    public List<decimal> DayPriceChange { get; set; }
    public List<decimal> DayVolume { get; set; }
    public List<decimal> DayVolumeChange { get; set; }
    public List<decimal> DayMin { get; set; }
    public List<decimal> DayMax { get; set; }

    public List<TransactionComment> Comments { get; set; }
}

