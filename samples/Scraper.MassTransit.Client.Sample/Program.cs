using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.MassTransit.Common;
using Scraper.Net;
using TheMulti0.Console;

namespace Scraper.MassTransit.Client.Sample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddTheMulti0Console())
                .ConfigureServices(
                    services =>
                    {
                        services
                            .AddScraperMassTransitClient()
                            .AddMassTransit(
                                x =>
                                {
                                    x.UsingRabbitMq(
                                        (context, cfg) =>
                                        {
                                            RabbitMqConfig rabbitMqConfig = new();
                                            cfg.Host(rabbitMqConfig.ConnectionString);
                                
                                            cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                            cfg.ConfigureEndpoints(context);
                                        });
                                })
                            .AddMassTransitHostedService()
                            .AddHostedService<Scraper>();
                    });
    }
}