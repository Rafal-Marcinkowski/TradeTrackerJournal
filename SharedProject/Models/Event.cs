using SharedProject.Interfaces;
using System.Collections.ObjectModel;

namespace SharedProject.Models;

public class Event : BindableBase, ITrackable, IDetailable, ILinkable, ICommentable
{
    public Event()
    {
        Comments = [];
        isNewCommentBeingAdded = false;
        DailyDataCollection = [];
        IsTracking = true;
        IsClosed = true;
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

    public string FinalComment { get; set; }

    private string newCommentText;
    public string NewCommentText
    {
        get => newCommentText;
        set
        {
            SetProperty(ref newCommentText, value);
        }
    }

    private bool isCommentBeingEdited;
    public bool IsCommentBeingEdited
    {
        get => isCommentBeingEdited;
        set
        {
            SetProperty(ref isCommentBeingEdited, value);
        }
    }

    private bool hasFinalComment;
    public bool HasFinalComment
    {
        get => hasFinalComment;
        set
        {
            SetProperty(ref hasFinalComment, value);
        }
    }

    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }
    public decimal EntryPrice { get; set; }

    public bool IsTracking { get; set; }
    public string? InformationLink { get; set; }

    public string? InitialDescription { get; set; }

    public ObservableCollection<DailyData> DailyDataCollection { get; set; }

    public ObservableCollection<Comment> Comments { get; set; }
    public bool IsClosed { get; set; }
    public string? Description { get; set; }
}
