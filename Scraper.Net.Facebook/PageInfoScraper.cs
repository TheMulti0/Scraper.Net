using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class PageInfoScraper
    {
        private const string ScriptName = "get_page_info.py";

        private readonly FacebookConfig _config;

        public PageInfoScraper(FacebookConfig config)
        {
            _config = config;
        }

        public async Task<PageInfo> GetPageInfoAsync(
            string id,
            CancellationToken ct)
        {
            GetPageInfoRequest request = await CreateGetPageInfoRequest(id, ct);

            string pageInfoJson = await GetPageInfoJson(request, ct).FirstOrDefaultAsync(ct);

            return Deserialize(pageInfoJson, request);
        }

        private static PageInfo Deserialize(string json, GetPageInfoRequest request)
        {
            try
            {
                var post = JsonSerializer.Deserialize<PageInfo>(json);

                if (post?.Url == null)
                {
                    throw new JsonException();
                }
                
                return post;
            }
            catch (JsonException)
            {
                var error = JsonSerializer.Deserialize<Error>(json);
                
                if (error == null)
                {
                    throw;
                }
                
                HandleError(error, request);
                throw; // HandleError should throw exception
            }
        }

        private IAsyncEnumerable<string> GetPageInfoJson(
            GetPageInfoRequest request,
            CancellationToken ct)
        {
            return ScriptExecutor.Execute(
                _config.PythonPath,
                ScriptName,
                request,
                ct);
        }

        private async Task<GetPageInfoRequest> CreateGetPageInfoRequest(string id, CancellationToken ct)
        {
            return new()
            {
                UserId = id,
                //Proxy = await GetProxyAsync(ct),
                CookiesFileName = _config.CookiesFileName
            };
        }
        
        private static void HandleError(Error error, GetPageInfoRequest request)
        {
            switch (error.Type)
            {
                case "ProxyError":
                    throw new InvalidOperationException($"Proxy is invalid, proxy is {request.Proxy}");
                case "TemporarilyBanned":
                    throw new InvalidOperationException($"Temporarily banned, proxy is {request.Proxy}");
                case "InvalidCookies":
                    throw new InvalidOperationException("Invalid cookies passed in the cookies file");
                case "LoginRequired":
                    throw new InvalidOperationException($"Login required in order to view {request.UserId}");
                case "HTTPError" when error.Message.StartsWith("404"):
                    throw new InvalidOperationException($"Cannot find user {request.UserId}");
                default:
                    throw new Exception($"Unrecognized error {error} {error.Message}");    
            }
        }
    }
}