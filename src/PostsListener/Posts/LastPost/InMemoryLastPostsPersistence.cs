using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PostsListener
{
    public class InMemoryLastPostsPersistence : ILastPostsPersistence
    {
        private readonly object _lastPostsLock = new();
        private readonly List<LastPost> _lastPosts = new();
        private readonly ILogger<InMemoryLastPostsPersistence> _logger;

        public InMemoryLastPostsPersistence(ILogger<InMemoryLastPostsPersistence> logger)
        {
            _logger = logger;
        }

        public IAsyncEnumerable<LastPost> GetAsync(CancellationToken ct = default)
        {
            lock (_lastPostsLock)
            {
                return _lastPosts.ToAsyncEnumerable();
            }
        }

        public Task<LastPost> GetAsync(string platform, string authorId, CancellationToken ct = default)
        {
            lock (_lastPostsLock)
            {
                return Task.FromResult(
                    _lastPosts.FirstOrDefault(
                        lastPost => lastPost.Platform == platform && lastPost.AuthorId == authorId));
            }
        }

        private Task AddAsync(LastPost lastPost)
        {
            lock (_lastPostsLock)
            {
                _lastPosts.Add(lastPost);
            }

            return Task.CompletedTask;
        }

        public Task AddOrUpdateAsync(
            string platform,
            string authorId,
            DateTime lastPostTime,
            CancellationToken ct = default)
        {
            lock (_lastPostsLock)
            {
                var lastPost = new LastPost
                {
                    Platform = platform,
                    AuthorId = authorId,
                    LastPostTime = lastPostTime
                };

                if (_lastPosts.Contains(lastPost))
                {
                    RemoveAsync(lastPost, ct)
                        .Wait(ct);
                }
                AddAsync(lastPost)
                    .Wait(ct);
            }

            _logger.LogInformation("Updated [{}] {} last post time to {}", platform, authorId, lastPostTime);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(LastPost lastPost, CancellationToken ct = default)
        {
            lock (_lastPostsLock)
            {
                if (!_lastPosts.Remove(lastPost))
                {
                    throw new InvalidOperationException("Failed to remove last post");
                }
            }

            _logger.LogInformation("Removed [{}] {} last post time", lastPost.Platform, lastPost.Id);

            return Task.CompletedTask;
        }
    }
}