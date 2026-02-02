using System.Text;
using System.Xml.Linq;
using Pandorax.Sitemap.Extensions;
using Pandorax.Sitemap.Models;
using Pandorax.Sitemap.Options;
using Pandorax.Sitemap.Writers;
using Xunit;
using SitemapIndexModel = Pandorax.Sitemap.Models.SitemapIndex;
using SitemapModel = Pandorax.Sitemap.Models.Sitemap;

namespace Pandorax.Sitemap.Tests;

public sealed class SitemapWriterTests
{
    private static readonly XNamespace SitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private static readonly XNamespace ImageNs = "http://www.google.com/schemas/sitemap-image/1.1";
    private static readonly XNamespace VideoNs = "http://www.google.com/schemas/sitemap-video/1.1";
    private static readonly XNamespace NewsNs = "http://www.google.com/schemas/sitemap-news/0.9";
    private static readonly XNamespace XhtmlNs = "http://www.w3.org/1999/xhtml";

    [Fact]
    public async Task WritesMinimalSitemap()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"))
        });

        var doc = await WriteAsync(sitemap);
        Assert.Equal(SitemapNs + "urlset", doc.Root?.Name);
        Assert.Equal(SitemapNs.NamespaceName, doc.Root?.GetDefaultNamespace().NamespaceName);

        var urls = doc.Root?.Elements(SitemapNs + "url").ToList();
        Assert.NotNull(urls);
        Assert.Single(urls!);
        Assert.Equal("https://example.com/", urls![0].Element(SitemapNs + "loc")?.Value);

        var nsDeclarations = doc.Root?.Attributes().Where(a => a.IsNamespaceDeclaration).ToList();
        Assert.NotNull(nsDeclarations);
        Assert.Single(nsDeclarations!);
    }

    [Fact]
    public async Task WritesLastModifiedInUtc()
    {
        var lastModified = new DateTimeOffset(2026, 1, 29, 12, 30, 0, TimeSpan.FromHours(2));
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"), lastModified: lastModified)
        });

        var doc = await WriteAsync(sitemap);
        var lastmod = doc.Root?.Element(SitemapNs + "url")?.Element(SitemapNs + "lastmod")?.Value;

        Assert.Equal("2026-01-29T10:30:00Z", lastmod);
    }

    [Fact]
    public async Task WritesChangeFrequencyAndPriority()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                changeFrequency: ChangeFrequency.Weekly,
                priority: 0.6m)
        });

        var doc = await WriteAsync(sitemap);
        var url = doc.Root?.Element(SitemapNs + "url");

        Assert.Equal("weekly", url?.Element(SitemapNs + "changefreq")?.Value);
        Assert.Equal("0.6", url?.Element(SitemapNs + "priority")?.Value);
    }

    [Fact]
    public async Task EmitsImageNamespaceWhenImagesPresent()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                images: new[] { new ImageEntry(new Uri("https://example.com/image.jpg")) })
        });

        var doc = await WriteAsync(sitemap);

        Assert.True(HasNamespace(doc.Root, "image", ImageNs));
        Assert.NotNull(doc.Root?.Element(SitemapNs + "url")?.Element(ImageNs + "image"));
    }

    [Fact]
    public async Task EmitsVideoNamespaceWhenVideosPresent()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                videos: new[]
                {
                    new VideoEntry(
                        thumbnailLoc: new Uri("https://example.com/thumb.jpg"),
                        title: "Intro",
                        description: "Description")
                    {
                        ContentLoc = new Uri("https://example.com/video.mp4")
                    }
                })
        });

        var doc = await WriteAsync(sitemap);

        Assert.True(HasNamespace(doc.Root, "video", VideoNs));
        Assert.NotNull(doc.Root?.Element(SitemapNs + "url")?.Element(VideoNs + "video"));
    }

    [Fact]
    public async Task EmitsNewsNamespaceWhenNewsPresent()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                news: new[]
                {
                    new NewsEntry(
                        publicationName: "Example",
                        publicationLanguage: "en",
                        publicationDate: new DateTimeOffset(2026, 1, 25, 8, 0, 0, TimeSpan.Zero),
                        title: "Launch")
                })
        });

        var doc = await WriteAsync(sitemap);

        Assert.True(HasNamespace(doc.Root, "news", NewsNs));
        Assert.NotNull(doc.Root?.Element(SitemapNs + "url")?.Element(NewsNs + "news"));
    }

    [Fact]
    public async Task EmitsXhtmlNamespaceWhenAlternatesPresent()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                alternates: new[]
                {
                    new AlternateLink("en", new Uri("https://example.com/en"))
                })
        });

        var doc = await WriteAsync(sitemap);

        Assert.True(HasNamespace(doc.Root, "xhtml", XhtmlNs));
        Assert.NotNull(doc.Root?.Element(SitemapNs + "url")?.Element(XhtmlNs + "link"));
    }

    [Fact]
    public async Task WritesImageOptionalFieldsOnlyWhenProvided()
    {
        var image = new ImageEntry(new Uri("https://example.com/image.jpg"))
        {
            Title = "Hero",
            Caption = "Caption",
            GeoLocation = "London",
            License = new Uri("https://example.com/license")
        };

        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"), images: new[] { image })
        });

        var doc = await WriteAsync(sitemap);
        var imageElement = doc.Root?.Element(SitemapNs + "url")?.Element(ImageNs + "image");

        Assert.Equal("https://example.com/image.jpg", imageElement?.Element(ImageNs + "loc")?.Value);
        Assert.Equal("Hero", imageElement?.Element(ImageNs + "title")?.Value);
        Assert.Equal("Caption", imageElement?.Element(ImageNs + "caption")?.Value);
        Assert.Equal("London", imageElement?.Element(ImageNs + "geo_location")?.Value);
        Assert.Equal("https://example.com/license", imageElement?.Element(ImageNs + "license")?.Value);
    }

    [Fact]
    public async Task WritesVideoFieldsIncludingTags()
    {
        var video = new VideoEntry(
            thumbnailLoc: new Uri("https://example.com/thumb.jpg"),
            title: "Intro",
            description: "Description")
        {
            ContentLoc = new Uri("https://example.com/video.mp4"),
            Duration = TimeSpan.FromSeconds(91),
            PublicationDate = new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero),
            FamilyFriendly = true,
            Category = "Docs",
            Uploader = "Pandorax",
            UploaderInfo = new Uri("https://example.com/uploader"),
            ViewCount = 42,
            Tags = new[] { "alpha", "beta" }
        };

        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"), videos: new[] { video })
        });

        var doc = await WriteAsync(sitemap);
        var videoElement = doc.Root?.Element(SitemapNs + "url")?.Element(VideoNs + "video");

        Assert.Equal("https://example.com/thumb.jpg", videoElement?.Element(VideoNs + "thumbnail_loc")?.Value);
        Assert.Equal("Intro", videoElement?.Element(VideoNs + "title")?.Value);
        Assert.Equal("Description", videoElement?.Element(VideoNs + "description")?.Value);
        Assert.Equal("https://example.com/video.mp4", videoElement?.Element(VideoNs + "content_loc")?.Value);
        Assert.Equal("91", videoElement?.Element(VideoNs + "duration")?.Value);
        Assert.Equal("2026-01-01T09:00:00Z", videoElement?.Element(VideoNs + "publication_date")?.Value);
        Assert.Equal("yes", videoElement?.Element(VideoNs + "family_friendly")?.Value);
        Assert.Equal("Docs", videoElement?.Element(VideoNs + "category")?.Value);
        Assert.Equal("Pandorax", videoElement?.Element(VideoNs + "uploader")?.Value);
        Assert.Equal("https://example.com/uploader", videoElement?.Element(VideoNs + "uploader")?.Attribute("info")?.Value);
        Assert.Equal("42", videoElement?.Element(VideoNs + "view_count")?.Value);
        Assert.Equal(2, videoElement?.Elements(VideoNs + "tag").Count());
    }

    [Fact]
    public async Task WritesNewsFieldsAndOptionalElements()
    {
        var news = new NewsEntry(
            publicationName: "Example",
            publicationLanguage: "en",
            publicationDate: new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
            title: "Launch")
        {
            Genres = "PressRelease",
            Keywords = "feature,launch",
            StockTickers = "EXM"
        };

        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"), news: new[] { news })
        });

        var doc = await WriteAsync(sitemap);
        var newsElement = doc.Root?.Element(SitemapNs + "url")?.Element(NewsNs + "news");

        Assert.Equal("Example", newsElement?.Element(NewsNs + "publication")?.Element(NewsNs + "name")?.Value);
        Assert.Equal("en", newsElement?.Element(NewsNs + "publication")?.Element(NewsNs + "language")?.Value);
        Assert.Equal("2026-02-01T00:00:00Z", newsElement?.Element(NewsNs + "publication_date")?.Value);
        Assert.Equal("Launch", newsElement?.Element(NewsNs + "title")?.Value);
        Assert.Equal("PressRelease", newsElement?.Element(NewsNs + "genres")?.Value);
        Assert.Equal("feature,launch", newsElement?.Element(NewsNs + "keywords")?.Value);
        Assert.Equal("EXM", newsElement?.Element(NewsNs + "stock_tickers")?.Value);
    }

    [Fact]
    public async Task WritesHreflangAlternates()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(
                new Uri("https://example.com/"),
                alternates: new[]
                {
                    new AlternateLink("en", new Uri("https://example.com/en"))
                })
        });

        var doc = await WriteAsync(sitemap);
        var link = doc.Root?.Element(SitemapNs + "url")?.Element(XhtmlNs + "link");

        Assert.Equal("alternate", link?.Attribute("rel")?.Value);
        Assert.Equal("en", link?.Attribute("hreflang")?.Value);
        Assert.Equal("https://example.com/en", link?.Attribute("href")?.Value);
    }

    [Fact]
    public async Task WritesSitemapIndex()
    {
        var index = new SitemapIndexModel(new[]
        {
            new SitemapIndexEntry(
                new Uri("https://example.com/sitemap.xml"),
                lastModified: new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero))
        });

        var doc = await WriteIndexAsync(index);
        Assert.Equal(SitemapNs + "sitemapindex", doc.Root?.Name);

        var sitemapElement = doc.Root?.Element(SitemapNs + "sitemap");
        Assert.Equal("https://example.com/sitemap.xml", sitemapElement?.Element(SitemapNs + "loc")?.Value);
        Assert.Equal("2026-01-01T12:00:00Z", sitemapElement?.Element(SitemapNs + "lastmod")?.Value);
    }

    [Fact]
    public async Task StrictValidationThrowsForRelativeUrls()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("/relative", UriKind.Relative))
        });

        var writer = new SitemapWriter();
        await using var stream = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentException>(() => writer.WriteAsync(sitemap, stream, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NonStrictValidationFiltersInvalidUrls()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/")),
            new SitemapUrl(new Uri("/relative", UriKind.Relative))
        });

        var doc = await WriteAsync(sitemap, new SitemapWriterOptions { StrictValidation = false });
        var urls = doc.Root?.Elements(SitemapNs + "url").ToList();

        Assert.NotNull(urls);
        Assert.Single(urls!);
    }

    [Fact]
    public async Task NonStrictValidationFiltersInvalidVideoEntries()
    {
        var invalidVideo = new VideoEntry(
            thumbnailLoc: new Uri("https://example.com/thumb.jpg"),
            title: "Intro",
            description: "Description");

        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"), videos: new[] { invalidVideo })
        });

        var doc = await WriteAsync(sitemap, new SitemapWriterOptions { StrictValidation = false });

        Assert.False(HasNamespace(doc.Root, "video", VideoNs));
        Assert.Null(doc.Root?.Element(SitemapNs + "url")?.Element(VideoNs + "video"));
    }

    [Fact]
    public async Task ThrowsWhenSitemapExceedsLimit()
    {
        var urls = new List<SitemapUrl>(50_001);
        for (var i = 0; i < 50_001; i++)
        {
            urls.Add(new SitemapUrl(new Uri($"https://example.com/{i}")));
        }

        var sitemap = new SitemapModel(urls);
        var writer = new SitemapWriter();
        await using var stream = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteAsync(sitemap, stream, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ThrowsWhenSitemapIndexExceedsLimit()
    {
        var entries = new List<SitemapIndexEntry>(50_001);
        for (var i = 0; i < 50_001; i++)
        {
            entries.Add(new SitemapIndexEntry(new Uri($"https://example.com/sitemap-{i}.xml")));
        }

        var index = new SitemapIndexModel(entries);
        var writer = new SitemapWriter();
        await using var stream = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteAsync(index, stream, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ToXmlAsyncMatchesWriterOutput()
    {
        var sitemap = new SitemapModel(new[]
        {
            new SitemapUrl(new Uri("https://example.com/"))
        });

        var writer = new SitemapWriter();
        await using var stream = new MemoryStream();
        await writer.WriteAsync(sitemap, stream, TestContext.Current.CancellationToken);
        var writerXml = Encoding.UTF8.GetString(stream.ToArray());

        var extensionXml = await sitemap.ToXmlAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(writerXml, extensionXml);
    }

    private static async Task<XDocument> WriteAsync(SitemapModel sitemap, SitemapWriterOptions? options = null)
    {
        var writer = new SitemapWriter(options);
        await using var stream = new MemoryStream();
        await writer.WriteAsync(sitemap, stream);
        var xml = Encoding.UTF8.GetString(stream.ToArray());
        return XDocument.Parse(xml);
    }

    private static async Task<XDocument> WriteIndexAsync(SitemapIndexModel index, SitemapWriterOptions? options = null)
    {
        var writer = new SitemapWriter(options);
        await using var stream = new MemoryStream();
        await writer.WriteAsync(index, stream);
        var xml = Encoding.UTF8.GetString(stream.ToArray());
        return XDocument.Parse(xml);
    }

    private static bool HasNamespace(XElement? root, string prefix, XNamespace ns)
    {
        return root?.Attributes().Any(a => a.IsNamespaceDeclaration && a.Name.LocalName == prefix && a.Value == ns.NamespaceName) == true;
    }
}
