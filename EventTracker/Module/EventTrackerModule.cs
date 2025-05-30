using EventTracker.MVVM.ViewModels;
using EventTracker.MVVM.Views;

namespace EventTracker.Module;

public class EventTrackerModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<AddEventViewModel>();

        containerRegistry.RegisterForNavigation<AddEventView>();
        containerRegistry.RegisterForNavigation<EventsMainMenuView>();
        containerRegistry.RegisterForNavigation<EventsOverviewMenuView>();
        containerRegistry.RegisterForNavigation<EventsOverviewView>();
    }
}
