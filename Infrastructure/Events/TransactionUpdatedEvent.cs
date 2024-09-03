using SharedModels.Models;

namespace Infrastructure.Events;

public class TransactionUpdatedEvent : PubSubEvent<Transaction>
{
}
