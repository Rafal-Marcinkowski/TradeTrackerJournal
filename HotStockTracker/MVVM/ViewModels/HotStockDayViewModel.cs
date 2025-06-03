using GalaSoft.MvvmLight.CommandWpf;
using HotStockTracker.Services;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockDayViewModel : BindableBase
{
    public HotStockDayViewModel(HotStockApiClient hotStockApiClient)
    {
        this.hotStockApiClient = hotStockApiClient;
    }

    public HotStockDayViewModel(HotStockDayDto dto, HotStockApiClient hotStockApiClient)
    {
        this.hotStockApiClient = hotStockApiClient;
        Date = dto.Date;
        Summary = dto.Summary;
        IsSummaryExpanded = dto.IsSummaryExpanded;
        HotStockItems = new ObservableCollection<HotStockItemViewModel>(
            dto.Items.Select(i => new HotStockItemViewModel(i)));
    }

    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public ObservableCollection<HotStockItemViewModel> HotStockItems { get; set; }

    public ObservableCollection<HotStockItemViewModel> TopGainers =>
        new(HotStockItems.OrderByDescending(i => i.ChangePercent).Take(10));

    public ObservableCollection<HotStockItemViewModel> TopLosers =>
        new(HotStockItems.OrderBy(i => i.ChangePercent).Take(10));

    private string _summary = string.Empty;
    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
            {
                EditButtonText = value ? "Zapisz" : "Edytuj";
                RaisePropertyChanged(nameof(IsNotEditMode));
                RaisePropertyChanged(nameof(EditButtonText));
            }
        }
    }

    public bool IsNotEditMode => !IsEditMode;
    public string EditButtonText { get; private set; } = "Edytuj";

    private bool _isSummaryExpanded = true;
    private readonly HotStockApiClient hotStockApiClient;

    public bool IsSummaryExpanded
    {
        get => _isSummaryExpanded;
        set => SetProperty(ref _isSummaryExpanded, value);
    }

    public ICommand ToggleEditCommand => new RelayCommand(async () =>
        {
            IsEditMode = !IsEditMode;
            if (!IsEditMode)
            {
                await SaveSummaryAsync();
            }
        });

    private async Task SaveSummaryAsync()
    {
        try
        {
            var dto = ToDto();
            await hotStockApiClient.UpdateDaySummaryAsync(dto);
            Debug.WriteLine("Summary saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save summary: {ex.Message}");
        }
    }

    public HotStockDayDto ToDto() => new()
    {
        Date = Date.Date,
        Summary = Summary,
        IsSummaryExpanded = IsSummaryExpanded,
        Items = [.. HotStockItems.Select(i => i.ToDto())]
    };
}
