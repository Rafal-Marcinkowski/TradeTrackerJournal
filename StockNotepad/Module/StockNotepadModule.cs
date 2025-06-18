using StockNotepad.MVVM.ViewModels;
using StockNotepad.MVVM.Views;

namespace StockNotepad.Module;

public class StockNotepadModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<StockNotepadOverviewViewModel>();
        containerRegistry.Register<StockNotepadSelectCompanyViewModel>();

        containerRegistry.RegisterForNavigation<StockNotepadOverviewView>();
        containerRegistry.RegisterForNavigation<StockNotepadSelectCompanyView>();
    }
}
