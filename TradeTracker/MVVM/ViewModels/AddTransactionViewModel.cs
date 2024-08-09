using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace TradeTracker.MVVM.ViewModels;

public class AddTransactionViewModel : BindableBase
{
    public AddTransactionViewModel()
    {

    }

    public ICommand AddTransactionCommand => new DelegateCommand(() =>
    {

    });
}
