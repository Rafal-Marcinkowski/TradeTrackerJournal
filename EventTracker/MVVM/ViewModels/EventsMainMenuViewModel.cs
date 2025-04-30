using EventTracker.MVVM.Views;
using Infrastructure.Services;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsMainMenuViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand NavigateToAddEventCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(AddEventView)));

    public ICommand NavigateToEventsOverviewMenuCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(EventsOverviewMenuView)));
}
