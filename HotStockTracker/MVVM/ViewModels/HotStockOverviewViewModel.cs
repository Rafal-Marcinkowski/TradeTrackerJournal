using HotStockTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockOverviewViewModel : BindableBase
{
    private readonly HotStockDayManager _dayManager;
    private bool _isLoading;

    public ObservableCollection<HotStockDayViewModel> Days { get; set; }
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public HotStockOverviewViewModel(HotStockDayManager dayManager)
    {
        _dayManager = dayManager;
        Days = [];
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        try
        {
            Debug.WriteLine("Starting data loading...");

            var wasAdded = await _dayManager.AddNewDayIfMissingAsync();
            Debug.WriteLine($"AddNewDayIfMissingAsync result: {wasAdded}");

            var latestDays = await _dayManager.GetLatestDaysAsync();
            Debug.WriteLine($"Retrieved {latestDays.Count} days");

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Days.Clear();
                foreach (var dayDto in latestDays)
                {
                    var dayVm = new HotStockDayViewModel(dayDto);
                    Debug.WriteLine($"Adding day with {dayVm.HotStockItems.Count} items");
                    Days.Add(dayVm);
                }
            });
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
