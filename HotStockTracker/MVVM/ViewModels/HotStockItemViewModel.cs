using SharedProject.Models;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockItemViewModel : BindableBase
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public decimal Volume { get; set; }
    public decimal Turnover { get; set; }
    public decimal TurnoverMedian { get; set; }
    public decimal TurnoverDynamicsPercent { get; set; }

    public HotStockItemViewModel() { }

    public HotStockItemViewModel(HotStockItemDto dto)
    {
        Name = dto.Name;
        Price = dto.Price;
        Change = dto.Change;
        ChangePercent = dto.ChangePercent;
        Volume = dto.Volume;
        Turnover = dto.Turnover;
        TurnoverMedian = dto.TurnoverMedian;
        TurnoverDynamicsPercent = dto.TurnoverDynamicsPercent;
    }

    public HotStockItemDto ToDto() => new()
    {
        Name = Name,
        Price = Price,
        Change = Change,
        ChangePercent = ChangePercent,
        Volume = Volume,
        Turnover = Turnover,
        TurnoverMedian = TurnoverMedian,
        TurnoverDynamicsPercent = TurnoverDynamicsPercent
    };
}
