using System.Net.Http;

namespace Infrastructure.DownloadHtmlData;

public class DownloadPageSource
{
    public async static Task<string> DownloadHtmlAsync(string companyCode)
    {
        string url = $"https://www.biznesradar.pl/notowania/{companyCode}#1d_lin_lin";
        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        using (HttpClient client = new HttpClient(handler))
        {
            while (true)
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    string requestUri = response.RequestMessage.RequestUri.ToString();
                    if (requestUri != url)
                    {
                        url = $"{requestUri},Q";
                        continue;
                    }
                    using (HttpContent content = response.Content)
                    {
                        var json = await content.ReadAsStringAsync();
                        return json;
                    }
                }
            }
        }
    }
}
