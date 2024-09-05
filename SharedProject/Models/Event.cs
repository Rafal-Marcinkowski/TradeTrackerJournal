using System.Collections.ObjectModel;

namespace SharedProject.Models;

public class Event : BindableBase
{
    public Event()
    {
        Comments = [];
        isNewCommentBeingAdded = false;
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

    private int entryMedianTurnover;
    public int EntryMedianTurnover
    {
        get => entryMedianTurnover;
        set => SetProperty(ref entryMedianTurnover, value);
    }

    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }

    public bool IsTracking { get; set; }
    public string? InformationLink { get; set; }

    public string? InitialDescription { get; set; }

    public ObservableCollection<DailyData> DailyDataCollection { get; set; }

    public ObservableCollection<Comment> Comments { get; set; }
}
