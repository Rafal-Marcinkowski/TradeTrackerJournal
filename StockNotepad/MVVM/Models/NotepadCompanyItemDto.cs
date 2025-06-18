namespace StockNotepad.MVVM.Models;

public class NotepadCompanyItemDto
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public CompanySummaryDto? Summary { get; set; }

    public List<NoteDto>? Notes { get; set; } = [];
}
