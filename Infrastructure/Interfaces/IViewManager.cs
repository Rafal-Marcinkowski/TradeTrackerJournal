namespace Infrastructure.Interfaces;

public interface IViewManager
{
    void NavigateTo(string viewName, bool clearRegion = true);
    void NavigateTo(string viewName, NavigationParameters parameters, bool clearRegion = true);
}