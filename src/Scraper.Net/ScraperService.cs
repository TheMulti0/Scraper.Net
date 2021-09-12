using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scraper.Net
{
    /// <summary>
    /// Implementation of <see cref="IScraperService"/> which is built on top of
    /// platform scrapers <see cref="IPlatformScraper"/>,
    /// post filters <see cref="PostFilter"/> and
    /// post processors <see cref="IPostProcessor"/>
    /// </summary>
    public class ScraperService : IScraperService
    {
        private readonly IDictionary<string, IPlatformScraper> _platformScrapers;
        private readonly IAsyncEnumerable<PostFilter> _postFilters;
        private readonly IEnumerable<IPostProcessor> _postProcessors;
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(
            IDictionary<string, IPlatformScraper> platformScrapers,
            IEnumerable<PostFilter> postFilters,
            IEnumerable<IPostProcessor> postProcessors,
            ILogger<ScraperService> logger)
        {
            _platformScrapers = platformScrapers;
            _postFilters = postFilters.ToAsyncEnumerable();
            _postProcessors = postProcessors;
            _logger = logger;
        }

        public Task<Author> GetAuthorAsync(
            string id,
            string platform,
            CancellationToken ct = default)
        {
            IPlatformScraper scraper = GetScraper(platform);

            return scraper.GetAuthorAsync(id, ct);
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IPlatformScraper scraper = GetScraper(platform);

            IAsyncEnumerable<Post> scrapedPosts = scraper
                .GetPostsAsync(id, ct)
                .WhereAwait(post => FilterAsync(post, platform, ct));
            
            IAsyncEnumerable<Post> posts = _postProcessors.Aggregate(
                scrapedPosts,
                (p, pP) => ProcessPosts(p, pP, platform));

            var any = false;
            
            await foreach (Post processedPost in posts.WithCancellation(ct))
            {
                if (!any)
                    any = true;
                yield return processedPost;
            }

            if (!any)
            {
                _logger.LogWarning("No posts found for [{}] {}", platform, id);
            }
        }

        private ValueTask<bool> FilterAsync(Post post, string platform, CancellationToken ct)
        {
            async ValueTask<bool> Filter(PostFilter filter)
            {
                try
                {
                    return await filter(post, platform);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to filter post {}", post.Url);
                    return false;
                }
            }

            return _postFilters.AllAwaitAsync(Filter, ct);
        }

        private IAsyncEnumerable<Post> ProcessPosts(
            IAsyncEnumerable<Post> posts,
            IPostProcessor postProcessor,
            string platform)
        {
            return posts.SelectMany(post => ProcessPost(post, postProcessor, platform));
        }

        private async IAsyncEnumerable<Post> ProcessPost(
            Post post,
            IPostProcessor postProcessor,
            string platform)
        {
            IAsyncEnumerator<Post> enumerator = postProcessor
                .ProcessAsync(post, platform)
                .GetAsyncEnumerator();
            var canContinue = true;

            while (canContinue)
            {
                Post processedPost;

                try
                {
                    if (!await enumerator.MoveNextAsync())
                    {
                        break;
                    }
                    processedPost = enumerator.Current;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Post processor failed");
                    
                    canContinue = false;

                    processedPost = post; // Return original post without processing if processing fails
                }   
                
                // the yield statement is outside the try catch block
                yield return processedPost;
            }
        }

        private IPlatformScraper GetScraper(string platform)
        {
            if (!_platformScrapers.ContainsKey(platform))
            {
                throw new ArgumentException($"No scraper found for platform {platform}");
            }

            return _platformScrapers[platform];
        }
    }
}