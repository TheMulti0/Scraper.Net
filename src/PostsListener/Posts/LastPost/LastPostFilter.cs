using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace PostsListener
{
    public class LastPostFilter
    {
        private readonly ILastPostsPersistence _persistence;
        private readonly ILogger<LastPostFilter> _logger;

        public LastPostFilter(
            ILastPostsPersistence persistence,
            ILogger<LastPostFilter> logger)
        {
            _persistence = persistence;
            _logger = logger;
        }

        public async Task<bool> FilterAsync(
            Post post,
            string platform,
            TimeSpan toleration,
            CancellationToken ct)
        {
            _logger.LogDebug("{} with creation date {}", post.Url, post.CreationDate);
            if (post.CreationDate == null)
            {
                _logger.LogDebug("returning because of null");
                return false;
            }

            LastPost existing = await _persistence.GetAsync(platform, post.Author.Id, ct);

            DateTime? lastPostCreationDate = existing?.LastPostTime.Floor(toleration);
            DateTime postCreationDate = ((DateTime)post.CreationDate).Floor(toleration);
            // post.CreationDate cannot be null here

            if (lastPostCreationDate >= postCreationDate)
            {
                _logger.LogDebug("returning because {} is bigger than {}", lastPostCreationDate, postCreationDate);
                return false;
            }

            await _persistence.AddOrUpdateAsync(platform, post.Author.Id, (DateTime)post.CreationDate, ct);
            return true;
        }
    }
}