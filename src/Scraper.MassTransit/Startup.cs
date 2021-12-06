using HtmlCssToImage.Net;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.MassTransit.Common;
using Scraper.Net;
using Scraper.Net.Facebook;
using Scraper.Net.Feeds;
using Scraper.Net.Screenshot;
using Scraper.Net.Twitter;
using Scraper.Net.Youtube;
using Scraper.Net.YoutubeDl;

namespace Scraper.MassTransit
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqConfig = _configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
            
            services.AddScraper(BuildScraper);

            services
                .AddMassTransit(
                    x =>
                    {
                        x.AddConsumer<GetAuthorConsumer>();
                        x.AddConsumer<GetPostsConsumer>();

                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                cfg.Host(rabbitMqConfig.ConnectionString);
                                
                                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                cfg.ConfigureEndpoints(context);
                            });
                    })
                .AddMassTransitHostedService();
        }

        private void BuildScraper(ScraperBuilder builder)
        {
            IConfiguration scraperConfig = _configuration.GetSection("Scraper");
            
            IConfigurationSection feedsConfig = scraperConfig.GetSection("Feeds");
            if (feedsConfig.GetValue<bool?>("Enabled") != false)
            {
                builder.AddFeeds();
            }

            IConfigurationSection twitterConfig = scraperConfig.GetSection("Twitter");
            var twitterConfigg = twitterConfig.Get<TwitterConfig>();
            if (twitterConfig.GetValue<bool>("Enabled") && twitterConfigg != null)
            {
                builder.AddTwitter(twitterConfigg);
            }

            IConfigurationSection facebookConfig = scraperConfig.GetSection("Facebook");
            if (facebookConfig.GetValue<bool>("Enabled"))
            {
                builder.AddFacebook(facebookConfig.Get<FacebookConfig>());
            }
            
            IConfigurationSection youtubeConfig = scraperConfig.GetSection("Youtube");
            if (youtubeConfig.GetValue<bool>("Enabled"))
            {
                builder.AddYoutube(youtubeConfig.Get<YoutubeConfig>());
            }

            IConfigurationSection youtubeDlConfig = scraperConfig.GetSection("YoutubeDl");
            if (youtubeDlConfig.GetValue<bool>("Enabled"))
            {
                builder.AddYoutubeDl(youtubeDlConfig.Get<YoutubeDlConfig>());
            }
            
            IConfigurationSection screenshotDlConfig = scraperConfig.GetSection("Screenshot");
            if (screenshotDlConfig.GetValue<bool>("Enabled"))
            {
                builder.AddScreenshot(
                    b => b.AddTwitter(),
                    screenshotDlConfig.Get<HtmlCssToImageCredentials>());
            }
        }
    }
}