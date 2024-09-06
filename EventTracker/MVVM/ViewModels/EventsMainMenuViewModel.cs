using EventTracker.MVVM.Views;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsMainMenuViewModel(IRegionManager regionManager) : BindableBase
{

    public ICommand NavigateToAddEventCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddEventView));
    });

    public ICommand NavigateToEventsOverviewMenuCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(EventsOverviewMenuView));
    });
}
