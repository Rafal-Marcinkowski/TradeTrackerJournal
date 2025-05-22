using HotStockTracker.MVVM.Models;
using System.Collections.ObjectModel;

namespace HotStockTracker.MVVM.ViewModels;

public class HotStockDayViewModel : BindableBase
{
    public DateTime Date { get; set; }
    public ObservableCollection<HotStockItem> TopGainers { get; set; }
    public ObservableCollection<HotStockItem> TopLosers { get; set; }
    public string Summary { get; set; }
    public bool IsCurrentDay { get; set; }
    public bool IsNotCurrentDay => !IsCurrentDay;
}
