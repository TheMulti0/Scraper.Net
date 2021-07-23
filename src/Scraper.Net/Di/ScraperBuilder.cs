using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net
{
    /// <summary>
    /// A class for configuring scraper-related services
    /// </summary>
    public class ScraperBuilder
    {
        public IServiceCollection Services { get; }

        public ScraperBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Adds a singleton service of the type <see cref="IPlatformScraper" /> and string platform using a
        /// factory specified in <paramref name="factory"/> to the
        /// specified IServiceCollection
        /// </summary>
        /// <param name="factory">The factory that creates the service</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public ScraperBuilder AddScraper(Func<IServiceProvider, (IPlatformScraper, string)> factory)
        {
            Services.AddSingleton(p =>
            {
                (IPlatformScraper scraper, string platform) = factory(p);
                
                return new RegisteredPlatformScraper(platform, scraper);
            });
            
            return this;
        }

        /// <summary>
        /// Adds a singleton service of the type <see cref="PostFilter" /> to the
        /// specified IServiceCollection
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public ScraperBuilder AddPostFilter(PostFilter filter)
        {
            Services.AddSingleton(filter);

            return this;
        }

        /// <summary>
        /// Adds a singleton service of the type <see cref="IPostProcessor" /> using a
        /// factory specified in <paramref name="factory"/> to the
        /// specified IServiceCollection
        /// </summary>
        /// <param name="factory">The factory that creates the service</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public ScraperBuilder AddPostProcessor(Func<IServiceProvider, IPostProcessor> factory)
        {
            Services.AddSingleton(factory);
            
            return this;
        }
    }
}