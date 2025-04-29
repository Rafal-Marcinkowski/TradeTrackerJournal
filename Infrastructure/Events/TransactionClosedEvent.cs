using SharedProject.Models;

namespace Infrastructure.Events;

public class TransactionClosedEvent : PubSubEvent<Transaction>;
