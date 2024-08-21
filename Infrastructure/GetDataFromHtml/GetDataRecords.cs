using Infrastructure.DownloadHtmlData;
using SharedModels.Models;

namespace Infrastructure.GetDataFromHtml;

public class GetDataRecords
{
    public async static Task<IEnumerable<DataRecord>> PrepareDataRecords(Transaction transaction, bool isRecent = true)
    {
        string html = string.Empty;

        List<DataRecord> finalDataRecords = [];
        List<DataRecord> dataRecords = [];

        bool isFirstRecordPageEnough = false;
        int counter = 1;
        do
        {
            html = counter == 1 ? await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName)
                : await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName, true, counter);
            if (String.IsNullOrEmpty(html))
            {
                return finalDataRecords;
            }
            await Task.Delay(5000);
            dataRecords = await GetRelevantNodes.PrepareRecords(html);
            finalDataRecords.AddRange(dataRecords);
            if (isRecent)
            {
                isFirstRecordPageEnough = true;
            }
            else
            {
                isFirstRecordPageEnough = finalDataRecords.Where(q => q.Date.Date >= transaction.EntryDate.Date).Count() >= 30;
                counter++;
            }
        } while (!isFirstRecordPageEnough);
        return finalDataRecords;
    }

    public async static Task<List<DataRecord>> GetAdditionalRecords(Transaction transaction, int neededRecords, bool beforeTransaction)
    {
        List<DataRecord> additionalRecords = [];
        int counter = 2;

        do
        {
            string html = await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName, true, counter);
            var newRecords = await GetRelevantNodes.PrepareRecords(transaction.CompanyName);

            if (beforeTransaction)
            {
                var filteredRecords = newRecords
                    .Where(r => r.Date < transaction.EntryDate.Date)
                    .OrderByDescending(r => r.Date)
                    .Take(neededRecords)
                    .ToList();

                additionalRecords.AddRange(filteredRecords);
            }

            neededRecords -= additionalRecords.Count;
            counter++;
        } while (neededRecords > 0);

        return additionalRecords;
    }
}
