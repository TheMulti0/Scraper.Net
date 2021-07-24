using Microsoft.Extensions.DependencyInjection;

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
            return services.AddSingleton(
                provider => new PostsStreamer(
                    provider.GetRequiredService<IScraperService>(),
                    filter));
        }
    }
}