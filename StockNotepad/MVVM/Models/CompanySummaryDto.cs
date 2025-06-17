namespace StockNotepad.MVVM.Models;

public class CompanySummaryDto
{
    public string Content { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }

    public bool IsEditing { get; set; } = false;
}
