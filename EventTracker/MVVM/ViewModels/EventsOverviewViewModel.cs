using Infrastructure.Events;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EventTracker.MVVM.ViewModels;

public class EventsOverviewViewModel : BindableBase, INavigationAware
{
    private readonly ITradeTrackerFacade _facade;
    private readonly TrackableDataLoader _eventLoader;

    public ObservableCollection<Event> Events { get; } = [];
    public CommandManager Commands { get; }

    public EventsOverviewViewModel(ITradeTrackerFacade facade)
    {
        _facade = facade;
        _eventLoader = new TrackableDataLoader(facade);
        Commands = new CommandManager(facade);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _facade.EventAggregator.GetEvent<DailyDataAddedEvent>()
            .Subscribe(d => _eventLoader.OnDailyDataAdded(d, Events));

        _facade.EventAggregator.GetEvent<EventUpdatedEvent>()
            .Subscribe(e => _eventLoader.OnTrackableUpdated(e, Events));
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        foreach (var key in navigationContext.Parameters.Keys)
        {
            await (key switch
            {
                "selectedCompany" => _eventLoader.GetItemsForCompany<Event>((int)navigationContext.Parameters[key]),
                "op" => _eventLoader.GetAllOpenItems<Event>(),
                "lastx" => _eventLoader.GetLastXItems<Event>((int)navigationContext.Parameters[key]),
                "events" => Task.FromResult(new ObservableCollection<Event>(
                    (IEnumerable<Event>)navigationContext.Parameters[key])),
                _ => Task.FromResult(new ObservableCollection<Event>())
            }).ContinueWith(t =>
            {
                Events.Clear();
                foreach (var item in t.Result)
                    Events.Add(item);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}
