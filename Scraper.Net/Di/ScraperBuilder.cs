using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net
{
    public class ScraperBuilder
    {
        public IServiceCollection Services { get; }

        public ScraperBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ScraperBuilder AddScraper(Func<IServiceProvider, (IPlatformScraper, string)> factory)
        {
            Services.AddSingleton(p =>
            {
                (IPlatformScraper scraper, string platform) = factory(p);
                
                return new RegisteredPlatformScraper(platform, scraper);
            });
            
            return this;
        }

        public ScraperBuilder AddPostProcessor(Func<IServiceProvider, IPostProcessor> factory)
        {
            Services.AddSingleton(factory);
            
            return this;
        }
    }
}