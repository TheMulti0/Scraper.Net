using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scraper.Net;
using Scraper.Net.Facebook;

namespace FacebookScraperApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IScraperService service = GetScraperService();

            while (true)
            {
                Console.Write("Enter a user to scrape on Facebook (enter c to cancel): ");
                string id = Console.ReadLine();
                if (id == "c")
                {
                    break;
                }
                Console.WriteLine();
                
                await PrintPostsAsync(service, id);

                Console.WriteLine();
            }
        }

        private static IScraperService GetScraperService()
        {
            var facebookConfig = new FacebookConfig
            {
                MaxPageCount = 1
            };

            var provider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddScraper(builder => builder.AddFacebook(facebookConfig))
                .BuildServiceProvider();

            return provider.GetRequiredService<IScraperService>();
        }

        private static async Task PrintPostsAsync(IScraperService service, string id)
        {
            try
            {
                IAsyncEnumerable<Post> posts = service.GetPostsAsync(id, "facebook");

                await foreach (Post post in posts)
                {
                    Console.WriteLine(post.Url);
                }
            }
            catch (IdNotFoundException)
            {
                Console.WriteLine("User not found!");
            }
            catch (LoginRequiredException)
            {
                Console.WriteLine("Login required!");
            }
        }
    }
}