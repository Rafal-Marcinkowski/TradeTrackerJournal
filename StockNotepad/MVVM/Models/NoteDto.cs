namespace StockNotepad.MVVM.Models;

public class NoteDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsEditing { get; set; } = false;
}
