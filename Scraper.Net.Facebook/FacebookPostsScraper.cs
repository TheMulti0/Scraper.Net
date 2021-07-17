using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class FacebookPostsScraper
    {
        private const string FacebookScriptName = "get_posts.py";

        private readonly FacebookScraperConfig _config;

        private readonly SemaphoreSlim _proxyIndexLock = new(1, 1);
        private int _proxyIndex;

        public FacebookPostsScraper(
            FacebookScraperConfig config)
        {
            _config = config;
        }

        public async Task<IEnumerable<FacebookPost>> GetPostsAsync(string id, CancellationToken ct)
        {
            GetPostsResponse response = await GetResponseAsync(id, ct);

            HandleError(response);

            return response.Posts.Select(raw => raw.ToPost());
        }

        private async Task<GetPostsResponse> GetResponseAsync(string id, CancellationToken ct)
        {
            var request = new GetPostsRequest
            {
                UserId = id,
                Pages = _config.PageCount,
                Proxy = await GetProxyAsync(ct),
                CookiesFileName = _config.CookiesFileName
            };

            string json = JsonSerializer.Serialize(request)
                .Replace("\"", "\\\""); // Python argument's double quoted strings need to be escaped

            string responseStr = await ScriptExecutor.ExecutePython(
                FacebookScriptName,
                ct,
                json);

            var response = JsonSerializer.Deserialize<GetPostsResponse>(responseStr);
            
            return response with { OriginalRequest = request };
        }

        private async Task<string> GetProxyAsync(CancellationToken ct)
        {
            if (_config.Proxies.Length == 0)
            {
                return null;
            }
            
            await _proxyIndexLock.WaitAsync(ct);

            try
            {
                if (_proxyIndex == _config.Proxies.Length - 1)
                {
                    _proxyIndex = 0;
                }
                else
                {
                    _proxyIndex++;
                }
                
                return _config.Proxies[_proxyIndex];
            }
            finally
            {
                _proxyIndexLock.Release();
            }
        }

        private static void HandleError(GetPostsResponse response)
        {
            switch (response.Error)
            {
                case "ProxyError":
                    throw new InvalidOperationException($"Proxy is invalid, proxy is {response.OriginalRequest.Proxy}");
                case "TemporarilyBanned":
                    throw new InvalidOperationException($"Temporarily banned, proxy is {response.OriginalRequest.Proxy}");
                case "InvalidCookies":
                    throw new InvalidOperationException("Invalid cookies passed in the cookies file");
                case "LoginRequired":
                    throw new InvalidOperationException($"Login required in order to view {response.OriginalRequest.UserId}");
                default:
                    if (response.Posts == null)
                    {
                        throw new Exception($"Unrecognized error {response.Error} {response.ErrorDescription}");    
                    }
                    break;
            }
        }
    }
}