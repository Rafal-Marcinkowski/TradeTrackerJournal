namespace StockNotepad.MVVM.Models;

public class CompanySummaryDto : BindableBase
{
    private string content = string.Empty;
    public string Content
    {
        get => content;
        set => SetProperty(ref content, value);
    }

    private DateTime updatedAt;
    public DateTime UpdatedAt
    {
        get => updatedAt;
        set => SetProperty(ref updatedAt, value);
    }
}
