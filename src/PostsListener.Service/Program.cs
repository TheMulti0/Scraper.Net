using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Client;
using Scraper.MassTransit.Common;
using Scraper.Net;
using TheMulti0.Console;

namespace PostsListener.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                .ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;

                        builder
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureLogging(
                    (context, builder) => builder
                        .AddConfiguration(context.Configuration)
                        .AddTheMulti0Console()
                        .AddSentry())
                .ConfigureServices(
                    (context, services) =>
                    {
                        IConfiguration config = context.Configuration;
                        Action<IServiceCollectionBusConfigurator> massTransitCallback = new Startup(config).ConfigureServices(services);
                        
                        services
                            .AddScraperMassTransitClient()
                            .AddMassTransit(
                                x =>
                                {
                                    massTransitCallback(x);

                                    x.UsingRabbitMq(
                                        (context, cfg) =>
                                        {
                                            var rabbitMqConfig = config.GetSection("RabbitMq").Get<RabbitMqConfig>() ?? new RabbitMqConfig();
                                
                                            cfg.Host(rabbitMqConfig.ConnectionString);
                                
                                            cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                            cfg.ConfigureEndpoints(context);
                                        });
                                })
                            .AddMassTransitHostedService();
                    });
        }
    }
}