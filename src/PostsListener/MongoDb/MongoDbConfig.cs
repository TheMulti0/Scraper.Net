using System;

namespace PostsListener
{
    public class MongoDbConfig
    {
        public Uri ConnectionString { get; set; } = new("mongodb://localhost:27017");

        public string DatabaseName { get; set; }
    }
}