using DataAccess.Data;
using Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewViewModel : BindableBase, INavigationAware
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private readonly ITransactionCommentData transactionCommentData;

    public TransactionsOverviewViewModel(ITransactionData transactionData, IDailyDataProvider dailyDataProvider, IEventAggregator eventAggregator, ITransactionCommentData transactionCommentData)
    {
        this.transactionCommentData = transactionCommentData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;
        isCommentBeingEdited = false;
        isNewCommentBeingAdded = false;
        HasFinalComment = !string.IsNullOrEmpty(FinalComment);
        this.transactionData = transactionData;
        transactions = new ObservableCollection<Transaction>();

        this.eventAggregator.GetEvent<DailyDataAddedEvent>().Subscribe(async dailyData =>
        {
            await OnDailyDataAdded(dailyData);
        });

        this.eventAggregator.GetEvent<TransactionUpdatedEvent>().Subscribe(async transaction =>
        {
            await OnTransactionUpdated(transaction);
        });
    }

    private async Task OnTransactionUpdated(Transaction transaction)
    {
        await App.Current.Dispatcher.InvokeAsync(async () =>
        {
            var transactionToUpdate = Transactions.FirstOrDefault(q => q.ID == transaction.ID);

            if (transactionToUpdate != null)
            {
                transactionToUpdate.EntryMedianTurnover = transaction.EntryMedianTurnover;
            }
        });
    }

    private ObservableCollection<Transaction> transactions;
    public ObservableCollection<Transaction> Transactions
    {
        get => transactions;
        set => SetProperty(ref transactions, value);
    }

    private async Task OnDailyDataAdded(DailyData dailyData)
    {
        await App.Current.Dispatcher.InvokeAsync(async () =>
        {
            var transaction = Transactions.FirstOrDefault(q => q.ID == dailyData.TransactionID);

            if (transaction != null)
            {
                transaction.DailyDataCollection.Add(dailyData);
            }
        });
    }

    private async Task GetDailyData(ObservableCollection<Transaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            var dailyData = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);
            transaction.DailyDataCollection = new ObservableCollection<DailyData>(dailyData);
        }
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        foreach (var key in navigationContext.Parameters.Keys)
        {
            _ = key switch
            {
                "selectedCompany" => Task.Run(() => GetTransactionsForCompany((int)navigationContext.Parameters[key])),
                "op" => Task.Run(() => GetAllOpenTransactions()),
                "lastx" => Task.Run(() => GetLastXTransactions((int)navigationContext.Parameters[key])),
                _ => Task.CompletedTask
            };
        }
    }

    private async Task GetLastXTransactions(int nrOfTransactionsToShow)
    {
        var transactions = await transactionData.GetAllTransactionsAsync();
        transactions = transactions.OrderByDescending(q => q.EntryDate).Take(nrOfTransactionsToShow);
        var transactionsList = new ObservableCollection<Transaction>(transactions);
        await GetDailyData(transactionsList);
        await GetAllComments(transactionsList);
        Transactions = transactionsList;
    }

    private async Task GetAllComments(ObservableCollection<Transaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            var comments = await transactionCommentData.GetAllCommentsForTransactionAsync(transaction.ID);
            transaction.Comments = (new ObservableCollection<TransactionComment>(comments));
        }
    }

    private async Task GetAllOpenTransactions()
    {
        var transactions = await transactionData.GetAllTransactionsAsync();
        transactions = transactions.OrderByDescending(q => q.EntryDate).Where(q => q.IsClosed == false);
        await GetDailyData(new ObservableCollection<Transaction>(transactions));
        await GetAllComments(new ObservableCollection<Transaction>(transactions));
        Transactions = new ObservableCollection<Transaction>(transactions);
    }

    private async Task GetTransactionsForCompany(int selectedCompanyId)
    {
        var transactions = await transactionData.GetAllTransactionsForCompany(selectedCompanyId);
        transactions = transactions.OrderByDescending(q => q.EntryDate);
        await GetDailyData(new ObservableCollection<Transaction>(transactions));
        await GetAllComments(new ObservableCollection<Transaction>(transactions));
        Transactions = new ObservableCollection<Transaction>(transactions);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public ICommand ToggleCommentsPanelCommand => new DelegateCommand<Transaction>(transaction =>
    {
        transaction.IsDetailsVisible = !transaction.IsDetailsVisible;
        transaction.IsNewCommentBeingAdded = false;

        IsCommentBeingEdited = false;
        NewCommentText = string.Empty;
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

    public ICommand ToggleTransactionTracker => new DelegateCommand<Transaction>(async (transaction) =>
    {
        if (transaction.IsTracking)
        {
            var dialog = new ConfirmationDialog()
            {
                DialogText = "Czy na pewno wyłączyć śledzenie?"
            };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                transaction.IsTracking = false;
                await transactionData.UpdateTransactionAsync(transaction);
            }
        }
        else
        {
            var dialog = new ConfirmationDialog()
            {
                DialogText = "Czy na pewno włączyć śledzenie?"
            };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                transaction.IsTracking = true;
                await transactionData.UpdateTransactionAsync(transaction);
            }
        }
    });



    public ICommand AddNewCommentCommand => new DelegateCommand<Transaction>(async (transaction) =>
    {
        transaction.IsNewCommentBeingAdded = !transaction.IsNewCommentBeingAdded;

        NewCommentText = string.Empty;
    });

    public ICommand ShowInfoCommand => new DelegateCommand<Transaction>(async (transaction) =>
    {
        if (!String.IsNullOrEmpty(transaction.InformationLink))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = transaction.InformationLink,
                UseShellExecute = true
            });
        }
    });

    public ICommand ConfirmCommentChangesCommand => new DelegateCommand<TransactionComment>(async (comment) =>
    {
        if (comment.IsEditing)
        {
            comment.IsEditing = false;
            await transactionCommentData.UpdateCommentAsync(comment);
        }
    });

    public ICommand DiscardCommentChangesCommand => new DelegateCommand<TransactionComment>((comment) =>
    {
        if (comment.IsEditing)
        {
            comment.CommentText = editedCommentOldText;
            comment.IsEditing = false;
        }
    });

    public ICommand EditCommentCommand => new DelegateCommand<TransactionComment>((comment) =>
    {
        if (!comment.IsEditing)
        {
            editedCommentOldText = comment.CommentText;
            comment.IsEditing = true;
        }
    });

    public ICommand ConfirmNewCommentCommand => new DelegateCommand<Transaction>(async (transaction) =>
    {
        if (!String.IsNullOrEmpty(NewCommentText))
        {
            TransactionComment transactionComment = new()
            {
                TransactionID = transaction.ID,
                EntryDate = DateTime.Now,
                CommentText = NewCommentText
            };
            transaction.Comments.Add(transactionComment);
            await transactionCommentData.InsertCommentAsync(transactionComment);
            transaction.IsNewCommentBeingAdded = !transaction.IsNewCommentBeingAdded;
            NewCommentText = string.Empty;
        }
    });

    public ICommand EditNewCommentCommand => new DelegateCommand<TransactionComment>((comment) =>
    {

    });

    public ICommand DeleteCommentCommand => new DelegateCommand<Tuple<TransactionComment, Transaction>>(async (parameters) =>
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = "Czy na pewno chcesz usunąć komentarz?"
        };
        dialog.ShowDialog();

        if (dialog.Result)
        {
            parameters.Item1.ID = await transactionCommentData.GetCommentID(parameters.Item1.CommentText);
            parameters.Item2.Comments.Remove(parameters.Item1);
            await transactionCommentData.DeleteCommentAsync(parameters.Item1.ID);
        }
    });
}
