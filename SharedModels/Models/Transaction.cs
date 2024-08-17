using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace SharedModels.Models;

public class Transaction : BindableBase
{
    public Transaction()
    {
        Comments = new ObservableCollection<TransactionComment>();
        isNewCommentBeingAdded = false;
        IsClosed = false;
    }

    public int ID { get; set; }

    public int CompanyID { get; set; }

    private bool isDetailsVisible;
    public bool IsDetailsVisible
    {
        get => isDetailsVisible;
        set
        {
            SetProperty(ref isDetailsVisible, value);
        }
    }

    private bool isNewCommentBeingAdded;
    public bool IsNewCommentBeingAdded
    {
        get => isNewCommentBeingAdded;
        set
        {
            SetProperty(ref isNewCommentBeingAdded, value);
        }
    }

    private string avgSellPriceText;
    public string AvgSellPriceText
    {
        get => avgSellPriceText;
        set => SetProperty(ref avgSellPriceText, value);
    }

    private double avgsellPrice;
    public double AvgSellPrice
    {
        get => avgsellPrice;
        set => SetProperty(ref avgsellPrice, value);
    }

    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime CloseDate { get; set; }
    public double EntryPrice { get; set; }


    public int EntryMedianVolume { get; set; }
    public int NumberOfShares { get; set; }
    public int PositionSize { get; set; }
    public bool IsClosed { get; set; }
    public int Duration { get; set; }

    public string InitialDescription { get; set; }
    public string ClosingDescription { get; set; }

    public List<double> DayOpenPrice { get; set; }
    public List<double> EndOfDayPrice { get; set; }
    public List<double> DayPriceChange { get; set; }
    public List<double> DayVolume { get; set; }
    public List<double> DayVolumeChange { get; set; }
    public List<double> DayMin { get; set; }
    public List<double> DayMax { get; set; }
    public List<double>? AvgPriceOfTheDay { get; set; }

    public ObservableCollection<TransactionComment> Comments { get; set; }
}

