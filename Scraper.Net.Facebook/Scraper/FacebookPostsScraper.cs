using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Scraper.Net.Abstractions;

namespace Scraper.Net.Facebook.Scraper
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

        public async Task<IEnumerable<FacebookPost>> GetPostsAsync(User user)
        {
            var response = await GetResponseAsync(user);

            HandleError(response);

            return response.Posts.Select(raw => raw.ToPost());
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

        private async Task<GetPostsResponse> GetResponseAsync(User user)
        {
            var request = new GetPostsRequest
            {
                UserId = user.UserId,
                Pages = _config.PageCount,
                Proxy = await GetProxyAsync(),
                CookiesFileName = _config.CookiesFileName
            };

            string json = JsonSerializer.Serialize(request)
                .Replace("\"", "\\\""); // Python argument's double quoted strings need to be escaped

            string responseStr = await ScriptExecutor.ExecutePython(
                FacebookScriptName,
                token: default,
                json);

            var response = JsonSerializer.Deserialize<GetPostsResponse>(responseStr);
            
            return response with { OriginalRequest = request };
        }

        private async Task<string> GetProxyAsync()
        {
            if (_config.Proxies.Length == 0)
            {
                return null;
            }
            
            await _proxyIndexLock.WaitAsync();

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
    }
}