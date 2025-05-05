using SharedProject.Models;

namespace Infrastructure.Events;

public class DailyDataAddedEvent : PubSubEvent<DailyData>;
