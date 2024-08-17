using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TradeTracker.MVVM.Models;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewViewModel : BindableBase, INavigationAware
{
    public TransactionsOverviewViewModel()
    {
        isCommentBeingEdited = false;
        isNewCommentBeingAdded = false;
        HasFinalComment = !string.IsNullOrEmpty(FinalComment);
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.ContainsKey("TransactionId"))
        {
            var transactionId = navigationContext.Parameters.GetValue<int>("TransactionId");
            // Wykonaj operacje z otrzymanym transactionId
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        // Zdecyduj, czy ten ViewModel powinien być celem nawigacji, czy powinien zostać utworzony nowy.
        return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        // Możesz wykonać jakieś operacje przed opuszczeniem widoku, jeśli to konieczne.
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

    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>
    {

        new Transaction
    {
        CompanyName = "Columbus",
        InitialDescription = "Komentarz otwierający Columbus",
        ClosingDescription = "halohalo",
        EntryDate = DateTime.Now,
        EntryPrice = 100,
        EntryMedianVolume = 500,
        IsClosed = true,
        CloseDate = DateTime.Now,
        DayOpenPrice = new List<double>{ 89, 23, 54},
        EndOfDayPrice = new List<double> { 131, 170, 14 },
        DayPriceChange = new List<double> { 2, -83, 65 },
        DayVolume = new List<double> { 520, 54222, 1342 },
        DayVolumeChange = new List<double> { 4, 7300, -32 },
        DayMin = new List<double> { 98, 101, 132 },
        DayMax = new List<double> { 105, 324, 98}
    },
         new Transaction
    {
        CompanyName = "Polwax",
        InitialDescription = "Komentarz otwierający Polwax",
        EntryDate = DateTime.Now,
        EntryPrice = 79,
        IsClosed= false,
        EntryMedianVolume = 123678,
        DayOpenPrice = new List<double>{ 89, 23, 54},
        EndOfDayPrice = new List<double> { 131, 170, 14 },
        DayPriceChange = new List<double> { -8, 17, -123 },
        DayVolume = new List<double> { 11234, 2132, 765 },
        DayVolumeChange = new List<double> { 1082, 708, 1232 },
        DayMin = new List<double> { 67, 71, 98 },
        DayMax = new List<double> { 105, 121, -131 },
        Comments = new ObservableCollection<TransactionComment>
        {
            new TransactionComment()
        {
            EntryDate = DateTime.Now,
            CommentText = "pierwszy komentarz, jakiś tam"
        }  ,
        new TransactionComment()
        {
            EntryDate= DateTime.Now,
            CommentText  = "drugi komentatrez dla wyswietlen"
        }
        }
    }
    };
}

