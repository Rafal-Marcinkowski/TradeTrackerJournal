namespace SharedProject.Models;

public class Comment : BindableBase
{
    public DateTime EntryDate { get; set; }

    public int? TransactionID { get; set; }
    public int? EventID { get; set; }
    public int ID { get; set; }

    private string commentText;
    public string CommentText
    {
        get => commentText;
        set => SetProperty(ref commentText, value);
    }

    private bool isEditing;
    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }

    public string OldCommentText { get; set; }
}
