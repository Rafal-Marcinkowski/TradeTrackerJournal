namespace SessionOpening.Module;

public class SessionOpeningModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<SessionOpeningViewModel>();
        containerRegistry.RegisterForNavigation<SessionOpeningView>();
    }
}
