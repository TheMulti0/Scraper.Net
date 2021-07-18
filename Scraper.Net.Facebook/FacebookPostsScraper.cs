using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class FacebookPostsScraper
    {
        private const string FacebookScriptName = "get_posts.py";

        private readonly FacebookConfig _config;
        private readonly SemaphoreSlim _proxyIndexLock = new(1, 1);
        private int _proxyIndex;

        public FacebookPostsScraper(FacebookConfig config)
        {
            if (_config.MaxPageCount < 1)
            {
                throw new ArgumentException(nameof(_config.MaxPageCount));
            }
            _config = config;
        }

        public IAsyncEnumerable<FacebookPost> GetFacebookPostsAsync(
            string id,
            CancellationToken ct)
        {
            return GetRawPosts(id, ct).Select(raw => raw.ToPost());
        }

        private async IAsyncEnumerable<RawFacebookPost> GetRawPosts(
            string id,
            [EnumeratorCancellation] CancellationToken ct)
        {
            GetPostsRequest request = await CreateGetPostsRequest(id, ct);

            IAsyncEnumerable<string> postsJson = GetRawPostsJson(request, ct);

            IAsyncEnumerable<RawFacebookPost> posts = postsJson.Select(json => Deserialize(json, request));

            await foreach (RawFacebookPost post in posts.WithCancellation(ct))
            {
                yield return post;
            }
        }

        private static RawFacebookPost Deserialize(string json, GetPostsRequest request)
        {
            try
            {
                var post = JsonSerializer.Deserialize<RawFacebookPost>(json);

                if (post?.Available != true)
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

        private IAsyncEnumerable<string> GetRawPostsJson(
            GetPostsRequest request,
            CancellationToken ct)
        {
            return ScriptExecutor.Execute(
                _config.PythonPath,
                FacebookScriptName,
                request,
                ct);
        }

        private async Task<GetPostsRequest> CreateGetPostsRequest(string id, CancellationToken ct) => new GetPostsRequest
        {
            UserId = id,
            Pages = _config.MaxPageCount,
            Proxy = await GetProxyAsync(ct),
            CookiesFileName = _config.CookiesFileName
        };

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

        private static void HandleError(Error error, GetPostsRequest request)
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
                default:
                    throw new Exception($"Unrecognized error {error} {error.Message}");    
            }
        }
    }
}