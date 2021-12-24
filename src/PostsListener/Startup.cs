using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Scraper.MassTransit.Client;
using Scraper.MassTransit.Common;
using Scraper.Net;
using Scraper.Net.Stream;

namespace PostsListener
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Action<IServiceCollectionBusConfigurator> ConfigureServices(IServiceCollection services)
        {
            AddStream(services);
            AddPersistence(services);
            
            services.AddSingleton<LastPostFilter>();
            services.AddSingleton<PostUrlFilter>();
            services.AddSingleton<PostFilter>();
            services.AddSingleton<ISubscriptionsManager, SubscriptionsManager>();
            services.AddHostedService<SubscriptionsLoaderService>();

            services.AddScraperMassTransitClient();

            return ConfigureMassTransit;
        }

        private void AddStream(IServiceCollection services)
        {
            var config = _configuration.GetSection("PostsStreamer").Get<PostStreamConfig>() ?? new PostStreamConfig();

            services.AddStream(
                provider => provider.GetRequiredService<PostFilter>().FilterAsync,
                config);
            
            var streamerManagerConfig = _configuration.GetSection("StreamerManager").Get<StreamConfig>() ?? new StreamConfig();
            
            services.AddSingleton(
                provider => new StreamManager(
                    streamerManagerConfig,
                    provider.GetRequiredService<PostStreamFactory>(),
                    provider.GetRequiredService<IBus>(),
                    provider.GetRequiredService<ILogger<StreamManager>>()));
        }

        private void ConfigureMassTransit(IServiceCollectionBusConfigurator x)
        {
            x.AddConsumer<AddOrUpdateNewPostSubscriptionConsumer>();
            x.AddConsumer<RemoveNewPostSubscriptionConsumer>();
            x.AddConsumer<GetNewPostSubscriptionsConsumer>();
            x.AddConsumer<PollNewPostSubscriptionConsumer>();
        }

        private void AddPersistence(IServiceCollection services)
        {
            IConfigurationSection mongoDbConfigg = _configuration.GetSection("MongoDb");
            var mongoDbConfig = mongoDbConfigg.Get<MongoDbConfig>();
            if (mongoDbConfigg.GetValue<bool>("Enabled") && mongoDbConfig != null)
            {
                services.AddSingleton(MongoDatabaseFactory.CreateDatabase(mongoDbConfig));

                services.AddSingleton<ISubscriptionsPersistence>(
                    provider => new MongoDbSubscriptionsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        provider.GetRequiredService<ILogger<MongoDbSubscriptionsPersistence>>()));
                
                services.AddSingleton<ILastPostsPersistence>(
                    provider => new MongoDbLastPostsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        provider.GetRequiredService<ILogger<MongoDbLastPostsPersistence>>()));
                
                services.AddSingleton<IPostUrlsPersistence>(
                    provider => new MongoDbPostUrlsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        mongoDbConfigg.GetSection("PostUrls").Get<PostUrlsPersistenceConfig>() ?? new PostUrlsPersistenceConfig(),
                        provider.GetRequiredService<ILogger<MongoDbPostUrlsPersistence>>()));
            }
            else
            {
                services.AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>();
                services.AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>();
                services.AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>();
            }
        }
    }
}