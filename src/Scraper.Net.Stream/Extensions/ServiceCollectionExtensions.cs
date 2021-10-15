using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Stream
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="PostStreamFactory"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="filter">A filter to distinct between old and new posts</param>
        /// <param name="config"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            PostFilter filter,
            PostStreamConfig config = null)
        {
            return services.AddStream(_ => filter, config);
        }

        /// <summary>
        /// Adds a <see cref="PostStreamFactory"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">Creates a filter to distinct between old and new posts</param>
        /// <param name="config"></param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddStream(
            this IServiceCollection services,
            Func<IServiceProvider, PostFilter> action,
            PostStreamConfig config = null)
        {
            return services.AddSingleton(
                provider => new PostStreamFactory(
                    provider.GetRequiredService<IScraperService>(),
                    action(provider),
                    config ?? new PostStreamConfig(),
                    provider.GetRequiredService<ILogger<IPostStream>>()));
        }
    }
}