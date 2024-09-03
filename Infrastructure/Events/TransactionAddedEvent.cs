using SharedModels.Models;

namespace Infrastructure.Events;

public class TransactionAddedEvent : PubSubEvent<Transaction>
{

}
