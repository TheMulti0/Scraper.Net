using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scraper.Net
{
    public class ScraperService : IScraperService
    {
        private readonly IDictionary<string, IPlatformScraper> _platformScrapers;
        private readonly IEnumerable<IPostProcessor> _postProcessors;
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(
            IDictionary<string, IPlatformScraper> platformScrapers,
            IEnumerable<IPostProcessor> postProcessors,
            ILogger<ScraperService> logger)
        {
            _platformScrapers = platformScrapers;
            _postProcessors = postProcessors;
            _logger = logger;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IPlatformScraper scraper = GetScraper(platform);
            
            IAsyncEnumerable<Post> scrapedPosts = scraper.GetPostsAsync(id, ct);
            
            IAsyncEnumerable<Post> posts = _postProcessors.Aggregate(
                scrapedPosts,
                ProcessPosts);

            await foreach (Post processedPost in posts.WithCancellation(ct))
            {
                yield return processedPost;
            }
        }

        private IAsyncEnumerable<Post> ProcessPosts(
            IAsyncEnumerable<Post> posts,
            IPostProcessor postProcessor)
        {
            return posts.SelectMany(post =>
            {
                try
                {
                    return postProcessor.ProcessAsync(post);
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e, "Post processor failed");
                    return new [] { post }.ToAsyncEnumerable(); // Return original post without processing if processing fails
                }
            });
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