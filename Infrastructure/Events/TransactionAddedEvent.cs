using SharedProject.Models;

namespace Infrastructure.Events;

public class TransactionAddedEvent : PubSubEvent<Transaction>;
