using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class EventData(ISQLDataAccess dBAccess) : IEventData
{
    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await dBAccess.LoadDataAsync<Event, dynamic>("GetAllEvents", new { });
    }

    public async Task<Event> GetEventAsync(int id)
    {
        var events = await dBAccess.LoadDataAsync<Event, dynamic>("GetEvent", new { ID = id });
        return events.FirstOrDefault();
    }

    public async Task<IEnumerable<Event>> GetAllEventsForCompany(int companyId)
    {
        var parameters = new { CompanyID = companyId };
        var events = await dBAccess.LoadDataAsync<Event, dynamic>("GetAllEventsForCompany", parameters);
        return events;
    }

    public async Task InsertEventAsync(Event e)
    {
        var parameters = new
        {
            e.CompanyID,
            e.CompanyName,
            e.EntryDate,
            e.EntryPrice,
            e.InitialDescription,
            e.InformationLink,
            e.IsTracking,
            e.EntryMedianTurnover,
            e.Description
        };

        await dBAccess.SaveDataAsync("InsertEvent", parameters);
    }

    public async Task UpdateEventAsync(Event e)
    {
        var parameters = new
        {
            e.ID,
            e.CompanyID,
            e.CompanyName,
            e.EntryDate,
            e.EntryPrice,
            e.InitialDescription,
            e.InformationLink,
            e.IsTracking,
            e.EntryMedianTurnover,
            e.Description
        };

        await dBAccess.SaveDataAsync("UpdateEvent", parameters);
    }

    public async Task<int> GetID(Event e)
    {
        var events = await GetAllEventsForCompany(e.CompanyID);
        return events.FirstOrDefault(q => q.CompanyID == e.CompanyID && q.EntryDate == e.EntryDate && q.EntryPrice == e.EntryPrice)?.ID ?? -1;
    }

    public async Task DeleteEventAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteEvent", new { ID = id });
    }
}
