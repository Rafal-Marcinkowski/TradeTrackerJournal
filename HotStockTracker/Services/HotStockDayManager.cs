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
            return [.. days.OrderByDescending(d => d.Date).Take(count)];
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

            var existingDays = await apiClient.GetHotStockDaysAsync();
            if (existingDays.Any(d => d.Date.Date == targetDate))
                return false;

            var gpwHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/akcje_gpw,4,2");
            var ncHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/newconnect,4,2");

            await Task.WhenAll(gpwHtmlTask, ncHtmlTask);

            var gpwStocks = htmlParser.Parse(gpwHtmlTask.Result, "GPW");
            var ncStocks = htmlParser.Parse(ncHtmlTask.Result, "NewConnect");

            var allStocks = gpwStocks.Concat(ncStocks).ToList();

            var newDay = await CreateHotStockDayDto(targetDate, allStocks);

            var result = await apiClient.AddHotStockDayAsync(newDay);
            Debug.WriteLine($"Day added with {newDay.HotStockItems.Count} stocks.");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in AddNewDayIfMissingAsync: {ex}");
            return false;
        }
    }

    private async Task<HotStockDayDto> CreateHotStockDayDto(DateTime date, List<HotStockItemDto> stocks)
    {
        var items = await GetTopMovers(stocks);

        return new HotStockDayDto
        {
            Date = date,
            Summary = string.Empty,
            OpeningComment = string.Empty,
            IsSummaryExpanded = true,
            HotStockItems = items
        };
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
