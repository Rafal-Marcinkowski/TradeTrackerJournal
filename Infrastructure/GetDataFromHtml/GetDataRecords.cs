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
            await Task.Delay(2500);
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

    public async static Task<List<DataRecord>> GetAdditionalRecords(string companyCode, int neededRecords, bool beforeTransaction, DateTime entryDate)
    {
        List<DataRecord> additionalRecords = [];
        int counter = 2;

        do
        {
            string html = await DownloadPageSource.DownloadHtmlAsync(companyCode, true, counter);
            var newRecords = await GetRelevantNodes.PrepareRecords(companyCode);

            if (beforeTransaction)
            {
                var filteredRecords = newRecords
                    .Where(r => r.Date < entryDate.Date)
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

    public async static Task<IEnumerable<DataRecord>> GetRecordsForMedianTurnoverCalculation(string companyCode, DateTime entryDate)
    {
        string html = string.Empty;

        List<DataRecord> finalDataRecords = [];
        List<DataRecord> dataRecords = [];

        html = await DownloadPageSource.DownloadHtmlAsync(companyCode);
        if (String.IsNullOrEmpty(html))
        {
            return finalDataRecords;
        }
        dataRecords = await GetRelevantNodes.PrepareRecords(html);
        finalDataRecords = dataRecords.Where(q => q.Date.Date < entryDate.Date).ToList();

        if (finalDataRecords.Count < 20)
        {
            int remainingBefore = 20 - finalDataRecords.Count;
            if (remainingBefore > 0)
            {
                await Task.Delay(1000);
                dataRecords = await GetAdditionalRecords(companyCode, remainingBefore, true, entryDate);
                finalDataRecords.InsertRange(0, dataRecords);
            }
        }
        return finalDataRecords;
    }
}
