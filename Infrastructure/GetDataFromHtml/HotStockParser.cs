using HtmlAgilityPack;
using Infrastructure.Interfaces;
using SharedProject.Models;

namespace Infrastructure.GetDataFromHtml;

public class HotStockParser : IHotStockParser
{
    public List<HotStockItemDto> Parse(string html, string market)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'hot-row')]");
        var result = new List<HotStockItemDto>();

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

        return result;
    }
}