using System;
using MongoDB.Bson;

namespace PostsListener
{
    public sealed record LastPost
    {
        public ObjectId Id { get; set; }

        public string Platform { get; init; }

        public string AuthorId { get; init; }

        public DateTime LastPostTime { get; set; }

        public bool Equals(LastPost other) => other != null &&
                                              Platform == other.Platform &&
                                              AuthorId == other.AuthorId;

        public override int GetHashCode() => HashCode.Combine(Platform, AuthorId);
    }
}