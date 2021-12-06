using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScraperMassTransitClient(
            this IServiceCollection services,
            TimeSpan? getPostsTimeout = null)
        {
            return services
                .AddSingleton<IScraperService>(
                    provider => new ScraperMassTransitClient(
                        provider.GetRequiredService<IBus>(),
                        getPostsTimeout));
        }
    }
}