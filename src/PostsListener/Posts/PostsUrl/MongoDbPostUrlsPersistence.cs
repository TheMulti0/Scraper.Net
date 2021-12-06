using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace PostsListener
{
    public class MongoDbPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly IMongoCollection<SentPost> _collection;
        private readonly ILogger<MongoDbPostUrlsPersistence> _logger;

        public MongoDbPostUrlsPersistence(
            IMongoDatabase database,
            PostUrlsPersistenceConfig config,
            ILogger<MongoDbPostUrlsPersistence> logger)
        {
            _logger = logger;
            _collection = database.GetCollection<SentPost>("PostUrls");

            if (config.ExpirationTime > TimeSpan.Zero &&
                _collection.Indexes.List()
                    .ToList()
                    .Count < 2) // There shouldn't be more than two indices in the collection
            {
                CreateExpirationIndex(config);
            }
        }

        private void CreateExpirationIndex(PostUrlsPersistenceConfig config)
        {
            TimeSpan configExpirationTime = config.ExpirationTime;

            _logger.LogInformation("Creating expiration index of time {}", configExpirationTime);
        
            IndexKeysDefinition<SentPost> keys = Builders<SentPost>.IndexKeys
                .Ascending(update => update.SentAt);

            var options = new CreateIndexOptions
            {
                ExpireAfter = configExpirationTime
            };

            var indexModel = new CreateIndexModel<SentPost>(keys, options);

            _collection.Indexes.CreateOne(indexModel);
        }

        public Task<bool> ExistsAsync(string url, CancellationToken ct = default)
        {
            return _collection
                .AsQueryable()
                .AnyAsync(sentUpdate => sentUpdate.Url == url, ct);
        }

        public async Task AddAsync(string url, CancellationToken ct = default)
        {
            var sentUpdate = new SentPost
            {
                SentAt = DateTime.Now,
                Url = url
            };

            await _collection.InsertOneAsync(sentUpdate, cancellationToken: ct);

            _logger.LogInformation("Added post {}", url);
        }

        public async Task RemoveAsync(string url, CancellationToken ct = default)
        {
            await _collection.DeleteOneAsync(
                new FilterDefinitionBuilder<SentPost>()
                    .Eq(s => s.Url, url), ct);

            _logger.LogInformation("Removed post {}", url);
        }
    }
}