using GalaSoft.MvvmLight.CommandWpf;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockDayViewModel : BindableBase
{
    public DateTime Date { get; set; }
    public ObservableCollection<HotStockItem> TopGainers { get; set; }
    public ObservableCollection<HotStockItem> TopLosers { get; set; }
    public string Summary { get; set; }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public bool IsNotEditMode => !IsEditMode;

    public string EditButtonText => IsEditMode ? "Zapisz" : "Edytuj";

    private bool _isSummaryExpanded = true;
    public bool IsSummaryExpanded
    {
        get => _isSummaryExpanded;
        set => SetProperty(ref _isSummaryExpanded, value);
    }

    public ICommand ToggleEditCommand => new RelayCommand(() =>
    {
        IsEditMode = !IsEditMode;
        if (!IsEditMode)
        {
        }
    });
}
