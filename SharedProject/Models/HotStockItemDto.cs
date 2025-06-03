namespace SharedProject.Models;

public class HotStockItemDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public decimal Volume { get; set; }
    public decimal Turnover { get; set; }
    public decimal TurnoverMedian { get; set; }
    public decimal TurnoverDynamicsPercent { get; set; }
}
