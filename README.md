# Scraper.Net

## What is Scraper.Net?

Scraper.Net is a multi-platform, asynchronous .NET library responsible of providing easy post scraping over a variety of sources.

## Where to begin?

Refer to the [documentation site](https://scraper-net.github.io/Scraper.Net) which contains [guides](https://scraper-net.github.io/Scraper.Net/articles/getting_started.html) along with an [API reference](https://scraper-net.github.io/Scraper.Net/api/index.html).

## Simple Usage

This example is taken from the [Getting started](https://scraper-net.github.io/Scraper.Net/articles/getting_started.html) page in the documentation site.

For background explanation and breakdown of the code refer to the page.

```cs
var provider = new ServiceCollection()
    .AddLogging()
    .AddScraper(builder => builder.AddFacebook())
    .BuildServiceProvider();

var service = provider.GetRequiredService<IScraperService>();

var posts = service.GetPostsAsync("NaftaliBennett", "facebook");

await foreach (var post in posts)
{
    Console.WriteLine(post.Content);
}
```
