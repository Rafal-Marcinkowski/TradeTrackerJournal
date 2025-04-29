namespace Infrastructure.Services;

public class ViewManager(IRegionManager regionManager)
{
    private const string MainRegionName = "MainRegion";

    public void NavigateTo(string viewName, bool clearRegion = true)
    {
        NavigateTo(viewName, null, clearRegion);
    }

    public void NavigateTo(string viewName, NavigationParameters parameters, bool clearRegion = true)
    {
        if (clearRegion && regionManager.Regions.ContainsRegionWithName(MainRegionName))
        {
            var region = regionManager.Regions[MainRegionName];
            region.RemoveAll();
        }

        regionManager.RequestNavigate(MainRegionName, viewName, parameters);
    }
}
