using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace PostsListener.Client
{
    public interface INewPostSubscriptionsClient
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct = default);

        Task AddOrUpdateSubscription(
            string id,
            string platform,
            TimeSpan pollInterval,
            DateTime earliestPostDate,
            CancellationToken ct = default);

        Task RemoveSubscription(
            string id,
            string platform,
            CancellationToken ct = default);

        Task TriggerPoll(
            string id,
            string platform,
            CancellationToken ct = default);
    }
}