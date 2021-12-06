using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class InMemoryPostUrlsPersistenceTests
    {
        private readonly IPostUrlsPersistence _persistence;

        public InMemoryPostUrlsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>()
                .BuildServiceProvider();

            _persistence = provider.GetRequiredService<IPostUrlsPersistence>();
        }
        
        [Test]
        public async Task TestAddSingleAsync()
        {
            const string url = "my-url";

            if (await _persistence.ExistsAsync(url))
            {
                await _persistence.RemoveAsync(url);
            }

            await _persistence.AddAsync(url);
            Assert.IsTrue(await _persistence.ExistsAsync(url));
        }
        
        [Test]
        public async Task TestAddRemoveSingleAsync()
        {
            const string url = "my-url";

            if (await _persistence.ExistsAsync(url))
            {
                await _persistence.RemoveAsync(url);
            }

            await _persistence.AddAsync(url);
            Assert.IsTrue(await _persistence.ExistsAsync(url));

            await _persistence.RemoveAsync(url);
            Assert.IsFalse(await _persistence.ExistsAsync(url));
        }
    }
}