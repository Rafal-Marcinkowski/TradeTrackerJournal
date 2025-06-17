using GalaSoft.MvvmLight.CommandWpf;
using HotStockTracker.MVVM.ViewModels;
using HotStockTracker.Services;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class HotStockDayViewModel : BindableBase
{
    public HotStockDayViewModel(HotStockDayDto dto, TTJApiClient hotStockApiClient)
    {
        this.hotStockApiClient = hotStockApiClient;
        Date = dto.Date;
        Summary = dto.Summary;
        OpeningComment = dto.OpeningComment;
        IsSummaryExpanded = dto.IsSummaryExpanded;
        HotStockItems = new ObservableCollection<HotStockItemViewModel>(
            dto.HotStockItems.Select(i => new HotStockItemViewModel(i)));

        HotStockItems.CollectionChanged += (_, _) =>
        {
            RaisePropertyChanged(nameof(TopGainers));
            RaisePropertyChanged(nameof(TopLosers));
        };
    }

    public ObservableCollection<HotStockItemViewModel> HotStockItems { get; }

    public ObservableCollection<HotStockItemViewModel> TopGainers =>
        new(HotStockItems.Where(i => i.Change.StartsWith("+")));

    public ObservableCollection<HotStockItemViewModel> TopLosers =>
        new(HotStockItems.Where(i => i.Change.StartsWith("-")));

    private readonly TTJApiClient hotStockApiClient;

    private string openingComment;
    public string OpeningComment
    {
        get => openingComment;
        set => SetProperty(ref openingComment, value);
    }

    private bool isOpeningCommentEditMode = false;
    public bool IsOpeningCommentEditMode
    {
        get => isOpeningCommentEditMode;
        set
        {
            if (SetProperty(ref isOpeningCommentEditMode, value))
            {
                RaisePropertyChanged(nameof(OpeningCommentEditButtonText));
            }
        }
    }

    private bool isSummaryEditMode = false;
    public bool IsSummaryEditMode
    {
        get => isSummaryEditMode;
        set
        {
            if (SetProperty(ref isSummaryEditMode, value))
            {
                RaisePropertyChanged(nameof(SummaryEditButtonText));
            }
        }
    }

    private string summary;
    public string Summary
    {
        get => summary;
        set => SetProperty(ref summary, value);
    }

    private bool isSummaryExpanded;
    public bool IsSummaryExpanded
    {
        get => isSummaryExpanded;
        set => SetProperty(ref isSummaryExpanded, value);
    }

    public DateTime Date { get; }
    public string OpeningCommentEditButtonText => IsOpeningCommentEditMode ? "Zapisz" : "Edytuj";
    public string SummaryEditButtonText => IsSummaryEditMode ? "Zapisz" : "Edytuj";

    public ICommand ToggleOpeningCommentEditCommand => new RelayCommand(async () =>
    {
        IsOpeningCommentEditMode = !IsOpeningCommentEditMode;
        if (!IsOpeningCommentEditMode)
            await UpdateDayAsync();
    });

    public ICommand ToggleSummaryEditCommand => new RelayCommand(async () =>
    {
        IsSummaryEditMode = !IsSummaryEditMode;
        if (!IsSummaryEditMode)
            await UpdateDayAsync();
    });

    private async Task UpdateDayAsync()
    {
        var dto = ToDto();
        await hotStockApiClient.UpdateHotStockDayAsync(dto);
    }

    public HotStockDayDto ToDto() => new()
    {
        Date = Date.Date,
        Summary = Summary,
        OpeningComment = OpeningComment,
        IsSummaryExpanded = IsSummaryExpanded,
        HotStockItems = [.. HotStockItems.Select(i => i.ToDto())]
    };
}
