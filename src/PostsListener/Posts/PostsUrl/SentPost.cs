using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PostsListener
{
    internal class SentPost
    {
        [BsonId]
        public string Url { get; set; }

        public DateTime SentAt { get; set; }
    }
}