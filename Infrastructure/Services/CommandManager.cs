using Infrastructure.Events;
using Infrastructure.Interfaces;
using SharedProject.Interfaces;
using SharedProject.Models;
using SharedProject.Views;
using System.Diagnostics;
using System.Windows.Input;

namespace Infrastructure.Services;

public class CommandManager(ITradeTrackerFacade facade)
{
    #region Tracking Commands
    public ICommand ToggleTrackingCommand => new DelegateCommand<ITrackable>(async item =>
    {
        if (item == null) return;

        var dialog = new ConfirmationDialog
        {
            DialogText = item.IsTracking
                ? "Czy na pewno wyłączyć śledzenie?"
                : "Czy na pewno włączyć śledzenie?"
        };

        dialog.ShowDialog();
        if (dialog.Result)
        {
            item.IsTracking = !item.IsTracking;

            switch (item)
            {
                case Transaction transaction:
                    await facade.TransactionManager.UpdateTransaction(transaction);
                    facade.EventAggregator.GetEvent<TransactionUpdatedEvent>().Publish(transaction);
                    break;

                case Event @event:
                    await facade.EventManager.UpdateEvent(@event);
                    facade.EventAggregator.GetEvent<EventUpdatedEvent>().Publish(@event);
                    break;
            }
        }
    });
    #endregion

    #region Navigation Commands
    public ICommand GoToRelatedItemsCommand => new DelegateCommand<ITrackable>(async item =>
    {
        if (item == null) return;

        switch (item)
        {
            case Transaction tx:
                var events = (await facade.EventManager.GetEventsForCompany(tx.CompanyID))
                    .Where(e => e.EntryDate >= tx.EntryDate)
                    .OrderByDescending(e => e.EntryDate);

                facade.ViewManager.NavigateTo("EventsOverviewView",
                    new NavigationParameters { { "events", events } });
                break;

            case Event ev:
                var transactions = (await facade.TransactionManager.GetTransactionsForCompany(ev.CompanyID))
                    .Where(t => t.EntryDate <= ev.EntryDate)
                    .OrderByDescending(t => t.EntryDate);

                facade.ViewManager.NavigateTo("TransactionsOverviewView",
                    new NavigationParameters { { "transactions", transactions } });
                break;
        }
    });
    #endregion

    #region Comment Commands
    public ICommand ToggleAddCommentCommand => new DelegateCommand<ICommentable>(item =>
    {
        if (item != null)
        {
            item.IsNewCommentBeingAdded = !item.IsNewCommentBeingAdded;
            item.NewCommentText = string.Empty;
        }
    });

    public ICommand ConfirmNewCommentCommand => new DelegateCommand<ICommentable>(async item =>
    {
        if (item == null || string.IsNullOrWhiteSpace(item.NewCommentText)) return;

        var comment = new Comment
        {
            EntryDate = DateTime.Now,
            CommentText = item.NewCommentText.Trim()
        };

        switch (item)
        {
            case Transaction tx:
                comment.TransactionID = tx.ID;
                break;
            case Event ev:
                comment.EventID = ev.ID;
                break;
        }

        await facade.CommentManager.AddComment(comment);
        item.Comments.Add(comment);
        item.IsNewCommentBeingAdded = false;
        item.NewCommentText = string.Empty;
    });

    public ICommand EditCommentCommand => new DelegateCommand<Comment>(comment =>
    {
        if (comment?.IsEditing == false)
        {
            comment.OldCommentText = comment.CommentText;
            comment.IsEditing = true;
        }
    });

    public ICommand ConfirmCommentChangesCommand => new DelegateCommand<Comment>(async comment =>
    {
        if (comment?.IsEditing == true)
        {
            comment.IsEditing = false;
            await facade.CommentManager.UpdateComment(comment);
        }
    });

    public ICommand DiscardCommentChangesCommand => new DelegateCommand<Comment>(comment =>
    {
        if (comment?.IsEditing == true)
        {
            comment.CommentText = comment.OldCommentText;
            comment.IsEditing = false;
        }
    });

    public ICommand DeleteCommentCommand => new DelegateCommand<object>(async param =>
    {
        if (param is not Tuple<Comment, ICommentable> tuple)
            return;

        var (comment, parentItem) = tuple;

        if (comment == null || parentItem == null)
            return;

        var dialog = new ConfirmationDialog
        {
            DialogText = "Czy na pewno chcesz usunąć komentarz?"
        };

        dialog.ShowDialog();

        if (dialog.Result)
        {
            await facade.CommentManager.DeleteComment(comment.ID);
            parentItem.Comments.Remove(comment);
        }
    });

    public ICommand ToggleCommentEditCommand => new DelegateCommand<Comment>(comment =>
    {
        if (comment != null)
        {
            if (!comment.IsEditing)
            {
                comment.OldCommentText = comment.CommentText;
            }
            comment.IsEditing = !comment.IsEditing;
        }
    });

    public ICommand CancelCommentEditCommand => new DelegateCommand<Comment>(comment =>
    {
        if (comment?.IsEditing == true)
        {
            comment.CommentText = comment.OldCommentText;
            comment.IsEditing = false;
        }
    });
    #endregion

    #region UI Commands
    public ICommand ToggleDetailsPanelCommand => new DelegateCommand<IDetailable>(item =>
    {
        if (item != null)
        {
            item.IsCommentBeingEdited = false;
            item.IsDetailsVisible = !item.IsDetailsVisible;
            item.IsNewCommentBeingAdded = false;
            item.NewCommentText = string.Empty;
        }
    });

    public ICommand OpenLinkCommand => new DelegateCommand<ILinkable>(item =>
    {
        if (!string.IsNullOrEmpty(item?.InformationLink))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = item.InformationLink,
                UseShellExecute = true
            });
        }
    });
    #endregion
}
