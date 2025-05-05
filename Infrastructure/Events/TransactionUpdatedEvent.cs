using SharedProject.Models;

namespace Infrastructure.Events;

public class TransactionUpdatedEvent : PubSubEvent<Transaction>;
