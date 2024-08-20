using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace SharedModels.Models;

public class Transaction : BindableBase
{
    public Transaction()
    {
        Comments = [];
        isNewCommentBeingAdded = false;
        IsClosed = false;
        AvgPriceOfTheDay = [];
        DailyDataCollection = [];
        IsTracking = true;
    }

    public int ID { get; set; }
    public int CompanyID { get; set; }

    private bool isDetailsVisible;
    public bool IsDetailsVisible
    {
        get => isDetailsVisible;
        set => SetProperty(ref isDetailsVisible, value);
    }

    private bool isNewCommentBeingAdded;
    public bool IsNewCommentBeingAdded
    {
        get => isNewCommentBeingAdded;
        set => SetProperty(ref isNewCommentBeingAdded, value);
    }

    private string avgSellPriceText = string.Empty;
    public string AvgSellPriceText
    {
        get => avgSellPriceText;
        set => SetProperty(ref avgSellPriceText, value);
    }

    private decimal? avgSellPrice;
    public decimal? AvgSellPrice
    {
        get => avgSellPrice;
        set => SetProperty(ref avgSellPrice, value);
    }

    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public decimal EntryPrice { get; set; }

    public int EntryMedianTurnover { get; set; }
    public int NumberOfShares { get; set; }
    public decimal PositionSize { get; set; }
    public int Duration { get; set; }
    public bool IsClosed { get; set; }
    public bool IsTracking { get; set; }
    public string? InformationLink { get; set; }

    public string? InitialDescription { get; set; }
    public string? ClosingDescription { get; set; }

    public List<decimal>? AvgPriceOfTheDay { get; set; }
    public ObservableCollection<DailyData> DailyDataCollection { get; set; }

    public ObservableCollection<TransactionComment> Comments { get; set; }
}
