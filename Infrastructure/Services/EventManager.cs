using DataAccess.Data;
using Infrastructure.Events;
using Infrastructure.Interfaces;
using SharedProject.Models;

namespace Infrastructure.Services;

public class EventManager(IEventData eventData, ICompanyData companyData, IEventAggregator eventAggregator) : IEventManager
{
    public async Task AddEvent(Event @event)
    {
        @event.CompanyID = await companyData.GetCompanyID(@event.CompanyName);
        await eventData.InsertEventAsync(@event);

        var company = await companyData.GetCompanyAsync(@event.CompanyID);
        company.EventCount++;
        await companyData.UpdateCompanyAsync(company.ID, @event.CompanyName,
            company.TransactionCount, company.EventCount);

        @event.ID = await eventData.GetID(@event);
        eventAggregator.GetEvent<EventAddedEvent>().Publish(@event);
    }

    public async Task<IEnumerable<Event>> GetEventsForCompany(int companyId)
    {
        return await eventData.GetAllEventsForCompany(companyId);
    }

    public async Task UpdateEvent(Event @event)
    {
        await eventData.UpdateEventAsync(@event);
    }
}
