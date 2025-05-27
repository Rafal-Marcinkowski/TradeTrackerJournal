using Infrastructure.DownloadHtmlData;
using Infrastructure.GetDataFromHtml;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockOverviewViewModel : BindableBase
{

    public ObservableCollection<HotStockDayViewModel> DayItems { get; set; } = [];

    public HotStockOverviewViewModel()
    {
        _ = DownloadData();
    }

    public async Task DownloadData()
    {
        var html = await DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/dynamika-obrotow/4,2");
        File.WriteAllText("C:\\Users\\rafal\\Desktop\\Pogromcy\\plik", html);
        HotStockParser parser = new();
        var records = await parser.ParseHotStocks(html);
        var topGainsers = records.Take(10).ToList().OrderByDescending(q => q.ChangePercent);
        var topLosers = records.TakeLast(10).ToList().OrderBy(q => q.ChangePercent);

        var dayitem = new HotStockDayViewModel
        {
            TopGainers = new ObservableCollection<HotStockItem>(topGainsers),
            TopLosers = new ObservableCollection<HotStockItem>(topLosers)
        };

        DayItems.Add(dayitem);
        DayItems.Add(dayitem);
        DayItems.Add(dayitem);
        DayItems.Add(dayitem);
        DayItems.Add(dayitem);
    }
}
