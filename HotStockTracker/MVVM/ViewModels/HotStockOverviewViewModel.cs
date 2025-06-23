using HotStockTracker.Services;
using Infrastructure.DownloadHtmlData;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockOverviewViewModel : BindableBase
{
    private readonly HotStockTrackerFacade facade;
    private readonly TTJApiClient hotStockApiClient;
    private bool _isLoading;

    public ObservableCollection<HotStockDayViewModel> Days { get; set; }
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public HotStockOverviewViewModel(HotStockTrackerFacade facade, TTJApiClient hotStockApiClient)
    {
        this.facade = facade;
        this.hotStockApiClient = hotStockApiClient;
        Days = [];
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        try
        {
            var gpwHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/akcje_gpw,4,2");
            var ncHtmlTask = DownloadPageSource.DownloadHtmlFromUrlAsync("https://www.biznesradar.pl/gielda/newconnect,4,2");

            await Task.WhenAll(gpwHtmlTask, ncHtmlTask);

            File.WriteAllText("C:\\Users\\rafal\\Desktop\\Pogromcy\\gpw", gpwHtmlTask.Result);
            Debug.WriteLine("Starting data loading...");

            var wasAdded = await facade.HotStockDayManager.CheckAndUpdateDaysAsync();
            Debug.WriteLine($"AddNewDayIfMissingAsync result: {wasAdded}");

            var latestDays = await facade.HotStockDayManager.GetLatestDaysAsync();
            Debug.WriteLine($"Retrieved {latestDays.Count} days");

            Days.Clear();
            foreach (var dayDto in latestDays)
            {
                var dayVm = new HotStockDayViewModel(dayDto, hotStockApiClient);
                Debug.WriteLine($"Adding day with {dayVm.HotStockItems.Count} items");
                Days.Add(dayVm);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in LoadDataAsync: {ex}");
        }
        finally
        {
            IsLoading = false;
            Debug.WriteLine("Data loading completed");
        }
    }
}
