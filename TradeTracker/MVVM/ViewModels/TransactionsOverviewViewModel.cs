using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewViewModel : BindableBase
{
    public TransactionsOverviewViewModel()
    {
        IsCommentBeingEdited = false;
        HasFinalComment = !string.IsNullOrEmpty(FinalComment);
    }

    public ICommand ToggleCommentsPanelCommand => new DelegateCommand<Transaction>(transaction =>
    {
        transaction.IsDetailsVisible = !transaction.IsDetailsVisible;
    });

    private string newCommentText;
    private bool isCommentBeingEdited;
    private bool hasFinalComment;

    public string FinalComment { get; set; }

    public string NewCommentText
    {
        get => newCommentText;
        set
        {
            newCommentText = value;
            RaisePropertyChanged();
        }
    }

    public bool IsCommentBeingEdited
    {
        get => isCommentBeingEdited;
        set
        {
            isCommentBeingEdited = value;
            RaisePropertyChanged();
        }
    }

    public bool HasFinalComment
    {
        get => hasFinalComment;
        set
        {
            hasFinalComment = value;
            RaisePropertyChanged();
        }
    }

    public ICommand AddCommentCommand => new DelegateCommand(() =>
    {
        IsCommentBeingEdited = true;
        NewCommentText = string.Empty;
    });

    public ICommand ConfirmCommentCommand => new DelegateCommand(() =>
    {
        if (IsCommentBeingEdited)
        {
            FinalComment = NewCommentText;
            HasFinalComment = true;
            IsCommentBeingEdited = false;
        }
    });

    public ICommand EditCommentCommand => new DelegateCommand(() =>
    {
        IsCommentBeingEdited = true;
        NewCommentText = FinalComment;
    });

    public ICommand DeleteCommentCommand => new DelegateCommand(() =>
    {
        FinalComment = string.Empty;
        HasFinalComment = false;
        IsCommentBeingEdited = false;
    });

    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>
    {

        new Transaction
    {
        CompanyName = "Columbus",
        InitialDescription = "bumcykcyk",
        FinalComment = "halohalo",
        EntryDate = DateTime.Now,
        EntryPrice = 100,
        EntryMedianVolume = 500,
        IsClosed = true,
        CloseDate = DateTime.Now,
        DayOpenPrice = new List<decimal>{ 89, 23, 54},
        EndOfDayPrice = new List<decimal> { 131, 170, 14 },
        DayPriceChange = new List<decimal> { 2, -83, 65 },
        DayVolume = new List<decimal> { 520, 54222, 1342 },
        DayVolumeChange = new List<decimal> { 4, 7300, -32 },
        DayMin = new List<decimal> { 98, 101, 132 },
        DayMax = new List<decimal> { 105, 324, 98}
    },
         new Transaction
    {
        CompanyName = "Polwax",
        EntryDate = DateTime.Now,
        EntryPrice = 79,
        IsClosed= false,
        EntryMedianVolume = 123678,
        DayOpenPrice = new List<decimal>{ 89, 23, 54},
        EndOfDayPrice = new List<decimal> { 131, 170, 14 },
        DayPriceChange = new List<decimal> { -8, 17, -123 },
        DayVolume = new List<decimal> { 11234, 2132, 765 },
        DayVolumeChange = new List<decimal> { 1082, 708, 1232 },
        DayMin = new List<decimal> { 67, 71, 98 },
        DayMax = new List<decimal> { 105, 121, -131 }
    }
    };


}

