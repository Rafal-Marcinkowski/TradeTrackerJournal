using Infrastructure.Events;
using Infrastructure.Interfaces;
using SharedProject.Models;
using SharedProject.Views;
using System.Diagnostics;
using System.Windows.Input;

namespace Infrastructure.Services;

public class CommandManager(ITradeTrackerFacade facade)
{
    #region Transaction Commands
    public ICommand ToggleTransactionTrackerCommand => new DelegateCommand<Transaction>(async tx =>
    {
        if (tx == null) return;

        var dialog = new ConfirmationDialog
        {
            DialogText = tx.IsTracking
                ? "Czy na pewno wyłączyć śledzenie?"
                : "Czy na pewno włączyć śledzenie?"
        };

        dialog.ShowDialog();
        if (dialog.Result)
        {
            tx.IsTracking = !tx.IsTracking;
            await facade.TransactionManager.UpdateTransaction(tx);

            if (tx.IsTracking)
            {
                facade.EventAggregator.GetEvent<TransactionAddedEvent>().Publish(tx);
            }
        }
    });

    public ICommand GoToEventCommand => new DelegateCommand<int?>(async txId =>
    {
        if (txId == null) return;

        var tx = await facade.TransactionManager.GetTransaction(txId.Value);
        var events = (await facade.EventManager.GetEventsForCompany(tx.CompanyID))
            .Where(e => e.EntryDate >= tx.EntryDate)
            .OrderByDescending(e => e.EntryDate);

        facade.ViewManager.NavigateTo("EventsOverviewView",
            new NavigationParameters { { "events", events } });
    });
    #endregion

    #region Comment Commands
    public ICommand ToggleAddCommentCommand => new DelegateCommand<Transaction>(tx =>
    {
        if (tx != null)
        {
            tx.IsNewCommentBeingAdded = !tx.IsNewCommentBeingAdded;
        }
    });

    public ICommand ConfirmNewCommentCommand =>
        new DelegateCommand<Transaction>(async tx =>
        {
            if (tx == null || string.IsNullOrWhiteSpace(tx.NewCommentText)) return;

            var comment = new Comment
            {
                TransactionID = tx.ID,
                EntryDate = DateTime.Now,
                CommentText = tx.NewCommentText.Trim()
            };

            await facade.CommentManager.AddComment(comment);
            tx.Comments.Add(comment);
            tx.IsNewCommentBeingAdded = false;
            tx.NewCommentText = string.Empty;
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

    public ICommand DeleteCommentCommand => new DelegateCommand<Tuple<Comment, Transaction>>(async param =>
    {
        if (param?.Item1 == null || param.Item2 == null) return;

        var dialog = new ConfirmationDialog
        {
            DialogText = "Czy na pewno chcesz usunąć komentarz?"
        };

        dialog.ShowDialog();
        if (dialog.Result)
        {
            await facade.CommentManager.DeleteComment(param.Item1.ID);
            param.Item2.Comments.Remove(param.Item1);
        }
    });
    #endregion

    #region UI Commands
    public ICommand ToggleCommentsPanelCommand => new DelegateCommand<Transaction>(transaction =>
    {
        if (transaction != null)
        {
            transaction.IsCommentBeingEdited = false;
            transaction.IsDetailsVisible = !transaction.IsDetailsVisible;
            transaction.IsNewCommentBeingAdded = false;
            transaction.NewCommentText = string.Empty;
        }
    });

    public ICommand OpenLinkCommand => new DelegateCommand<Transaction>(tx =>
    {
        if (!string.IsNullOrEmpty(tx?.InformationLink))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = tx.InformationLink,
                UseShellExecute = true
            });
        }
    });

    #endregion
}
