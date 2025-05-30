using EFCore.Models;
using HotStockTracker.MVVM.ViewModels;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace HotStockTracker;

public static class HotStockDayMapper
{
    public static HotStockDayViewModel ToViewModel(this HotStockDay day)
    {
        var items = day.HotStockItems?
            .Select(i => new HotStockItemDto
            {
                Name = i.Name,
                Price = i.Price,
                Change = i.Change,
                ChangePercent = i.ChangePercent,
                Volume = i.Volume,
                Turnover = i.Turnover,
                TurnoverMedian = i.TurnoverMedian,
                TurnoverDynamicsPercent = i.TurnoverDynamicsPercent
            }).ToList() ?? [];

        return new HotStockDayViewModel
        {
            Date = day.Date,
            Summary = day.Summary,
            IsSummaryExpanded = day.IsSummaryExpanded,
            HotStockItems = items,
            TopGainers = new ObservableCollection<HotStockItemDto>(
                items.OrderByDescending(i => i.ChangePercent).Take(10)),
            TopLosers = new ObservableCollection<HotStockItemDto>(
                items.OrderBy(i => i.ChangePercent).Take(10))
        };
    }
}
