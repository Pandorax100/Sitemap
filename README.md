# Pandorax.Sitemap

A small sitemap library for .NET 10 that writes sitemap.xml files and sitemap indexes with optional extensions (image, video, news, and hreflang).

## Features

- Writes sitemaps and sitemap indexes
- Supports image, video, news, and hreflang extensions
- Streaming XML output via `XmlWriter`

## Install

Install from NuGet:

```bash
dotnet add package Pandorax.Sitemap
```

Or with the NuGet Package Manager:

```powershell
Install-Package Pandorax.Sitemap
```

## Quick Start

```csharp
using System;
using System.Threading.Tasks;
using Pandorax.Sitemap.Extensions;
using Pandorax.Sitemap.Models;

public static class Example
{
    public static async Task<string> BuildSitemapAsync()
    {
        var sitemap = new Sitemap(new[]
        {
            new UrlEntry(
                loc: new Uri("https://example.com/"),
                lastModified: new DateTimeOffset(2026, 1, 29, 12, 30, 0, TimeSpan.Zero),
                changeFrequency: ChangeFrequency.Daily,
                priority: 1.0m,
                images: new[]
                {
                    new ImageEntry(new Uri("https://example.com/images/hero.jpg"))
                    {
                        Title = "Homepage Hero",
                        Caption = "Our main banner"
                    }
                },
                videos: new[]
                {
                    new VideoEntry(
                        thumbnailLoc: new Uri("https://example.com/videos/intro-thumb.jpg"),
                        title: "Intro",
                        description: "Quick product introduction")
                    {
                        ContentLoc = new Uri("https://example.com/videos/intro.mp4"),
                        Duration = TimeSpan.FromSeconds(90)
                    }
                },
                news: new[]
                {
                    new NewsEntry(
                        publicationName: "Example News",
                        publicationLanguage: "en",
                        publicationDate: new DateTimeOffset(2026, 1, 25, 8, 0, 0, TimeSpan.Zero),
                        title: "We launched a new feature")
                    {
                        Keywords = "feature,launch"
                    }
                },
                alternates: new[]
                {
                    new AlternateLink("en", new Uri("https://example.com/")),
                    new AlternateLink("fr", new Uri("https://example.com/fr/"))
                })
        });

        return await sitemap.ToXmlAsync();
    }
}
```

## Writing to a Stream

```csharp
using System.IO;
using System.Threading.Tasks;
using Pandorax.Sitemap.Models;
using Pandorax.Sitemap.Writers;

public static class StreamExample
{
    public static async Task WriteToStreamAsync(Sitemap sitemap, Stream output)
    {
        var writer = new SitemapWriter();
        await writer.WriteAsync(sitemap, output);
    }
}
```

## Minimal ASP.NET MVC Controller Example

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pandorax.Sitemap.Extensions;
using Pandorax.Sitemap.Models;

[ApiController]
[Route("sitemap.xml")]
public sealed class SitemapController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var sitemap = new Sitemap(new[]
        {
            new UrlEntry(
                loc: new Uri("https://example.com/"),
                lastModified: new DateTimeOffset(2026, 1, 29, 12, 30, 0, TimeSpan.Zero))
        });

        var xml = await sitemap.ToXmlAsync();
        return Content(xml, "application/xml");
    }
}
```

## Validation

- URLs must be absolute (`Uri.IsAbsoluteUri`)
- A sitemap can contain up to 50,000 URLs
- A sitemap index can contain up to 50,000 entries
- Required extension fields are enforced (e.g., `image:loc`, `video:thumbnail_loc`)

## License

MIT.
