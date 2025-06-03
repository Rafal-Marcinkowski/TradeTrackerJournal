using System.Text.Json.Serialization;

namespace EFCore.Models;

public class HotStockDay
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool IsSummaryExpanded { get; set; } = false;

    [JsonIgnore]
    public ICollection<HotStockItem> HotStockItems { get; set; } = [];
}
