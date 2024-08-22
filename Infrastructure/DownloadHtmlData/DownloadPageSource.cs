using Serilog;
using System.Net.Http;

namespace Infrastructure.DownloadHtmlData;

public class DownloadPageSource
{
    public async static Task<string> DownloadHtmlAsync(string companyCode, bool isArchivedPage = false, int archivedPageNumber = 0)
    {
        await Task.Delay(2000);
        string url = $"https://www.biznesradar.pl/notowania-historyczne/{companyCode}";
        if (isArchivedPage)
        {
            url = $"https://www.biznesradar.pl/notowania-historyczne/{companyCode},{archivedPageNumber}";
        }

        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };

        using HttpClient client = new HttpClient(handler);
        try
        {
            while (true)
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Error<HttpResponseMessage>($"Problemy z BiznesRadar: {response.Content}", response);
                    return "BiznesRadarServerError";
                }

                string requestUri = response.RequestMessage.RequestUri.ToString();
                if (requestUri != url)
                {
                    url = requestUri;
                    if (isArchivedPage)
                    {
                        url = $"{requestUri},{archivedPageNumber}";
                    }
                    await Task.Delay(1000);
                    continue;
                }

                using HttpContent content = response.Content;
                return await content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error<Exception>($"Problemy z BiznesRadarem: {ex.Message}", ex);
            return "BiznesRadarServerError";
        }
    }
}
