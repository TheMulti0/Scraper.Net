using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Stream
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="PostsStreamer"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="filter">A filter to distinct between old and new posts</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            PostFilter filter)
        {
            return services.AddStream(_ => filter);
        }

        /// <summary>
        /// Adds a <see cref="PostsStreamer"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">Creates a filter to distinct between old and new posts</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            Func<IServiceProvider, PostFilter> action)
        {
            return services.AddSingleton(
                provider => new PostsStreamer(
                    provider.GetRequiredService<IScraperService>(),
                    action(provider),
                    provider.GetRequiredService<ILogger<PostsStreamer>>()));
        }
    }
}