using HtmlAgilityPack;
using Infrastructure.Interfaces;
using SharedProject.Models;
using System.Globalization;

namespace Infrastructure.GetDataFromHtml;

public class HotStockParser : IHotStockParser
{
    public List<HotStockItemDto> Parse(string html, string market, DateTime date = default)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new List<HotStockItemDto>();

        if (date == default)
        {
            var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'hot-row')]");
            if (rows == null)
                return result;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 11)
                    continue;

                var item = new HotStockItemDto
                {
                    Name = HtmlEntity.DeEntitize(cells[0].InnerText.Trim()),
                    Market = market,
                    Price = cells[2].InnerText.Trim(),
                    Change = cells[3].InnerText.Trim(),
                    ChangePercent = cells[4].InnerText.Trim(),
                    ReferencePrice = cells[5].InnerText.Trim(),
                    OpenPrice = cells[6].InnerText.Trim(),
                    MinPrice = cells[7].InnerText.Trim(),
                    MaxPrice = cells[8].InnerText.Trim(),
                    Volume = cells[9].InnerText.Trim(),
                    Turnover = cells[10].InnerText.Trim()
                };

                result.Add(item);
            }
        }
        else
        {
            var rows = doc.DocumentNode.SelectNodes("//tr[@data-blink-soid]");
            if (rows == null)
                return result;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 11)
                    continue;

                var timeCell = cells[1].SelectSingleNode(".//span[@data-push-type='time']");
                if (timeCell == null)
                    continue;

                var rawTime = timeCell.InnerText.Trim();

                if (!TryParsePolishDate(rawTime, out DateTime parsedDate))
                    continue;

                if (parsedDate.Date != date.Date)
                    continue;

                var item = new HotStockItemDto
                {
                    Name = HtmlEntity.DeEntitize(cells[0].InnerText.Trim()),
                    Market = market,
                    Price = cells[2].InnerText.Trim(),
                    Change = cells[3].InnerText.Trim(),
                    ChangePercent = cells[4].InnerText.Trim(),
                    ReferencePrice = cells[5].InnerText.Trim(),
                    OpenPrice = cells[6].InnerText.Trim(),
                    MinPrice = cells[7].InnerText.Trim(),
                    MaxPrice = cells[8].InnerText.Trim(),
                    Volume = cells[9].InnerText.Trim(),
                    Turnover = cells[10].InnerText.Trim()
                };

                result.Add(item);
            }
        }

        return result;
    }
    private static bool TryParsePolishDate(string input, out DateTime date)
    {
        var culture = new CultureInfo("pl-PL");
        return DateTime.TryParseExact(input, "d MMM HH:mm", culture, DateTimeStyles.None, out date);
    }
}