using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public interface ISubscriptionsManager
    {
        IEnumerable<Subscription> Get();
        
        Task AddOrUpdateAsync(
            Subscription subscription,
            DateTime? earliestPostDate = null,
            CancellationToken ct = default);

        Task RemoveAsync(Subscription subscription, CancellationToken ct);
    }
}