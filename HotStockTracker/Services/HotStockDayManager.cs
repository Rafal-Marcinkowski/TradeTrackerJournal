using Infrastructure.DownloadHtmlData;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using SharedProject.Models;
using System.Diagnostics;

namespace HotStockTracker.Services;

public class HotStockDayManager(TTJApiClient apiClient, IHotStockParser htmlParser)
{
    public async Task<List<HotStockDayDto>> GetLatestDaysAsync(int count = 10)
    {
        try
        {
            var days = await apiClient.GetHotStockDaysAsync();
            return [.. days.OrderBy(d => d.Date).Take(count)];
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetLatestDaysAsync: {ex}");
            return [];
        }
    }

    public async Task<bool> CheckAndUpdateDaysAsync()
    {
        try
        {
            var now = DateTime.Now;
            DateTimeManager.ShouldCheckForData(now, out var today, out var previousWorkDay);

            var todayExists = await EnsureDayExistsAsync(today);
            if (!todayExists)
            {
                Debug.WriteLine($"Failed to ensure today ({today:yyyy-MM-dd}) exists");
                return false;
            }

            if (DateTimeManager.ShouldUpdateToday(now))
            {
                var todayUpdated = await CheckAndUpdateSingleDayAsync(today);
                if (!todayUpdated)
                {
                    Debug.WriteLine($"Failed to update today ({today:yyyy-MM-dd})");
                    return false;
                }
            }

            if (previousWorkDay != DateTime.MinValue)
            {
                var previousDayUpdated = await CheckAndUpdateSingleDayAsync(previousWorkDay);
                if (!previousDayUpdated)
                {
                    Debug.WriteLine($"Failed to update previous work day ({previousWorkDay:yyyy-MM-dd})");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in CheckAndUpdateDaysAsync: {ex}");
            return false;
        }
    }

    private async Task<bool> CheckAndUpdateSingleDayAsync(DateTime date)
    {
        var existingDays = await apiClient.GetHotStockDaysAsync();
        var existingDay = existingDays.FirstOrDefault(d => d.Date.Date == date.Date);

        if (existingDay == null)
        {
            return await AddNewDayWithDataAsync(date);
        }

        if (existingDay.HotStockItems == null || existingDay.HotStockItems.Count == 0)
        {
            return await UpdateDayWithDataAsync(existingDay);
        }

        return true;
    }

    private async Task<bool> EnsureDayExistsAsync(DateTime date)
    {
        var existingDays = await apiClient.GetHotStockDaysAsync();
        if (existingDays.Any(d => d.Date.Date == date.Date))
            return true;

        var emptyDay = new HotStockDayDto
        {
            Date = date,
            Summary = string.Empty,
            OpeningComment = string.Empty,
            IsSummaryExpanded = true,
            HotStockItems = []
        };

        return await apiClient.AddHotStockDayAsync(emptyDay);
    }

    private async Task<bool> UpdateDayWithDataAsync(HotStockDayDto existingDay)
    {
        var gpwHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/akcje_gpw,4,2");
        var ncHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/newconnect,4,2");

        await Task.WhenAll(gpwHtmlTask, ncHtmlTask);

        var gpwStocks = htmlParser.Parse(gpwHtmlTask.Result, "GPW", existingDay.Date.Date);
        var ncStocks = htmlParser.Parse(ncHtmlTask.Result, "NewConnect", existingDay.Date.Date);

        var allStocks = gpwStocks.Concat(ncStocks).ToList();
        existingDay.HotStockItems = await GetTopMovers(allStocks);

        return await apiClient.UpdateHotStockDayAsync(existingDay);
    }

    private async Task<bool> AddNewDayWithDataAsync(DateTime date)
    {
        var gpwHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/akcje_gpw,4,2");
        var ncHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/newconnect,4,2");

        await Task.WhenAll(gpwHtmlTask, ncHtmlTask);

        var gpwStocks = htmlParser.Parse(gpwHtmlTask.Result, "GPW");
        var ncStocks = htmlParser.Parse(ncHtmlTask.Result, "NewConnect");

        var allStocks = gpwStocks.Concat(ncStocks).ToList();
        var newDay = new HotStockDayDto
        {
            Date = date,
            Summary = string.Empty,
            OpeningComment = string.Empty,
            IsSummaryExpanded = true,
            HotStockItems = await GetTopMovers(allStocks)
        };

        return await apiClient.AddHotStockDayAsync(newDay);
    }

    private async Task<List<HotStockItemDto>> GetTopMovers(List<HotStockItemDto> stocks)
    {
        decimal ParseChange(string changeStr)
        {
            if (string.IsNullOrWhiteSpace(changeStr))
                return 0m;

            var cleaned = changeStr
                .Replace("(", "")
                .Replace(")", "")
                .Replace("%", "")
                .Replace("+", "")
                .Replace(",", ".")
                .Trim();

            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val))
                return val;

            return 0m;
        }

        bool IsValidTurnover(string turnoverStr)
        {
            return decimal.TryParse(
                turnoverStr.Replace(" ", "").Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var val
            ) && val >= 10000m;
        }

        var validStocks = stocks.Where(s => IsValidTurnover(s.Turnover)).ToList();

        var gainers = validStocks
            .Where(s => s.ChangePercent.Contains("+"))
            .OrderByDescending(s => ParseChange(s.ChangePercent))
            .Take(10);

        var losers = validStocks
            .Where(s => s.ChangePercent.Contains("-"))
            .OrderBy(s => ParseChange(s.ChangePercent))
            .Take(10);

        return [.. gainers, .. losers];
    }
}
