using SharedProject.Models;

namespace Infrastructure.Events;

public class EventUpdatedEvent : PubSubEvent<Event>;
