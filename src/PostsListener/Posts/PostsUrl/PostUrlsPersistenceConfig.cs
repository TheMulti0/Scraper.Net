using System;

namespace PostsListener
{
    public class PostUrlsPersistenceConfig
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromDays(7);
    }
}