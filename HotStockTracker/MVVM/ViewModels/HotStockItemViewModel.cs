using SharedProject.Models;
using System.Globalization;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockItemViewModel
{
    public string Name { get; set; }
    public string Market { get; set; }
    public string Price { get; set; }
    public string Change { get; set; }
    public string ChangePercent { get; set; }
    public string ReferencePrice { get; set; }
    public string OpenPrice { get; set; }
    public string MinPrice { get; set; }
    public string MaxPrice { get; set; }
    public string Volume { get; set; }
    public string Turnover { get; set; }

    public HotStockItemViewModel() { }

    public HotStockItemViewModel(HotStockItemDto dto)
    {
        Name = dto.Name;
        Market = dto.Market;
        Price = dto.Price;
        Change = dto.Change;
        ChangePercent = dto.ChangePercent;
        ReferencePrice = dto.ReferencePrice;
        OpenPrice = dto.OpenPrice;
        MinPrice = dto.MinPrice;
        MaxPrice = dto.MaxPrice;
        Volume = dto.Volume;
        Turnover = dto.Turnover;
    }

    public HotStockItemDto ToDto() => new()
    {
        Name = Name,
        Market = Market,
        Price = Price,
        Change = Change,
        ChangePercent = ChangePercent,
        ReferencePrice = ReferencePrice,
        OpenPrice = OpenPrice,
        MinPrice = MinPrice,
        MaxPrice = MaxPrice,
        Volume = Volume,
        Turnover = Turnover
    };

    public decimal ParsedChangePercent =>
        decimal.TryParse(ChangePercent.Replace("%", "").Replace("+", "").Replace(",", "."),
            CultureInfo.InvariantCulture, out var result)
        ? result
        : 0m;
}