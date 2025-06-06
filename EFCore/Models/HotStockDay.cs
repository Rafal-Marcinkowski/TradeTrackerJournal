using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore.Models;

public class HotStockDay
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(1000)]
    public string Summary { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string OpeningComment { get; set; } = string.Empty;
    public bool IsSummaryExpanded { get; set; } = false;

    [JsonIgnore]
    public ICollection<HotStockItem> HotStockItems { get; set; } = [];
}
