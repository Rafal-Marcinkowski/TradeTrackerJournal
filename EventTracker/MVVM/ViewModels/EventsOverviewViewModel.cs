using DataAccess.Data;
using Infrastructure.Events;
using SharedProject.Models;
using SharedProject.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace EventTracker.MVVM.ViewModels;

public class EventsOverviewViewModel : BindableBase, INavigationAware
{
    private readonly IEventData eventData;
    private readonly IRegionManager regionManager;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private readonly ICommentData CommentData;

    public EventsOverviewViewModel(IEventData eventData, IDailyDataProvider dailyDataProvider,
        IEventAggregator eventAggregator, ICommentData CommentData, IRegionManager regionManager)
    {
        this.regionManager = regionManager;
        this.CommentData = CommentData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;
        isCommentBeingEdited = false;
        isNewCommentBeingAdded = false;
        HasFinalComment = !string.IsNullOrEmpty(FinalComment);
        this.eventData = eventData;
        events = [];

        this.eventAggregator.GetEvent<DailyDataAddedEvent>().Subscribe(async dailyData =>
        {
            await OnDailyDataAdded(dailyData);
        });

        this.eventAggregator.GetEvent<EventUpdatedEvent>().Subscribe(async e =>
        {
            await OnEventUpdated(e);
        });
    }

    private async Task OnEventUpdated(Event e)
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var eventToUpdate = Events.FirstOrDefault(q => q.ID == e.ID);

