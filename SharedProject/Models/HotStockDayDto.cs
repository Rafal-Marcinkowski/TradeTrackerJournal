namespace SharedProject.Models;

public class HotStockDayDto
{
    public DateTime Date { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool IsSummaryExpanded { get; set; }
    public List<HotStockItemDto> Items { get; set; } = [];
}
