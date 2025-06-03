using HtmlAgilityPack;
using Infrastructure.Interfaces;
using SharedProject.Models;
using System.Globalization;

namespace Infrastructure.GetDataFromHtml;

public class HotStockParser : IHotStockParser
{
    public async Task<List<HotStockItemDto>> ParseHotStocks(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var hotStocks = new List<HotStockItemDto>();

        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'hot-row')]");
        if (rows == null) return hotStocks;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count < 9) continue;

            try
            {
                var stock = new HotStockItemDto
                {
                    Name = CleanText(cells[0].InnerText),

                    Price = ParseDecimal(cells[2].InnerText),

                    Change = ParseDecimal(cells[3].InnerText),

                    ChangePercent = ParseDecimal(cells[4].InnerText.Replace("(", "").Replace(")", "").Replace("%", "")),

                    Volume = ParseDecimal(cells[5].InnerText),

                    Turnover = ParseDecimal(cells[6].InnerText),

                    TurnoverMedian = ParseDecimal(cells[7].InnerText),

                    TurnoverDynamicsPercent = ParseDecimal(cells[8].InnerText.Replace("%", ""))
                };

                hotStocks.Add(stock);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd parsowania wiersza: {ex.Message}");
            }
        }

        return hotStocks;
    }

    private string CleanText(string text)
    {
        return System.Net.WebUtility.HtmlDecode(text).Trim();
    }

    private decimal ParseDecimal(string value)
    {
        var cleanValue = value.Replace(" ", "").Replace(",", ".");
        return decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0m;
    }
}
