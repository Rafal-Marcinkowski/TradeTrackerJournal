using Infrastructure.DownloadHtmlData;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using SharedProject.Models;
using System.Diagnostics;

namespace HotStockTracker.Services;

public class HotStockDayManager(HotStockApiClient apiClient, IHotStockParser htmlParser)
{
    public async Task<List<HotStockDayDto>> GetLatestDaysAsync(int count = 5)
    {
        try
        {
            var days = await apiClient.GetHotStockDaysAsync();
            Debug.WriteLine($"Retrieved {days.Count} days from API with");

            return [.. days
                .OrderByDescending(d => d.Date)
                .Take(count)];
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetLatestDaysAsync: {ex}");
            return [];
        }
    }

    public async Task<bool> AddNewDayIfMissingAsync()
    {
        try
        {
            var now = DateTime.Now;

            if (!DateTimeManager.IsWithinTradingTimeWindow(now, out var targetDate))
            {
                Debug.WriteLine("Outside allowed time window (17:06–8:59). Skipping.");
                return false;
            }

            Debug.WriteLine($"Using date: {targetDate:yyyy-MM-dd}");

            var existingDays = await apiClient.GetHotStockDaysAsync();
            if (existingDays.Any(d => d.Date.Date == targetDate))
            {
                Debug.WriteLine($"Day {targetDate:yyyy-MM-dd} already exists, skipping");
                return false;
            }

            Debug.WriteLine($"Creating new day data for {targetDate:yyyy-MM-dd}...");
            var newDay = await FetchAndCreateDayDataAsync(targetDate);

            var result = await apiClient.AddHotStockDayAsync(newDay);
            Debug.WriteLine($"Day added with {newDay.Items.Count}");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in AddNewDayIfMissingAsync: {ex}");
            return false;
        }
    }

    private async Task<HotStockDayDto> FetchAndCreateDayDataAsync(DateTime date)
    {
        var html = await DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/dynamika-obrotow/4,2");
        var records = await htmlParser.ParseHotStocks(html);

        var topGainers = records.OrderByDescending(r => r.ChangePercent).Take(10);
        var topLosers = records.OrderBy(r => r.ChangePercent).Take(10);

        var selected = topGainers.Concat(topLosers).ToList();

        return new HotStockDayDto
        {
            Date = date,
            Summary = string.Empty,
            IsSummaryExpanded = true,
            Items = selected
        };
    }
}
