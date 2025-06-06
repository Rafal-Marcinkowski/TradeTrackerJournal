using GalaSoft.MvvmLight.CommandWpf;
using HotStockTracker.MVVM.ViewModels;
using HotStockTracker.Services;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class HotStockDayViewModel : BindableBase
{
    private readonly HotStockApiClient hotStockApiClient;

    private string summary;
    private bool isSummaryExpanded;
    private bool isEditMode;

    public HotStockDayViewModel(HotStockDayDto dto, HotStockApiClient hotStockApiClient)
    {
        this.hotStockApiClient = hotStockApiClient;
        Date = dto.Date;
        Summary = dto.Summary;
        IsSummaryExpanded = dto.IsSummaryExpanded;
        HotStockItems = new ObservableCollection<HotStockItemViewModel>(
            dto.HotStockItems.Select(i => new HotStockItemViewModel(i)));

        HotStockItems.CollectionChanged += (_, _) =>
        {
            RaisePropertyChanged(nameof(TopGainers));
            RaisePropertyChanged(nameof(TopLosers));
        };
    }

    public string OpeningComment { get; set; }
    public bool IsOpeningCommentEditMode { get; set; }
    public bool IsSummaryEditMode { get; set; }
    public string OpeningCommentEditButtonText => IsOpeningCommentEditMode ? "Zapisz" : "Edytuj";
    public string SummaryEditButtonText => IsSummaryEditMode ? "Zapisz" : "Edytuj";

    public ICommand ToggleOpeningCommentEditCommand { get; }
    public ICommand ToggleSummaryEditCommand { get; }

    public DateTime Date { get; }

    public ObservableCollection<HotStockItemViewModel> HotStockItems { get; }

    public ObservableCollection<HotStockItemViewModel> TopGainers =>
        new(HotStockItems.Where(i => i.Change.StartsWith("+")));

    public ObservableCollection<HotStockItemViewModel> TopLosers =>
        new(HotStockItems.Where(i => i.Change.StartsWith("-")));

    public string Summary
    {
        get => summary;
        set => SetProperty(ref summary, value);
    }

    public bool IsSummaryExpanded
    {
        get => isSummaryExpanded;
        set => SetProperty(ref isSummaryExpanded, value);
    }

    public bool IsEditMode
    {
        get => isEditMode;
        set
        {
            if (SetProperty(ref isEditMode, value))
            {
                RaisePropertyChanged(nameof(IsNotEditMode));
                RaisePropertyChanged(nameof(EditButtonText));
            }
        }
    }

    public bool IsNotEditMode => !IsEditMode;

    public string EditButtonText => IsEditMode ? "Zapisz" : "Edytuj";

    public ICommand ToggleEditCommand => new RelayCommand(async () =>
    {
        IsEditMode = !IsEditMode;
        if (!IsEditMode)
            await SaveSummaryAsync();
    });

    private async Task SaveSummaryAsync()
    {
        var dto = ToDto();
        await hotStockApiClient.UpdateDaySummaryAsync(dto);
    }

    public HotStockDayDto ToDto() => new()
    {
        Date = Date.Date,
        Summary = Summary,
        IsSummaryExpanded = IsSummaryExpanded,
        HotStockItems = [.. HotStockItems.Select(i => i.ToDto())]
    };
}
