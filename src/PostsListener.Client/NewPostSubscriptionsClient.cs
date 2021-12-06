using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;

namespace PostsListener.Client
{
    internal class NewPostSubscriptionsClient : INewPostSubscriptionsClient
    {
        private static readonly GetNewPostSubscriptions GetNewPostSubscriptions = new();

        private readonly IRequestClient<GetNewPostSubscriptions> _getSubscriptions;
        private readonly IRequestClient<AddOrUpdateNewPostSubscription> _addSubscription;
        private readonly IRequestClient<RemoveNewPostSubscription> _removeSubscription;
        private readonly IRequestClient<PollNewPostSubscription> _poll;

        public NewPostSubscriptionsClient(IBus bus)
        {
            _getSubscriptions = bus.CreateRequestClient<GetNewPostSubscriptions>();
            _addSubscription = bus.CreateRequestClient<AddOrUpdateNewPostSubscription>();
            _removeSubscription = bus.CreateRequestClient<RemoveNewPostSubscription>();
            _poll = bus.CreateRequestClient<PollNewPostSubscription>();
        }
        
        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct)
        {
            Response<Subscriptions> subscriptions = await _getSubscriptions.GetResponse<Subscriptions>(GetNewPostSubscriptions, ct);

            return subscriptions.Message.Items;
        }

        public async Task AddOrUpdateSubscription(
            string id,
            string platform,
            TimeSpan pollInterval,
            DateTime earliestPostDate,
            CancellationToken ct)
        {
            await _addSubscription.GetResponse<OperationSucceeded>(
                new AddOrUpdateNewPostSubscription
                {
                    Id = id,
                    Platform = platform,
                    PollInterval = pollInterval,
                    EarliestPostDate = earliestPostDate
                }, ct);
        }

        public async Task RemoveSubscription(
            string id,
            string platform,
            CancellationToken ct)
        {
            await _removeSubscription.GetResponse<OperationSucceeded>(
                new RemoveNewPostSubscription
                {
                    Id = id,
                    Platform = platform
                }, ct);
        }

        public async Task TriggerPoll(string id, string platform, CancellationToken ct = default)
        {
            await _poll.GetResponse<OperationSucceeded>(
                new PollNewPostSubscription
                {
                    Id = id,
                    Platform = platform
                }, ct);
        }
    }
}