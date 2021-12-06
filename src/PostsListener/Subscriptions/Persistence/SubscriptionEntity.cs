using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PostsListener
{
    public sealed record SubscriptionEntity
    {
        [BsonId]
        public ObjectId SubscriptionId { get; init; }

        public int Version { get; init; }
        
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }

        public DateTime? NextPollTime { get; init; }
    }
}