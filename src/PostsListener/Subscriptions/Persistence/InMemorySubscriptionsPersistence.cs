using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PostsListener
{
    public class InMemorySubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly object _subscriptionsLock = new();
        private readonly List<SubscriptionEntity> _subscriptions = new();
        private readonly ILogger<InMemorySubscriptionsPersistence> _logger;

        public InMemorySubscriptionsPersistence(ILogger<InMemorySubscriptionsPersistence> logger)
        {
            _logger = logger;
        }

        public IAsyncEnumerable<SubscriptionEntity> GetAsync(CancellationToken ct = default)
        {
            lock (_subscriptionsLock)
            {
                return _subscriptions.ToAsyncEnumerable();
            }
        }

        public Task<SubscriptionEntity> GetAsync(string id, string platform, CancellationToken ct = default)
        {
            lock (_subscriptionsLock)
            {
                return Task.FromResult(
                    _subscriptions.Find(entity => entity.Id == id && entity.Platform == platform));
            }
        }

        public Task AddOrUpdateAsync(SubscriptionEntity subscription, CancellationToken ct = default)
        {
            lock (_subscriptionsLock)
            {
                _subscriptions.Add(subscription);
            }
            
            _logger.LogInformation("Added subscription [{}] {}", subscription.Platform, subscription.Id);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(SubscriptionEntity subscription, CancellationToken ct = default)
        {
            lock (_subscriptionsLock)
            {
                if (!_subscriptions.Remove(subscription))
                {
                    throw new InvalidOperationException("Failed to remove subscription");
                }    
            }
            
            _logger.LogInformation("Removed subscription [{}] {}", subscription.Platform, subscription.Id);

            return Task.CompletedTask;
        }
    }
}