using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class PostsScraper
    {
        private const string ScriptName = "get_posts.py";

        private readonly FacebookConfig _config;

        public PostsScraper(FacebookConfig config)
        {
            _config = config;
        }

        public IAsyncEnumerable<FacebookPost> GetFacebookPostsAsync(
            string id,
            string proxy,
            CancellationToken ct)
        {
            return GetRawPosts(id, proxy, ct).Select(raw => raw.ToPost());
        }

        private async IAsyncEnumerable<RawFacebookPost> GetRawPosts(
            string id,
            string proxy,
            [EnumeratorCancellation] CancellationToken ct)
        {
            GetPostsRequest request = CreateGetPostsRequest(id, proxy);

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
                var exception = JsonSerializer.Deserialize<FacebookScraperException>(json);
                
                if (exception == null)
                {
                    throw;
                }
                
                ExceptionHandler.HandleException(request.UserId, request.Proxy, exception);
                throw; // HandleException should throw exception
            }
        }

        private IAsyncEnumerable<string> GetRawPostsJson(
            GetPostsRequest request,
            CancellationToken ct)
        {
            return ScriptExecutor.Execute(
                _config.PythonPath,
                ScriptName,
                request,
                ct);
        }

        private GetPostsRequest CreateGetPostsRequest(string id, string proxy) => new()
        {
            UserId = id,
            Pages = _config.MaxPageCount,
            Proxy = proxy,
            CookiesFileName = _config.CookiesFileName
        };
    }
}