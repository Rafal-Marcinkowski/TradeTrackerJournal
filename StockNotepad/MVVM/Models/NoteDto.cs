namespace StockNotepad.MVVM.Models;

public class NoteDto : BindableBase
{
    public int Id { get; set; }

    private string title = string.Empty;
    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }


    private string content = string.Empty;
    public string Content
    {
        get => content;
        set => SetProperty(ref content, value);
    }

    public DateTime CreatedAt { get; set; }

    private bool isEditing = false;
    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }

    public string TitleBackup = string.Empty;
    public string ContentBackup = string.Empty;
}
