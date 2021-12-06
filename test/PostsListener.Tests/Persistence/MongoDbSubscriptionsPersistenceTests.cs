using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class MongoDbSubscriptionsPersistenceTests
    {
        private readonly CrudTestBase<SubscriptionEntity> _crud;

        public MongoDbSubscriptionsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ISubscriptionsPersistence>(
                    _ => new MongoDbSubscriptionsPersistence(
                        MongoDatabaseFactory.CreateDatabase(new MongoDbConfig
                        {
                            DatabaseName = "ScraperDb"
                        }),
                        NullLogger<MongoDbSubscriptionsPersistence>.Instance))
                .BuildServiceProvider();

            var subscriptionsPersistence = provider.GetRequiredService<ISubscriptionsPersistence>();
            
            var id = ObjectId.GenerateNewId();
            
            _crud = new CrudTestBase<SubscriptionEntity>(
                () => new SubscriptionEntity
                {
                    SubscriptionId = id,
                    Platform = "facebook",
                    Id = "test",
                    PollInterval = TimeSpan.FromHours(1)
                },
                subscriptionsPersistence.GetAsync,
                subscriptionsPersistence.AddOrUpdateAsync,
                subscriptionsPersistence.RemoveAsync);
        }
        
        [Test]
        public async Task TestAddSingleAsync()
        {
            await _crud.TestAddSingleAsync();
        }
        
        [Test]
        public async Task TestAddRemoveSingleAsync()
        {
            await _crud.TestAddRemoveSingleAsync();
        }
    }
}