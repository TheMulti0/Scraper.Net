using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class GetNewPostSubscriptionsConsumer : IConsumer<GetNewPostSubscriptions>
    {
        private readonly ISubscriptionsManager _subscriptionsManager;

        public GetNewPostSubscriptionsConsumer(ISubscriptionsManager subscriptionsManager)
        {
            _subscriptionsManager = subscriptionsManager;
        }

        public async Task Consume(ConsumeContext<GetNewPostSubscriptions> context)
        {
            IEnumerable<Subscription> subscriptions = _subscriptionsManager.Get();

            await context.RespondAsync(
                new Subscriptions
                {
                    Items = subscriptions
                });
        }
    }
}