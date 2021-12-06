using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper.MassTransit.Common;
using Scraper.Net;
using TheMulti0.Console;

namespace PostsListener.Client.Sample
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
                            .AddMassTransit(
                                x =>
                                {
                                    x.AddPostsListenerClient<NewPostConsumer>();

                                    x.UsingRabbitMq(
                                        (context, cfg) =>
                                        {
                                            RabbitMqConfig rabbitMqConfig = new();
                                            cfg.Host("amqp://guest:guest@144.172.75.90:5672//");
                                
                                            cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                            cfg.ConfigureEndpoints(context);
                                        });
                                })
                            .AddMassTransitHostedService()
                            .AddHostedService<Subscriber>();
                    });
    }
}