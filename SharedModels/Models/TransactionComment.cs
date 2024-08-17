using Prism.Mvvm;

namespace SharedModels.Models;

public class TransactionComment : BindableBase
{
    public DateTime EntryDate { get; set; }

    private string commentText;
    private bool isEditing;

    public string CommentText
    {
        get => commentText;
        set => SetProperty(ref commentText, value);
    }

    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }
}
