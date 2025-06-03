using HotStockTracker.MVVM.ViewModels;
using HotStockTracker.MVVM.Views;
using HotStockTracker.Services;

namespace HotStockTracker.Module;

public class HotStockTrackerModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<HotStockOverviewViewModel>();
        containerRegistry.RegisterSingleton<HotStockDayManager>();
        containerRegistry.RegisterSingleton<HotStockTrackerFacade>();
        containerRegistry.Register<HotStockApiClient>();

        containerRegistry.RegisterForNavigation<HotStockOverviewView>();
    }
}
