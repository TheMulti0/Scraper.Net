using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper.Net.Screenshot
{
    public class ScreenshotterBuilder
    {
        public IServiceCollection Services { get; }

        public ScreenshotterBuilder(IServiceCollection services)
        {
            Services = services;
        }

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