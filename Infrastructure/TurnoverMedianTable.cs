using System.IO;

namespace Infrastructure;

public class TurnoverMedianTable
{
    public static async Task UpdateMedianTable()
    {
        if (File.Exists("C:\\Users\\rafal\\Desktop\\Pogromcy\\NotoriaSerwis_2\\TurnoverMedianTable"))
        {
            File.Copy("C:\\Users\\rafal\\Desktop\\Pogromcy\\NotoriaSerwis_2\\TurnoverMedianTable",
                 "C:\\Users\\rafal\\Desktop\\Pogromcy\\TradeTrackerJournal\\TurnoverMedianTable", true);
        }

        await InitializeTurnoverMedian();
    }

    public static Dictionary<string, string> TurnoverMedianDictionary { get; set; }

    public static async Task InitializeTurnoverMedian()
    {
        TurnoverMedianDictionary = [];
        if (File.Exists("C:\\Users\\rafal\\Desktop\\Pogromcy\\TradeTrackerJournal\\TurnoverMedianTable"))
        {
            using StreamReader reader = new("C:\\Users\\rafal\\Desktop\\Pogromcy\\TradeTrackerJournal\\TurnoverMedianTable");
            string line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                string[] parts = line.Split(' ');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    TurnoverMedianDictionary.Add(key, value);
                }
            }
        }
    }

    public async static Task<string> GetTurnoverMedianForCompany(string companyCode)
    {
        if (TurnoverMedianDictionary == null || !TurnoverMedianDictionary.TryGetValue(companyCode, out string? value))
            return string.Empty;

        return value;
    }
}
