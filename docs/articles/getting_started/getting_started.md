---
uid: guides.getting_started
---

# Getting started

Scraper.Net is the base project containing the main services and abstractions.

Platform specific scraping capabilities are implemented in separate projects (which all reference the base project), or can be self implemented.

Without a platform specific implementation the base project supplies no real functionality.

## Installation

To get started, install the implementation package\s of choice 

The following installation procedure is written for the Facebook package but is the same for other packages

### [dotnet CLI](#tab/dotnet-cli)
Run the following:

```
dotnet add package Scraper.Net.Facebook
```

### [Project file](#tab/project-file)
Add this to your project's `.csproj` file:

```xml
<PackageReference Include="Scraper.Net.Facebook" Version="*" />
```

***

## Usage

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

In the following example `Microsoft.Extensions.DependencyInjection` is used to create an [`IScraperService`](xref:Scraper.Net.IScraperService).

Using the [`AddScraper`](xref:Scraper.Net.ScraperBuilder) extension method it is possible to register in the [`ScraperBuilder`](xref:Scraper.Net.ScraperBuilder) your desired platform scraper.

After building the service provider, the generated [`IScraperService`](xref:Scraper.Net.IScraperService) is used to get the posts asynchronously using async enumerable and print their content.