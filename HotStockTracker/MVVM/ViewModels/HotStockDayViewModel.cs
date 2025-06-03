using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockDayViewModel : BindableBase
{
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
                RaisePropertyChanged(nameof(IsNotEditMode));
                EditButtonText = value ? "Zapisz" : "Edytuj";
            }
        }
    }

    public bool IsNotEditMode => !IsEditMode;
    public string EditButtonText { get; private set; } = "Edytuj";

    private bool _isSummaryExpanded = true;
    public bool IsSummaryExpanded
    {
        get => _isSummaryExpanded;
        set => SetProperty(ref _isSummaryExpanded, value);
    }

    public ICommand ToggleEditCommand { get; }

    public HotStockDayViewModel()
    {
        ToggleEditCommand = new RelayCommand(() => IsEditMode = !IsEditMode);
    }

    public HotStockDayViewModel(HotStockDayDto dto)
    {
        Date = dto.Date;
        Summary = dto.Summary;
        IsSummaryExpanded = dto.IsSummaryExpanded;
        HotStockItems = new ObservableCollection<HotStockItemViewModel>(
            dto.Items.Select(i => new HotStockItemViewModel(i)));
    }

    public HotStockDayDto ToDto() => new()
    {
        Date = Date,
        Summary = Summary,
        IsSummaryExpanded = IsSummaryExpanded,
        Items = [.. HotStockItems.Select(i => i.ToDto())]
    };
}
