using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Stream
{
    /// <summary>
    /// Periodically polls for posts, distincts duplicates and exposes a stream of new posts
    /// </summary>
    public class PostsStreamer
    {
        private readonly IScraperService _service;
        private readonly PostFilter _filter;
        private readonly ILogger<PostsStreamer> _logger;

        public PostsStreamer(
            IScraperService service,
            PostFilter filter,
            ILogger<PostsStreamer> logger)
        {
            _service = service;
            _filter = filter;
            _logger = logger;
        }

        public IObservable<Post> Stream(
            string id,
            string platform,
            TimeSpan interval,
            IScheduler scheduler = null)
        {
            return PollingStreamer.Stream(
                ct => GetPostsAsync(id, platform, ct),
                post => _filter(post, platform),
                interval,
                scheduler);
        }

        private async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct)
        {
            _logger.LogInformation("Scraping [{}] {}", platform, id);
            
            IAsyncEnumerator<Post> enumerator = _service.GetPostsAsync(id, platform, ct)
                .GetAsyncEnumerator(ct);

            for (var more = true; more;)
            {
                try
                {
                    more = await enumerator.MoveNextAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get posts for [{}] {}", platform, id);
                }
                
                yield return enumerator.Current;
            }
            
            _logger.LogInformation("Done scraping [{}] {}", platform, id);
        } 
    }
}