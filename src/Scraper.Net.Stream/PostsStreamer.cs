using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
            IObservable<Unit> trigger = Observable
                .Timer(TimeSpan.Zero, interval)
                .Select(_ => Unit.Default);
            
            return Stream(
                id,
                platform,
                trigger,
                scheduler);
        }
        
        public IObservable<Post> Stream(
            string id,
            string platform,
            TimeSpan interval,
            IObservable<Unit> trigger,
            IScheduler scheduler = null)
        {
            IObservable<Unit> combinedTrigger = Observable
                .Timer(TimeSpan.Zero, interval)
                .Select(_ => Unit.Default)
                .Merge(trigger);
            
            return Stream(
                id,
                platform,
                combinedTrigger,
                scheduler);
        }

        public IObservable<Post> Stream(
            string id,
            string platform,
            IObservable<Unit> trigger,
            IScheduler scheduler = null)
        {
            IObservable<Post> stream = PollingStreamer.Stream(
                ct => PollAsync(id, platform, ct),
                trigger,
                _pollingTimeout,
                scheduler);
            
            return stream
                .WhereAwait(async post =>
                {
                    try
                    {
                        return await _filter(post, platform);
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

                IAsyncEnumerable<Post> posts = GetPostsAsync(id, platform, ct);
                await foreach (Post post in posts.WithCancellation(ct))
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

        private IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            CancellationToken ct)
        {
            return _service
                .GetPostsAsync(id, platform, ct)
                .OrderBy(post => post.CreationDate)
                .Catch<Post, Exception>(
                    e => _logger.LogError(e, "Failed to get posts for [{}] {}", platform, id));
        }
    }
}