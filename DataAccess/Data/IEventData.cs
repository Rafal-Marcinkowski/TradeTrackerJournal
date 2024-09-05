using SharedProject.Models;

namespace DataAccess.Data;
public interface IEventData
{
    Task DeleteEventAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Event>> GetAllEventsForCompany(int companyId);
    Task<Event> GetEventAsync(int id);
    Task<int> GetID(Event e);
    Task InsertEventAsync(Event e);
    Task UpdateEventAsync(Event e);
}