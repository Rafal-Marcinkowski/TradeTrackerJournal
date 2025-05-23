namespace SharedProject.Models;

public class HotStockItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public decimal Volume { get; set; }
    public decimal Turnover { get; set; }
    public decimal TurnoverMedian { get; set; }
    public decimal TurnoverDynamicsPercent { get; set; }
}
