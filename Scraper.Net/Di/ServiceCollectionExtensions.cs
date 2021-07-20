using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.Net
{
    public static class ServiceCollectionExtensions
    {
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

            IEnumerable<IPostProcessor> postProcessors = provider.GetServices<IPostProcessor>();

            var logger = provider.GetService<ILogger<ScraperService>>();

            return new ScraperService(platformScrapers, postProcessors, logger);
        }
    }
}