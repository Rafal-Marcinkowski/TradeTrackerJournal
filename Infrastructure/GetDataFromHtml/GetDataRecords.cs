using Infrastructure.DownloadHtmlData;
using Serilog;
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
            if (String.IsNullOrEmpty(html) || html == "BiznesRadarServerError")
            {
                return finalDataRecords;
            }
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
            if (String.IsNullOrEmpty(html) || html == "BiznesRadarServerError")
            {
                Log.Error<string>($"Problemy przy pobieraniu rekordów {html}", html);
            }
            var newRecords = await GetRelevantNodes.PrepareRecords(html);
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

        List<DataRecord> medianDataRecords = [];
        List<DataRecord> currentDataRecords = [];
        List<DataRecord> allNecessaryDataRecords = [];

        html = await DownloadPageSource.DownloadHtmlAsync(companyCode);
        if (String.IsNullOrEmpty(html) || html == "BiznesRadarServerError")
        {
            Log.Error<string>($"Problemy przy pobieraniu rekordów {html}", html);
        }
        currentDataRecords = await GetRelevantNodes.PrepareRecords(html);
        allNecessaryDataRecords.AddRange(currentDataRecords);
        medianDataRecords = currentDataRecords.Where(q => q.Date.Date < entryDate.Date).ToList();

        if (medianDataRecords.Count < 20)
        {
            int remainingBefore = 20 - medianDataRecords.Count;
            if (remainingBefore > 0)
            {
                currentDataRecords = await GetAdditionalRecords(companyCode, remainingBefore, true, entryDate.Date);
                medianDataRecords.InsertRange(0, currentDataRecords);
            }
        }
        allNecessaryDataRecords.AddRange(medianDataRecords);
        return medianDataRecords;
    }

    public async static Task<IEnumerable<DataRecord>> GetAllNecessaryRecords(Transaction transaction)
    {
        List<DataRecord> currentDataRecords = [];
        List<DataRecord> allNecessaryDataRecords = [];
        string html = string.Empty;
        int counter = 1;
        bool predicate;

        do
        {
            html = counter == 1
                ? await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName)
                : await DownloadPageSource.DownloadHtmlAsync(transaction.CompanyName, true, counter);

            currentDataRecords = await GetRelevantNodes.PrepareRecords(html);
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

