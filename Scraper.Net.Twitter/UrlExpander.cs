using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scraper.Net.Twitter
{
    public class UrlExpander
    {
        private readonly HttpClient _httpClient;

        public UrlExpander()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> ExpandAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            bool wasRedirected = response.StatusCode == HttpStatusCode.Redirect ||
                                 response.StatusCode == HttpStatusCode.Moved ||
                                 response.StatusCode == HttpStatusCode.MovedPermanently;
            Uri actualUrl = response.Headers.Location;

            return wasRedirected && actualUrl != null
                ? actualUrl.ToString()
                : url;
        }
    }
}