using System.Net.Http;

namespace Infrastructure.DownloadHtmlData;

public class DownloadPageSource
{
    public async static Task<string> DownloadHtmlAsync(string companyCode, bool isArchivedPage = false, int archivedPageNumber = 0)
    {
        string url = $"https://www.biznesradar.pl/notowania-historyczne/{companyCode}";
        if (isArchivedPage)
        {
            url = $"https://www.biznesradar.pl/notowania-historyczne/{companyCode},{archivedPageNumber}";
        }

        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };

        using (HttpClient client = new HttpClient(handler))
        {
            try
            {
                while (true)
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return string.Empty;
                        }

                        string requestUri = response.RequestMessage.RequestUri.ToString();
                        if (requestUri != url)
                        {
                            url = $"{requestUri},Q";
                            continue;
                        }

                        using (HttpContent content = response.Content)
                        {
                            return await content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
