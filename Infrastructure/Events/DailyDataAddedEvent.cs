using Prism.Events;
using SharedModels.Models;

namespace Infrastructure.Events;

public class DailyDataAddedEvent : PubSubEvent<DailyData>
{
}
