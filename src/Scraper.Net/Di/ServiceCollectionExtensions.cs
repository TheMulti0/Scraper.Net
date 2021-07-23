using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.Net
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a singleton service of the type <see cref="IScraperService" /> using a factory of scraper-related services
        /// specified in <paramref name="builderAction"/> to the
        /// specified IServiceCollection
        /// </summary>
        /// <param name="services">This service collection</param>
        /// <param name="builderAction">Scraper-related services factory</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddScraper(
            this IServiceCollection services,
            Action<ScraperBuilder> builderAction)
        {
            builderAction(new ScraperBuilder(services));

            return services.AddSingleton(CreateScraperService);
        }

        private static IScraperService CreateScraperService(IServiceProvider provider)
        {
            Dictionary<string, IPlatformScraper> platformScrapers = provider
                .GetServices<RegisteredPlatformScraper>()
                .ToDictionary(r => r.Platform, r => r.Scraper);

            var postFilters = provider.GetServices<PostFilter>();
            var postProcessors = provider.GetServices<IPostProcessor>();
            var logger = provider.GetService<ILogger<ScraperService>>();

            return new ScraperService(platformScrapers, postFilters, postProcessors, logger);
        }
    }
}