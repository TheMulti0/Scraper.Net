using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class InMemoryLastPostsPersistenceTests
    {
        private readonly CrudTestBase<LastPost> _crud;

        public InMemoryLastPostsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>()
                .BuildServiceProvider();

            var subscriptionsPersistence = provider.GetRequiredService<ILastPostsPersistence>();
            
            _crud = new CrudTestBase<LastPost>(
                () => new LastPost
                {
                    Platform = "facebook",
                    AuthorId = "test",
                    LastPostTime = DateTime.Now
                },
                subscriptionsPersistence.GetAsync,
                (post, ct) => subscriptionsPersistence.AddOrUpdateAsync(post.Platform, post.AuthorId, post.LastPostTime, ct),
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