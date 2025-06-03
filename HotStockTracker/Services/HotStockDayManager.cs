using Infrastructure.DownloadHtmlData;
using Infrastructure.Interfaces;
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
            var today = DateTime.Today;

            var startTime = today.AddHours(17).AddMinutes(6);
            var endTime = today.AddDays(1).AddHours(8).AddMinutes(59);

            Debug.WriteLine($"Current time: {now:HH:mm}");
            Debug.WriteLine($"Time window: {startTime:HH:mm} - {endTime:HH:mm}");

            if (now < startTime || now > endTime)
            {
                Debug.WriteLine("Outside allowed time window (17:06–8:59). Skipping.");
                return false;
            }


            var existingDays = await apiClient.GetHotStockDaysAsync();
            Debug.WriteLine($"Existing days count: {existingDays.Count}");

            if (existingDays.Any(d => d.Date.Date == today.Date))
            {
                Debug.WriteLine("Day already exists, skipping");
                return false;
            }

            Debug.WriteLine("Creating new day data...");
            var newDay = await FetchAndCreateDayDataAsync(today);

            Debug.WriteLine("Sending new day to API...");
            var result = await apiClient.AddHotStockDayAsync(newDay);
            Debug.WriteLine($"Day added with items: {result?.Items.Count}");

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
