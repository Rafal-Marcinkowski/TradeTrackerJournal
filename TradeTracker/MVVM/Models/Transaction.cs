public class Transaction
{
    public string CompanyName { get; set; }
    public DateTime EntryDate { get; set; }
    public decimal EntryPrice { get; set; }
    public int MedianVolume { get; set; }

    public List<decimal> OpenPrice { get; set; }
    public List<decimal> EndOfDayPrice { get; set; }
    public List<decimal> DayPriceChange { get; set; }
    public List<decimal> DayVolume { get; set; }
    public List<decimal> DayVolumeChange { get; set; }
    public List<decimal> DayMin { get; set; }
    public List<decimal> DayMax { get; set; }
}

