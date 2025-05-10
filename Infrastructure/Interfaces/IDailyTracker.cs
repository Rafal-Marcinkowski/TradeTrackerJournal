using SharedProject.Interfaces;

namespace Infrastructure.Interfaces;

public interface IDailyTracker
{
    Task StartTracker(ITrackable trackable = null);
}