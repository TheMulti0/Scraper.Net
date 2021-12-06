using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace PostsListener
{
    public class MongoDbLastPostsPersistence : ILastPostsPersistence
    {
        private readonly ILogger<MongoDbLastPostsPersistence> _logger;
        private readonly IMongoCollection<LastPost> _lastPosts;
        private readonly UpdateOptions _updateOptions;

        public MongoDbLastPostsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbLastPostsPersistence> logger)
        {
            _logger = logger;
            _lastPosts = database.GetCollection<LastPost>(nameof(LastPost));

            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IAsyncEnumerable<LastPost> GetAsync(CancellationToken ct = default)
        {
            return _lastPosts.AsAsyncEnumerable(
                FilterDefinition<LastPost>.Empty,
                ct);
        }

        public Task<LastPost> GetAsync(string platform, string authorId, CancellationToken ct = default)
        {
            return _lastPosts
                .AsQueryable()
                .FirstOrDefaultAsync(lastPost => lastPost.Platform == platform && lastPost.AuthorId == authorId, ct);
        }

        public async Task AddOrUpdateAsync(
            string platform,
            string authorId,
            DateTime lastPostTime,
            CancellationToken ct = default)
        {
            UpdateDefinition<LastPost> updateDefinition = Builders<LastPost>.Update
                .Set(post => post.LastPostTime, lastPostTime);

            UpdateResult result = await _lastPosts.UpdateOneAsync(
                post => post.Platform == platform && post.AuthorId == authorId,
                updateDefinition,
                _updateOptions,
                ct);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to add or update last post");
            }

            _logger.LogInformation("Updated [{}] {} last post time to {}", platform, authorId, lastPostTime);
        }

        public async Task RemoveAsync(LastPost lastPost, CancellationToken ct = default)
        {
            var result = await _lastPosts.DeleteOneAsync(
                l => l.Id == lastPost.Id,
                ct);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to remove last post");
            }

            _logger.LogInformation("Removed [{}] {} last post time", lastPost.Platform, lastPost.Id);
        }
    }
}