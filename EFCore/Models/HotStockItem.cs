using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore.Models;

public class HotStockItem
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(25)]
    public string Market { get; set; }

    [MaxLength(25)]
    public string Price { get; set; }

    [MaxLength(25)]
    public string Change { get; set; }

    [MaxLength(25)]
    public string ChangePercent { get; set; }

    [MaxLength(25)]
    public string ReferencePrice { get; set; }

    [MaxLength(25)]
    public string OpenPrice { get; set; }

    [MaxLength(25)]
    public string MinPrice { get; set; }

    [MaxLength(25)]
    public string MaxPrice { get; set; }

    [MaxLength(25)]
    public string Volume { get; set; }

    [MaxLength(25)]
    public string Turnover { get; set; }

    public int HotStockDayId { get; set; }

    [JsonIgnore]
    public HotStockDay? HotStockDay { get; set; }
}
