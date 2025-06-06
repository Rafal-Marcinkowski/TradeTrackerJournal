namespace SharedProject.Models;

public class HotStockDayDto
{
    public DateTime Date { get; set; }
    public string Summary { get; set; }
    public string OpeningComment { get; set; }
    public bool IsSummaryExpanded { get; set; }

    public List<HotStockItemDto> HotStockItems { get; set; }
}
