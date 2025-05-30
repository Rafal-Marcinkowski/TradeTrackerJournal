using HotStockTracker.Services;
using System.Collections.ObjectModel;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockOverviewViewModel : BindableBase
{
    private readonly HotStockTrackerFacade facade;

    public ObservableCollection<HotStockDayViewModel> DayItems { get; set; } = [];

    public HotStockOverviewViewModel(HotStockTrackerFacade facade)
    {
        this.facade = facade;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await facade.HotStockDayManager.AddNewDayIfMissingAsync();

        var latestDays = await facade.HotStockDayManager.GetLatestDaysAsync();
        DayItems.Clear();
        foreach (var day in latestDays)
        {
            DayItems.Add(day);
        }
    }
}
