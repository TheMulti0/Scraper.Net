using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace PostsListener
{
    public class MongoDbSubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly IMongoCollection<SubscriptionEntity> _subscriptions;
        private readonly ILogger<MongoDbSubscriptionsPersistence> _logger;
        private readonly UpdateOptions _updateOptions;

        public MongoDbSubscriptionsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbSubscriptionsPersistence> logger)
        {
            _logger = logger;
            _subscriptions = database.GetCollection<SubscriptionEntity>(nameof(SubscriptionEntity));
            
            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IAsyncEnumerable<SubscriptionEntity> GetAsync(CancellationToken ct = default)
        {
            return _subscriptions.AsAsyncEnumerable(
                FilterDefinition<SubscriptionEntity>.Empty,
                ct);
        }

        public Task<SubscriptionEntity> GetAsync(string id, string platform, CancellationToken ct = default)
        {
            return _subscriptions
                .AsQueryable()
                .FirstOrDefaultAsync(s => s.Id == id && s.Platform == platform, ct);
        }
        
        private Task<SubscriptionEntity> GetAsync(ObjectId id, CancellationToken ct = default)
        {
            return _subscriptions
                .AsQueryable()
                .FirstOrDefaultAsync(s => s.SubscriptionId == id, ct);
        }

        public async Task AddOrUpdateAsync(SubscriptionEntity subscription, CancellationToken ct = default)
        {
            UpdateResult result;
            do
            {
                int version = await GetDocumentVersion(subscription, ct);
                ObjectId id = GetSubscriptionId(subscription);

                UpdateDefinition<SubscriptionEntity> updateDefinition = Builders<SubscriptionEntity>.Update
                    .Set(s => s.Version, version + 1)
                    .Set(s => s.PollInterval, subscription.PollInterval)
                    .Set(s => s.NextPollTime, subscription.NextPollTime)
                    .SetOnInsert(s => s.Id, subscription.Id)
                    .SetOnInsert(s => s.Platform, subscription.Platform);

                result = await _subscriptions.UpdateOneAsync(
                    s => s.SubscriptionId == id &&
                         s.Version == version,
                    updateDefinition,
                    _updateOptions,
                    ct);

                if (!result.IsAcknowledged)
                {
                    throw new InvalidOperationException("Failed to add or update subscription");
                }    
            }
            while (result.ModifiedCount < 1 &&
                   result.UpsertedId == BsonObjectId.Empty);

            _logger.LogInformation("Updated subscription [{}] {} (interval is {}, nextPollTime is {})", subscription.Platform, subscription.Id, subscription.PollInterval, subscription.NextPollTime);
        }

        private static ObjectId GetSubscriptionId(SubscriptionEntity subscription)
        {
            ObjectId sid = subscription.SubscriptionId;
            return sid == ObjectId.Empty 
                ? ObjectId.GenerateNewId() 
                : sid;
        }

        private async Task<int> GetDocumentVersion(SubscriptionEntity subscription, CancellationToken ct)
        {
            SubscriptionEntity existing = null;
            
            if (subscription.SubscriptionId != ObjectId.Empty)
            {
                existing = await GetAsync(subscription.SubscriptionId, ct);
            }

            return existing?.Version ?? subscription.Version;
        }

        public async Task RemoveAsync(SubscriptionEntity subscription, CancellationToken ct = default)
        {
            var result = await _subscriptions.DeleteOneAsync(
                s => s.SubscriptionId == subscription.SubscriptionId,
                ct);
            
            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            _logger.LogInformation("Removed subscription [{}] {}", subscription.Platform, subscription.Id);
        }
    }
}