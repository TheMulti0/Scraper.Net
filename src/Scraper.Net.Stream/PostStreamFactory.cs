using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Stream
{
    /// <summary>
    /// Creates a <see cref="IPostStream"/>
    /// </summary>
    public class PostStreamFactory
    {
        private readonly IScraperService _service;
        private readonly PostFilter _filter;
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeSpan? _pollingTimeout;
        private readonly ILogger<IPostStream> _logger;

        public PostStreamFactory(
            IScraperService service,
            PostFilter filter,
            PostStreamConfig config,
            ILogger<IPostStream> logger)
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
        public IPostStream Stream(
            string id,
            string platform,
            TimeSpan interval,
            DateTime? nextPollTime = null,
            IScheduler scheduler = null)
        {
            async Task<bool> Filter(Post post)
            {
                try
                {
                    return await _filter(post, platform);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to filter post {}", post.Url);
                    return false;
                }
            }

            return new PostStream(
                interval,
                _pollingTimeout,
                nextPollTime,
                scheduler,
                ct => PollAsync(id, platform, ct),
                Filter);
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
                .Do(
                    _ => { },
                    e => _logger.LogError(e, "Failed to get posts for [{}] {}", platform, id));
        }
    }
}