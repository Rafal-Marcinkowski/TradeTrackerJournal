using SharedProject.Models;
using SharedProject.ViewModels;

namespace Infrastructure.Interfaces;
public interface IEventManager
{
    Task AddEvent(Event @event);
    Task<IEnumerable<Event>> GetAllEvents();
    Task<IEnumerable<Event>> GetEventsForCompany(int companyId);
    Task<bool> TryAddEvent(TransactionEventViewModel ev);
    Task UpdateEvent(Event @event);
}