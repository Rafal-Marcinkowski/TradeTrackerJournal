using Serilog;
using System.Net.Http;

namespace Infrastructure.DownloadHtmlData;

public class DownloadPageSource
{
    private static readonly HttpClient client = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    });

    public async static Task<(string html, string misdirectedUrl)> DownloadHtmlAsync(string companyCode, bool isArchivedPage = false, int archivedPageNumber = 0)
    {
        string url = $"https://www.biznesradar.pl/notowania-historyczne/{companyCode}";
        url = isArchivedPage ? $"{companyCode},{archivedPageNumber}" : url;

        if (archivedPageNumber >= 3)
        {
            url = url[..(url.IndexOf(',') + 1)] + archivedPageNumber;
        }

        Log.Information($"Pobieranie danych z URL: {url}");

        try
        {
            while (true)
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    string reason = response.ReasonPhrase;
                    Log.Error<HttpResponseMessage>($"Problemy z BiznesRadar: {response.StatusCode} {reason}", response);
                    return ("BiznesRadarServerError", url);
                }

                string requestUri = response.RequestMessage.RequestUri.ToString();

                if (requestUri != url)
                {
                    url = requestUri;
                    continue;
                }

                return (await response.Content.ReadAsStringAsync(), url);
            }
        }

        catch (HttpRequestException httpEx)
        {
            Log.Error<Exception>($"Problemy z połączeniem: {httpEx.Message}", httpEx);
            return ("BiznesRadarServerError", url);
        }

        catch (Exception ex)
        {
            Log.Error<Exception>($"Problemy z BiznesRadarem: {ex.Message}", ex);
            return ("BiznesRadarServerError", url);
        }
    }

    public async static Task<string> DownloadHtmlFromUrlAsync(string url)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }
}