using Infrastructure.DownloadHtmlData;
using Serilog;
using SharedProject.Models;

namespace Infrastructure.GetDataFromHtml;

public class GetDataRecords
{
    public async static Task<IEnumerable<DataRecord>> GetAllNecessaryRecords(Transaction transaction)
    {
        List<DataRecord> currentDataRecords = [];
        List<DataRecord> allNecessaryDataRecords = [];
        string html = string.Empty;
        string misdirectedUrl = null;
        int counter = 1;
        bool predicate;

        do
        {
            await Task.Delay(new Random().Next(1000, 1500));
            (html, misdirectedUrl) = counter == 1
            ? await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName)
            : await DownloadPageSource.DownloadHtmlAsync(misdirectedUrl, true, counter);

            currentDataRecords = await GetRelevantNodes.PrepareRecords(html);
            if (currentDataRecords.Count == 0)
            {
                Log.Information("Osiągnięto koniec dostępnych danych. Błędne dane transakcji lub błąd serwera.");
                allNecessaryDataRecords.Clear();
                break;
            }
            allNecessaryDataRecords.AddRange(currentDataRecords);

            if (allNecessaryDataRecords.First().Date <= transaction.EntryDate.Date)
            {
                return allNecessaryDataRecords;
            }
            if (transaction.EntryMedianTurnover == 0)
            {
                predicate = allNecessaryDataRecords.Where(q => q.Date < transaction.EntryDate).Count() <= 20;
            }
            else
            {
                predicate = !allNecessaryDataRecords.Any(q => q.Date.Date <= transaction.EntryDate.Date);
            }

            counter++;
        } while (predicate);
        return allNecessaryDataRecords;
    }
}

