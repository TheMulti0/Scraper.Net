using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class InMemorySubscriptionsPersistenceTests
    {
        private readonly CrudTestBase<SubscriptionEntity> _crud;

        public InMemorySubscriptionsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>()
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