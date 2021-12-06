using MongoDB.Bson;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public static class SubscriptionsExtensions
    {
        public static Subscription ToSubscription(this SubscriptionEntity entity)
        {
            return new Subscription
            {
                Id = entity.Id,
                Platform = entity.Platform,
                PollInterval = entity.PollInterval,
                NextPollTime = entity.NextPollTime
            };
        }
        
        public static SubscriptionEntity ToNewEntity(this Subscription entity)
        {
            return new SubscriptionEntity
            {
                SubscriptionId = ObjectId.Empty,
                Version = 0,
                Id = entity.Id,
                Platform = entity.Platform,
                PollInterval = entity.PollInterval,
                NextPollTime = entity.NextPollTime
            };
        }
    }
}