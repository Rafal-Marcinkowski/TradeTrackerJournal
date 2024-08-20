namespace SharedModels.Models;

public class DailyData
{
    public int ID { get; set; }
    public int TransactionID { get; set; }
    public DateTime Date { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal ClosePrice { get; set; }
    public decimal Volume { get; set; }
    public decimal Turnover { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal? PriceChange { get; set; }
    public decimal? TurnoverChange { get; set; }
    public int? TransactionCount { get; set; }
}
