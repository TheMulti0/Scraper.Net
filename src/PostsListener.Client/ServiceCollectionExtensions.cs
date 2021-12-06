using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.MassTransit.Common;

namespace PostsListener.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollectionBusConfigurator AddPostsListenerClient<TNewPostConsumer>(
            this IServiceCollectionBusConfigurator configurator,
            int concurrencyLimit = 0)
            where TNewPostConsumer : class, IConsumer<NewPost>
        {
            if (concurrencyLimit <= 0)
            {
                configurator.AddConsumer<TNewPostConsumer>();
            }
            else
            {
                configurator.AddConsumer<TNewPostConsumer>(cc => cc.UseConcurrencyLimit(concurrencyLimit));
            }

            return configurator.AddPostsListenerClient();
        }

        public static IServiceCollectionBusConfigurator AddPostsListenerClient(
            this IServiceCollectionBusConfigurator configurator)
        {
            configurator.Collection
                .AddSingleton<INewPostSubscriptionsClient, NewPostSubscriptionsClient>();

            return configurator;
        }
    }
}