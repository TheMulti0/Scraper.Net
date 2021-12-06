using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsLoaderService : IHostedService
    {
        private readonly StreamManager _streamManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsLoaderService(
            StreamManager streamManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamManager = streamManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            var subscriptions = _subscriptionsPersistence
                .GetAsync(ct);
            
            await foreach (SubscriptionEntity entity in subscriptions.WithCancellation(ct))
            {
                Subscription subscription = entity.ToSubscription();
                
                var postSubscription = _streamManager.AddOrUpdate(subscription, DateTime.MinValue);

                postSubscription.SaveNewDueTimes(entity, _subscriptionsPersistence.AddOrUpdateAsync);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (PostSubscription disposable in _streamManager.Get().Values)
            {
                disposable.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}