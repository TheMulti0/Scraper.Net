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
        /// <param name="config"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            PostFilter filter,
            PostsStreamerConfig config = null)
        {
            return services.AddStream(_ => filter, config);
        }

        /// <summary>
        /// Adds a <see cref="PostsStreamer"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">Creates a filter to distinct between old and new posts</param>
        /// <param name="config"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            Func<IServiceProvider, PostFilter> action,
            PostsStreamerConfig config = null)
        {
            return services.AddSingleton(
                provider => new PostsStreamer(
                    provider.GetRequiredService<IScraperService>(),
                    action(provider),
                    config,
                    provider.GetRequiredService<ILogger<PostsStreamer>>()));
        }
    }
}