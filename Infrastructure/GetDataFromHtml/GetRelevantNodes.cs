using HtmlAgilityPack;
using Infrastructure.DownloadHtmlData;
using System.Globalization;

namespace Infrastructure.GetDataFromHtml;

public class GetRelevantNodes
{
    public async static Task<Dictionary<string, object>> PrepareData(string companyCode)
    {
        string html = await DownloadPageSource.DownloadHtmlAsync(companyCode);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var dateNode = doc.DocumentNode.SelectSingleNode("//time[@class='q_ch_date']");
        var date = DateTime.Parse(dateNode.GetAttributeValue("datetime", string.Empty), null, DateTimeStyles.RoundtripKind);

        var priceNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_close']/span");
        var price = Convert.ToDecimal(priceNode.InnerText, CultureInfo.InvariantCulture);

        var openNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_open']/span");
        var openPrice = Convert.ToDecimal(openNode.InnerText, CultureInfo.InvariantCulture);

        var minNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_min']/span");
        var minPrice = Convert.ToDecimal(minNode.InnerText, CultureInfo.InvariantCulture);

        var maxNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_max']/span");
        var maxPrice = Convert.ToDecimal(maxNode.InnerText, CultureInfo.InvariantCulture);

        var volumeNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_vol']/span");
        var volume = Convert.ToDecimal(volumeNode.InnerText.Replace(" ", ""), CultureInfo.InvariantCulture);

        var turnoverNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_mc']/span");
        var turnover = Convert.ToDecimal(turnoverNode.InnerText.Replace(" ", ""), CultureInfo.InvariantCulture);

        var transactionsNode = doc.DocumentNode.SelectSingleNode("//td[@id='pr_t_trnr']/span");
        var transactions = Convert.ToInt32(transactionsNode.InnerText, CultureInfo.InvariantCulture);

        var dataDictionary = new Dictionary<string, object>
    {
        { "Date", date },
        { "Price", price },
        { "OpenPrice", openPrice },
        { "MinPrice", minPrice },
        { "MaxPrice", maxPrice },
        { "Volume", volume },
        { "Turnover", turnover },
        { "Transactions", transactions }
    };

        return dataDictionary;
    }
}
