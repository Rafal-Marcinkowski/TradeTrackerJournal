namespace TradeTracker.MVVM.Models;

public class OpenPosition
{
    public string CompanyName { get; set; }  // Nazwa spółki
    public DateTime Date { get; set; }  // Data transakcji
    public string Duration { get; set; }  // Czas trwania transakcji (np. "5 dni")
    public decimal EntryPrice { get; set; }  // Cena wejścia
    public int PositionSize { get; set; }  // Wielkość pozycji (np. liczba akcji)
    public int NumberOfShares { get; set; }  // Ilość akcji
    public decimal? AvgSellPrice { get; set; }  // Średnia cena sprzedaży, może być null jeśli nie ustawiona

    public OpenPosition(string companyName, DateTime date, string duration, decimal entryPrice, int positionSize, int numberOfShares, decimal? avgSellPrice)
    {
        CompanyName = companyName;
        Date = date;
        Duration = duration;
        EntryPrice = entryPrice;
        PositionSize = positionSize;
        NumberOfShares = numberOfShares;
        AvgSellPrice = avgSellPrice;
    }

    public OpenPosition() { }
}
