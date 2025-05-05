using DataAccess.Data;
using Infrastructure.Events;
using Infrastructure.Interfaces;
using Serilog;
using SharedProject.Models;
using SharedProject.ViewModels;
using SharedProject.Views;
using System.Globalization;
using ValidationComponent.Events;

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

    public async Task<IEnumerable<Event>> GetAllEvents()
    {
        return await eventData.GetAllEventsAsync();
    }

    public async Task<bool> TryAddEvent(TransactionEventViewModel ev)
    {
        Event e = await FillNewEventProperties(ev);

        if (!await ValidateNewEventProperties(e)) return false;

        try
        {
            e.CompanyID = await companyData.GetCompanyID(e.CompanyName);

            if (!await CheckEventValidity(e)) return false;

            if (await ConfirmEvent(e)) return true;
        }

        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
        return false;
    }

    private async Task<bool> ConfirmEvent(Event e)
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = $"Czy dodać zdarzenie? \n" +
                  $"{e.CompanyName}\n" +
                  $"{e.EntryDate}\n" +
                  $"Początkowy kurs: {e.EntryPrice}\n"
        };

        dialog.ShowDialog();

        if (dialog.Result)
        {
            await eventData.InsertEventAsync(e);
            var company = await companyData.GetCompanyAsync(e.CompanyID);
            company.EventCount++;
            await companyData.UpdateCompanyAsync(company.ID, e.CompanyName, company.TransactionCount, company.EventCount);
            await eventData.UpdateEventAsync(e);
            e.ID = await eventData.GetID(e);
            eventAggregator.GetEvent<EventAddedEvent>().Publish(e);
            return true;
        }

        return false;
    }

    private async Task<bool> CheckEventValidity(Event e)
    {
        if (await new CheckEvent(eventData).IsExisting(e))
        {
            var errorDialog = new ErrorDialog()
            {
                DialogText = $"Zdarzenie już istnieje w bazie danych!"
            };

            errorDialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateNewEventProperties(Event e)
    {
        var validator = new AddEventValidator();
        var results = validator.Validate(e);

        if (!results.IsValid)
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));

            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };

            dialog.ShowDialog();
            return false;
        }

        return true;
    }

    private async Task<Event> FillNewEventProperties(TransactionEventViewModel ev)
    {
        return new Event
        {
            CompanyName = ev.SelectedCompanyName,
            EntryDate = DateTimeManager.ParseEntryDate(ev.EntryDate),
            EntryPrice = decimal.TryParse(
             ev.EntryPrice.Replace(" ", "").Replace(",", "."),
             NumberStyles.Any,
             CultureInfo.InvariantCulture,
             out var entryPrice)
             ? entryPrice
             : 0,
            InformationLink = ev.InformationLink.Trim(),
            InitialDescription = ev.InitialDescription.Trim(),
            Description = ev.Description.Trim(),
        };
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
