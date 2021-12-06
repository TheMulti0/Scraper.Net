using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly StreamManager _streamManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsManager(
            StreamManager streamManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamManager = streamManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public IEnumerable<Subscription> Get()
        {
            return _streamManager.Get().Keys;
        }

        public async Task AddOrUpdateAsync(
            Subscription subscription,
            DateTime? earliestPostDate = null,
            CancellationToken ct = default)
        {
            SubscriptionEntity entity = await ToEntity(subscription, ct);

            PostSubscription postSubscription = _streamManager.AddOrUpdate(subscription, earliestPostDate ?? DateTime.MinValue);

            await _subscriptionsPersistence.AddOrUpdateAsync(entity, ct);
            
            postSubscription.SaveNewDueTimes(entity, _subscriptionsPersistence.AddOrUpdateAsync);
        }

        private async Task<SubscriptionEntity> ToEntity(Subscription subscription, CancellationToken ct)
        {
            SubscriptionEntity existing = await _subscriptionsPersistence.GetAsync(
                subscription.Id,
                subscription.Platform,
                ct);

            SubscriptionEntity entity = subscription.ToNewEntity();

            if (existing == null)
            {
                return entity;
            }
            
            return entity with { SubscriptionId = existing.SubscriptionId };
        }

        public async Task RemoveAsync(Subscription subscription, CancellationToken ct)
        {
            _streamManager.Remove(subscription);
            
            SubscriptionEntity entity = await _subscriptionsPersistence.GetAsync(subscription.Id, subscription.Platform, ct);
            
            await _subscriptionsPersistence.RemoveAsync(entity, ct);
        }
    }
}