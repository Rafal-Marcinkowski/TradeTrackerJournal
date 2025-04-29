using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface IEventManager
{
    Task AddEvent(Event @event);
    Task<IEnumerable<Event>> GetEventsForCompany(int companyId);
    Task UpdateEvent(Event @event);
}