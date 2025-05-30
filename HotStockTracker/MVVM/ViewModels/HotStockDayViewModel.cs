using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockDayViewModel : BindableBase
{
    public DateTime Date { get; set; }

    public ICollection<HotStockItemDto> HotStockItems { get; set; } = [];

    public ObservableCollection<HotStockItemDto> TopGainers { get; set; } = [];
    public ObservableCollection<HotStockItemDto> TopLosers { get; set; } = [];

    private string summary = string.Empty;
    public string Summary
    {
        get => summary;
        set => SetProperty(ref summary, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
                RaisePropertyChanged(nameof(IsNotEditMode));
        }
    }

    public bool IsNotEditMode => !IsEditMode;

    private string _editButtonText = "Edytuj";
    public string EditButtonText
    {
        get => _editButtonText;
        set => SetProperty(ref _editButtonText, value);
    }

    private bool _isSummaryExpanded = true;
    public bool IsSummaryExpanded
    {
        get => _isSummaryExpanded;
        set => SetProperty(ref _isSummaryExpanded, value);
    }

    public ICommand ToggleEditCommand => new RelayCommand(() =>
    {
        IsEditMode = !IsEditMode;
        EditButtonText = IsEditMode ? "Zapisz" : "Edytuj";
    });
}