            if (eventToUpdate != null)
            {
                eventToUpdate.EntryMedianTurnover = eventToUpdate.EntryMedianTurnover;
            }
        });
    }

    private ObservableCollection<Event> events;
    public ObservableCollection<Event> Events
    {
        get => events;
        set => SetProperty(ref events, value);
    }

    private async Task OnDailyDataAdded(DailyData dailyData)
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var e = Events.FirstOrDefault(q => q.ID == dailyData.EventID);

            e?.DailyDataCollection.Add(dailyData);
        });
    }

    private async Task GetDailyData(ObservableCollection<Event> events)
    {
        foreach (var e in events)
        {
            var dailyData = (await dailyDataProvider.GetDailyDataForEventAsync(e.ID)).OrderBy(q => q.Date);

            e.DailyDataCollection = new ObservableCollection<DailyData>(dailyData);
        }
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        foreach (var key in navigationContext.Parameters.Keys)
        {
            _ = key switch
            {
                "selectedCompany" => Task.Run(() => GetEventsForCompany((int)navigationContext.Parameters[key])),
                "op" => Task.Run(() => GetAllOpenEvents()),
                "lastx" => Task.Run(() => GetLastXEvents((int)navigationContext.Parameters[key])),
                "events" => Task.Run(() => GetEventsForTransaction((IEnumerable<Event>)navigationContext.Parameters[key])),
                _ => Task.CompletedTask
            };
        }
    }

    private async Task GetEventsForTransaction(IEnumerable<Event> events)
    {
        var eventsList = new ObservableCollection<Event>(events);
        await GetDailyData(eventsList);
        await GetAllComments(eventsList);
        Events = eventsList;
    }

    private async Task GetLastXEvents(int nrOfTransactionsToShow)
    {
        var events = await eventData.GetAllEventsAsync();
        events = events.OrderByDescending(q => q.EntryDate).Take(nrOfTransactionsToShow);
        var eventsList = new ObservableCollection<Event>(events);
        await GetDailyData(eventsList);
        await GetAllComments(eventsList);
        Events = eventsList;
    }

    private async Task GetAllComments(ObservableCollection<Event> events)
    {
        foreach (var e in events)
        {
            var comments = await CommentData.GetAllCommentsForEventAsync(e.ID);
            e.Comments = (new ObservableCollection<Comment>(comments));
        }
    }

    private async Task GetAllOpenEvents()
    {
        //var events = await eventData.GetAllEventsAsync();
        //events = events.OrderByDescending(q => q.EntryDate).Where(q => q.IsClosed == false);
        //await GetDailyData(new ObservableCollection<Event>(events));
        //await GetAllComments(new ObservableCollection<Event>(events));
        //Events = new ObservableCollection<Event>(events);
    }

    private async Task GetEventsForCompany(int selectedCompanyId)
    {
        var events = await eventData.GetAllEventsForCompany(selectedCompanyId);
        events = events.OrderByDescending(q => q.EntryDate);
        await GetDailyData(new ObservableCollection<Event>(events));
        await GetAllComments(new ObservableCollection<Event>(events));
        Events = new ObservableCollection<Event>(events);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public ICommand ToggleCommentsPanelCommand => new DelegateCommand<Event>(e =>
    {
        e.IsDetailsVisible = !e.IsDetailsVisible;
        e.IsNewCommentBeingAdded = false;
        IsCommentBeingEdited = false;
        NewCommentText = string.Empty;
    });

    public ICommand NavigateBackCommand => new DelegateCommand(() =>
    {
        var navigationJournal = regionManager.Regions.First().NavigationService.Journal;
        if (navigationJournal.CanGoBack)
        {
            navigationJournal.GoBack();
        }
    });

    private bool isNewCommentBeingAdded;
    public bool IsNewCommentBeingAdded
    {
        get => isNewCommentBeingAdded;
        set
        {
            SetProperty(ref isNewCommentBeingAdded, value);
        }
    }

    public string FinalComment { get; set; }

    private string newCommentText;
    public string NewCommentText
    {
        get => newCommentText;
        set
        {
            SetProperty(ref newCommentText, value);
        }
    }

    private bool isCommentBeingEdited;
    public bool IsCommentBeingEdited
    {
        get => isCommentBeingEdited;
        set
        {
            SetProperty(ref isCommentBeingEdited, value);
        }
    }

    private bool hasFinalComment;
    public bool HasFinalComment
    {
        get => hasFinalComment;
        set
        {
            SetProperty(ref hasFinalComment, value);
        }
    }

    private string editedCommentOldText;

    public ICommand ToggleEventTracker => new DelegateCommand<Event>(async (e) =>
    {
        if (e is not null)
        {
            var dialog = new ConfirmationDialog
            {
                DialogText = e.IsTracking ? "Czy na pewno wyłączyć śledzenie?" : "Czy na pewno włączyć śledzenie?"
            };

            dialog.ShowDialog();

            if (dialog.Result)
            {
                e.IsTracking = !e.IsTracking;
                await eventData.UpdateEventAsync(e);

                if (e.IsTracking)
                {
                    eventAggregator.GetEvent<EventAddedEvent>().Publish(e);
                }
            }
        }
    });

    public ICommand AddNewCommentCommand => new DelegateCommand<Event>(async (e) =>
    {
        e.IsNewCommentBeingAdded = !e.IsNewCommentBeingAdded;

        NewCommentText = string.Empty;
    });

    public ICommand ShowInfoCommand => new DelegateCommand<Event>(async (e) =>
    {
        if (!String.IsNullOrEmpty(e.InformationLink))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.InformationLink,
                UseShellExecute = true
            });
        }
    });

    public ICommand ConfirmCommentChangesCommand => new DelegateCommand<Comment>(async (comment) =>
    {
        if (comment.IsEditing)
        {
            comment.IsEditing = false;
            await CommentData.UpdateCommentAsync(comment);
        }
    });

    public ICommand DiscardCommentChangesCommand => new DelegateCommand<Comment>((comment) =>
    {
        if (comment.IsEditing)
        {
            comment.CommentText = editedCommentOldText;
            comment.IsEditing = false;
        }
    });

    public ICommand EditCommentCommand => new DelegateCommand<Comment>((comment) =>
    {
        if (!comment.IsEditing)
        {
            editedCommentOldText = comment.CommentText;
            comment.IsEditing = true;
        }
    });

    public ICommand ConfirmNewCommentCommand => new DelegateCommand<Event>(async (e) =>
    {
        if (!String.IsNullOrEmpty(NewCommentText))
        {
            Comment Comment = new()
            {
                TransactionID = null,
                EventID = e.ID,
                EntryDate = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                CommentText = NewCommentText
            };

            e.Comments.Add(Comment);
            await CommentData.InsertCommentAsync(Comment);
            e.IsNewCommentBeingAdded = !e.IsNewCommentBeingAdded;
            NewCommentText = string.Empty;
        }
    });

    public ICommand EditNewCommentCommand => new DelegateCommand<Comment>((comment) =>
    {

    });

    public ICommand DeleteCommentCommand => new DelegateCommand<Tuple<Comment, Event>>(async (parameters) =>
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = "Czy na pewno chcesz usunąć komentarz?"
        };
        dialog.ShowDialog();

        if (dialog.Result)
        {
            parameters.Item1.ID = await CommentData.GetCommentID(parameters.Item1.CommentText);
            parameters.Item2.Comments.Remove(parameters.Item1);
            await CommentData.DeleteCommentAsync(parameters.Item1.ID);
        }
    });
}
