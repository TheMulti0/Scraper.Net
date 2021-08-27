using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeSpan? _pollingTimeout;
        private readonly ILogger<PostsStreamer> _logger;

        public PostsStreamer(
            IScraperService service,
            PostFilter filter,
            PostsStreamerConfig config,
            ILogger<PostsStreamer> logger)
        {
            _service = service;
            _filter = filter;
            
            int max = config.MaxDegreeOfParallelism;
            if (max >= 1)
            {
                _semaphore = new SemaphoreSlim(max, max);
            }

            _pollingTimeout = config.PollingTimeout;
            
            _logger = logger;
        }

        public IObservable<Post> Stream(
            string id,
            string platform,
            TimeSpan interval,
            IScheduler scheduler = null)
        {
            IObservable<Post> stream = PollingStreamer.Stream(
                ct => PollAsync(id, platform, ct),
                interval,
                _pollingTimeout,
                scheduler);
            
            return stream
                .Where(post =>
                {
                    try
                    {
                        return _filter(post, platform);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e, "Failed to filter post {}", post.Url);
                        return false;
                    }
                }); // Apply filter
        }

        private async IAsyncEnumerable<Post> PollAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct)
        {
            await (_semaphore?.WaitAsync(ct) ?? Task.CompletedTask);

            try
            {
                _logger.LogInformation("Beginning to scrape [{}] {}", platform, id);
            
                await foreach (Post post in GetPostsAsync(id, platform, ct))
                {
                    yield return post;
                }

                _logger.LogInformation("Finished scraping [{}] {}", platform, id);
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        private async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct)
        {
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
        }
    }
}