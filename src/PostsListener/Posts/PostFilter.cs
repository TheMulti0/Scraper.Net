using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace PostsListener
{
    public class PostFilter
    {
        private const string Facebook = "facebook";
        private readonly LastPostFilter _lastPostFilter;
        private readonly PostUrlFilter _postUrlFilter;
        private readonly ILogger<PostFilter> _logger;

        public PostFilter(
            LastPostFilter lastPostFilter,
            PostUrlFilter postUrlFilter,
            ILogger<PostFilter> logger)
        {
            _lastPostFilter = lastPostFilter;
            _postUrlFilter = postUrlFilter;
            _logger = logger;
        }

        public async Task<bool> FilterAsync(
            Post post,
            string platform,
            CancellationToken ct)
        {
            async Task<bool> Filter()
            {
                if (platform == Facebook)
                {
                    return await _lastPostFilter.FilterAsync(post, platform, TimeSpan.FromMinutes(1), ct) &&
                           await _postUrlFilter.FilterAsync(post, ct);
                }

                return await _lastPostFilter.FilterAsync(post, platform, TimeSpan.Zero, ct);
            }

            if (await Filter())
            {
                _logger.LogDebug("Sending {}", post.Url);
                return true;
            }
            
            _logger.LogDebug("Not sending {}", post.Url);
            return false;
        }
    }
}