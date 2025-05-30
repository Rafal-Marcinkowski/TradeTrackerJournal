namespace HotStockTracker.Services;

public class HotStockTrackerFacade(HotStockDayManager hotStockDayManager)
{
    public HotStockDayManager HotStockDayManager { get; } = hotStockDayManager;
}
