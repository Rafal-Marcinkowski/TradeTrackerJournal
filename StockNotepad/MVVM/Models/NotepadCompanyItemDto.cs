using System.Collections.ObjectModel;

namespace StockNotepad.MVVM.Models;

public class NotepadCompanyItemDto
{
    public string CompanyName { get; set; } = string.Empty;

    public CompanySummaryDto? Summary { get; set; }

    public ObservableCollection<NoteDto> Notes { get; set; } = [];
}
