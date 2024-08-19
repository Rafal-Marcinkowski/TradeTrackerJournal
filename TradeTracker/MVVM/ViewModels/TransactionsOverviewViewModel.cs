using DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SharedModels.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewViewModel : BindableBase, INavigationAware
{
    private readonly ITransactionData transactionData;
    public TransactionsOverviewViewModel(ITransactionData transactionData)
    {
        isCommentBeingEdited = false;
        isNewCommentBeingAdded = false;
        HasFinalComment = !string.IsNullOrEmpty(FinalComment);
        this.transactionData = transactionData;
        transactions = new ObservableCollection<Transaction>();
    }

    private ObservableCollection<Transaction> transactions;
    public ObservableCollection<Transaction> Transactions
    {
        get => transactions;
        set => SetProperty(ref transactions, value);
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.ContainsKey("selectedCompany"))
        {
            int companyId = (int)navigationContext.Parameters["selectedCompany"];
            Task.Run(() => GetTransactions(companyId));
        }
    }

    private async Task GetTransactions(int selectedCompanyId)
    {
        var transactions = await transactionData.GetAllTransactionsForCompany(selectedCompanyId);
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

    public ICommand AddNewCommentCommand => new DelegateCommand<Transaction>((transaction) =>
    {
        transaction.IsNewCommentBeingAdded = !transaction.IsNewCommentBeingAdded;
        NewCommentText = string.Empty;
    });

    public ICommand ConfirmCommentChangesCommand => new DelegateCommand<TransactionComment>((comment) =>
    {
        if (comment.IsEditing)
        {
            comment.IsEditing = false;
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

    public ICommand ConfirmNewCommentCommand => new DelegateCommand<Transaction>((transaction) =>
    {
        if (!String.IsNullOrEmpty(NewCommentText))
        {
            TransactionComment transactionComment = new();
            transactionComment.EntryDate = DateTime.Now;
            transactionComment.CommentText = NewCommentText;
            transaction.Comments.Add(transactionComment);
            NewCommentText = string.Empty;
            transaction.IsNewCommentBeingAdded = !transaction.IsNewCommentBeingAdded;
        }
    });

    public ICommand EditNewCommentCommand => new DelegateCommand<TransactionComment>((comment) =>
    {

    });

    public ICommand DeleteCommentCommand => new DelegateCommand<Tuple<TransactionComment, Transaction>>((parameters) =>
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = "Czy na pewno chcesz usunąć komentarz?"
        };
        dialog.ShowDialog();

        if (dialog.Result)
        {
            parameters.Item2.Comments.Remove(parameters.Item1);
        }
    });
}

