using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PostsListener
{
    public class InMemoryPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly object _postUrlsLock = new();
        private readonly List<string> _postUrls = new();
        private readonly ILogger<InMemoryPostUrlsPersistence> _logger;

        public InMemoryPostUrlsPersistence(ILogger<InMemoryPostUrlsPersistence> logger)
        {
            _logger = logger;
        }

        public Task<bool> ExistsAsync(string url, CancellationToken ct = default)
        {
            lock (_postUrlsLock)
            {
                return Task.FromResult(_postUrls.Contains(url));
            }
        }

        public Task AddAsync(string url, CancellationToken ct = default)
        {
            lock (_postUrlsLock)
            {
                _postUrls.Add(url);
            }

            _logger.LogInformation("Added post {}", url);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string url, CancellationToken ct = default)
        {
            lock (_postUrlsLock)
            {
                if (!_postUrls.Remove(url))
                {
                    throw new InvalidOperationException($"Failed to remove url {url}");
                }
            }

            _logger.LogInformation("Removed post {}", url);

            return Task.CompletedTask;
        }
    }
}