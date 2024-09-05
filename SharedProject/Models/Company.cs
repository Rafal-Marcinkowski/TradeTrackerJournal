using System.Collections.ObjectModel;

namespace SharedProject.Models;

public class Company : BindableBase
{
    public Company()
    {
        transactionCount = 0;
        eventCount = 0;
        Transactions = [];
        Events = [];
    }

    public int ID { get; set; }

    public string CompanyName { get; set; }

    private int transactionCount;
    public int TransactionCount
    {
        get => transactionCount;
        set => SetProperty(ref transactionCount, value);
    }

    private int eventCount;
    public int EventCount
    {
        get => eventCount;
        set => SetProperty(ref eventCount, value);
    }

    public ObservableCollection<Transaction> Transactions { get; set; }
    public ObservableCollection<Transaction> Events { get; set; }
}
