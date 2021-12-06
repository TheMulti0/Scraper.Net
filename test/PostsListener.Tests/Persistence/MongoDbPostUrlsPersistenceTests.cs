using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class MongoDbPostUrlsPersistenceTests
    {
        private readonly IPostUrlsPersistence _persistence;

        public MongoDbPostUrlsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<IPostUrlsPersistence>(_ =>
                {
                    IMongoDatabase mongoDatabase = MongoDatabaseFactory.CreateDatabase(
                        new MongoDbConfig
                        {
                            DatabaseName = "ScraperDb"
                        });
                    var config = new PostUrlsPersistenceConfig();
                    
                    return new MongoDbPostUrlsPersistence(mongoDatabase, config, NullLogger<MongoDbPostUrlsPersistence>.Instance);
                })
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