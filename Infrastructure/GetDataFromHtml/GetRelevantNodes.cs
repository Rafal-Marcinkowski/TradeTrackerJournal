﻿using HtmlAgilityPack;
using SharedProject.Models;
using System.Globalization;

namespace Infrastructure.GetDataFromHtml;

public class GetRelevantNodes
{
    public async static Task<List<DataRecord>> PrepareRecords(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rows = doc.DocumentNode.SelectNodes("//table[@class='qTableFull']/tr");

        var records = new List<DataRecord>();

        if (rows is not null)
        {
            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("td").Select(td => td.InnerText.Trim()).ToList();

                var record = new DataRecord
                {
                    Date = DateTime.ParseExact(cells[0], "dd.MM.yyyy", CultureInfo.InvariantCulture).Date,
                    Open = Math.Round(decimal.Parse(cells[1].Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture), 2),
                    Max = Math.Round(decimal.Parse(cells[2].Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture), 2),
                    Min = Math.Round(decimal.Parse(cells[3].Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture), 2),
                    Close = Math.Round(decimal.Parse(cells[4].Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture), 2),
                    Volume = Math.Round(decimal.Parse(cells[5].Replace(" ", ""), CultureInfo.InvariantCulture), 2),
                    Turnover = Math.Round(decimal.Parse(cells[6].Replace(" ", ""), CultureInfo.InvariantCulture), 2)
                };

                records.Add(record);
            }
        }
        return records;
    }
}
