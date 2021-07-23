using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    /// <summary>
    /// A class for configuring screenshot-related services
    /// </summary>
    public class ScreenshotterBuilder
    {
        public IServiceCollection Services { get; }

        public ScreenshotterBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Adds a singleton service of the type <see cref="IPlatformScreenshotter" /> and string platform using a
        /// factory specified in <paramref name="factory"/> to the
        /// specified IServiceCollection
        /// </summary>
        /// <param name="factory">The factory that creates the service</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public ScreenshotterBuilder AddScreenshotter(Func<IServiceProvider, (IPlatformScreenshotter, string)> factory)
        {
            Services.AddSingleton(p =>
            {
                (IPlatformScreenshotter screenshotter, string platform) = factory(p);
                
                return new RegisteredPlatformScreenshotter(platform, screenshotter);
            });
            
            return this;
        }
    }
}