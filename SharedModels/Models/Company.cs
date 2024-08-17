using System.Collections.ObjectModel;

namespace SharedModels.Models;

public class Company
{

    public int ID { get; set; }

    public string CompanyName { get; set; }

    public int TransactionCount { get; set; }

    public ObservableCollection<Transaction> Transactions { get; set; }
}
