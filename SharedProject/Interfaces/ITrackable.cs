using SharedProject.Models;
using System.Collections.ObjectModel;

namespace SharedProject.Interfaces;

public interface ITrackable
{
    int ID { get; set; }
    int CompanyID { get; set; }
    string CompanyName { get; set; }
    DateTime EntryDate { get; set; }
    public decimal EntryPrice { get; set; }
    bool IsTracking { get; set; }
    ObservableCollection<DailyData> DailyDataCollection { get; set; }
    int EntryMedianTurnover { get; set; }
}
