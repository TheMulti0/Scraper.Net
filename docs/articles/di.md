---
uid: articles.di
---

In order to register Scraper.Net in a [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) DI container you need to call `AddScraper`:

```cs
services.AddScraper(builder => {});
```

The lambda provided is the configurator of the [`ScraperBuilder`](xref:Scraper.Net.ScraperBuilder).

Using the scraper builder it is possible to add a platform scraper, post filter and post processor easily.

Such as:

```cs
services.AddScraper(builder => builder
    .AddScraper(provider => (new MyScraper(provider.GetService<ILoggerFactory>), "myscraper"))
    .AddPostFilter((post, platform, ct) => post.Url.StartsWith("https"))
    .AddPostProcessor(provider => new MyPostProcessor(provider.GetService<ILoggerFactory>)));
```

The official libraries include extension methods to simplify this process:

```cs
services.AddScraper(builder => builder.AddFacebook().AddYoutubeDl());
```