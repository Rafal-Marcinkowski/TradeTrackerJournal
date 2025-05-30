using EFCore.Models;
using HotStockTracker.MVVM.ViewModels;
using Infrastructure.DownloadHtmlData;
using Infrastructure.GetDataFromHtml;

namespace HotStockTracker.Services;

public class HotStockDayManager(HotStockApiClient apiClient)
{
    public async Task<List<HotStockDayViewModel>> GetLatestDaysAsync(int count = 5)
    {
        var days = await apiClient.GetHotStockDaysAsync();
        return [.. days.OrderByDescending(d => d.Date).Take(count)];
    }

    public async Task<bool> HasDataForDateAsync(DateTime date)
    {
        var days = await apiClient.GetHotStockDaysAsync();
        return days.Any(d => d.Date == date);
    }

    public async Task AddNewDayIfMissingAsync()
    {
        var today = DateTime.Today;
        if (await HasDataForDateAsync(today))
            return;

        var html = await DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/dynamika-obrotow/4,2");
        var records = await new HotStockParser().ParseHotStocks(html);

        var topGainers = records.OrderByDescending(r => r.ChangePercent).Take(10);
        var topLosers = records.OrderBy(r => r.ChangePercent).Take(10);
        var allItems = topGainers.Concat(topLosers)
            .Select(r => new HotStockItem
            {
                Name = r.Name,
                Price = r.Price,
                Change = r.Change,
                ChangePercent = r.ChangePercent,
                Volume = r.Volume,
                Turnover = r.Turnover,
                TurnoverMedian = r.TurnoverMedian,
                TurnoverDynamicsPercent = r.TurnoverDynamicsPercent
            }).ToList();

        var newDay = new HotStockDay
        {
            Date = today,
            Summary = "",
            IsSummaryExpanded = true,
            HotStockItems = allItems
        };

        await apiClient.AddFullHotStockDayAsync(newDay);
    }
}
