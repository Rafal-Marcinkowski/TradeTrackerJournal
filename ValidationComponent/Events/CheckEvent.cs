using DataAccess.Data;
using SharedProject.Models;

namespace ValidationComponent.Events;

public class CheckEvent(IEventData eventData)
{
    public async Task<bool> IsExisting(Event e)
    {
        var events = await eventData.GetAllEventsForCompany(e.CompanyID);
        var comparer = new EventComparer();
        return events.Any(q => comparer.Equals(e, q));
    }
}
