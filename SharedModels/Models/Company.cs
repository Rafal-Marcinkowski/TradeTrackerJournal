using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace SharedModels.Models;

public class Company : BindableBase
{
    public Company()
    {
        transactionCount = 0;
        Transactions = new ObservableCollection<Transaction>();
    }

    public int ID { get; set; }

    public string CompanyName { get; set; }

    private int transactionCount;
    public int TransactionCount
    {
        get => transactionCount;
        set => SetProperty(ref transactionCount, value);
    }

    public ObservableCollection<Transaction> Transactions { get; set; }
}
