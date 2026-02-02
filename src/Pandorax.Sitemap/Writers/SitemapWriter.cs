using System.Globalization;
using System.Text;
using System.Xml;
using Pandorax.Sitemap.Models;
using Pandorax.Sitemap.Options;
using Pandorax.Sitemap.Validation;
using SitemapIndexModel = Pandorax.Sitemap.Models.SitemapIndex;
using SitemapModel = Pandorax.Sitemap.Models.Sitemap;

namespace Pandorax.Sitemap.Writers;

/// <summary>
/// Writes sitemap and sitemap index documents using <see cref="XmlWriter"/>.
/// </summary>
public sealed class SitemapWriter : ISitemapWriter
{
    private const int MaxSitemapEntries = 50_000;
    private const string SitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private const string ImageNamespace = "http://www.google.com/schemas/sitemap-image/1.1";
    private const string VideoNamespace = "http://www.google.com/schemas/sitemap-video/1.1";
    private const string NewsNamespace = "http://www.google.com/schemas/sitemap-news/0.9";
    private const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";

    private readonly SitemapWriterOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapWriter"/> class.
    /// </summary>
    /// <param name="options">The writer options to use.</param>
    public SitemapWriter(SitemapWriterOptions? options = null)
    {
        _options = options ?? new SitemapWriterOptions();
    }

    /// <inheritdoc />
    public async Task WriteAsync(SitemapModel sitemap, Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sitemap);
        ArgumentNullException.ThrowIfNull(stream);

        var urls = PrepareUrls(sitemap);
        if (urls.Count > MaxSitemapEntries)
        {
            throw new InvalidOperationException($"A sitemap cannot contain more than {MaxSitemapEntries} URLs.");
        }

        var namespaces = NamespaceFlags.FromUrls(urls);

        using var writer = CreateXmlWriter(stream);
        await writer.WriteStartDocumentAsync();
        writer.WriteStartElement("urlset", SitemapNamespace);
        WriteNamespaces(writer, namespaces);

        foreach (var url in urls)
        {
            cancellationToken.ThrowIfCancellationRequested();
            WriteUrl(writer, url, namespaces);
        }

        await writer.WriteEndElementAsync();
        await writer.WriteEndDocumentAsync();
        await writer.FlushAsync();
    }

    /// <inheritdoc />
    public async Task WriteAsync(SitemapIndexModel sitemapIndex, Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sitemapIndex);
        ArgumentNullException.ThrowIfNull(stream);

        var entries = PrepareIndexEntries(sitemapIndex);
        if (entries.Count > MaxSitemapEntries)
        {
            throw new InvalidOperationException($"A sitemap index cannot contain more than {MaxSitemapEntries} entries.");
        }

        using var writer = CreateXmlWriter(stream);
        await writer.WriteStartDocumentAsync();
        writer.WriteStartElement("sitemapindex", SitemapNamespace);

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            WriteSitemapIndexEntry(writer, entry);
        }

        await writer.WriteEndElementAsync();
        await writer.WriteEndDocumentAsync();
        await writer.FlushAsync();
    }
    private static IReadOnlyList<UrlEntry> FilterUrls(IReadOnlyList<UrlEntry> urls)
    {
        if (urls.Count == 0)
        {
            return Array.Empty<UrlEntry>();
        }

        var filtered = new List<UrlEntry>(urls.Count);
        foreach (var url in urls)
        {
            if (url is null || url.Loc is null || !url.Loc.IsAbsoluteUri)
            {
                continue;
            }

            var changeFrequency = url.ChangeFrequency.HasValue && Enum.IsDefined(typeof(ChangeFrequency), url.ChangeFrequency.Value)
                ? url.ChangeFrequency
                : null;

            var priority = url.Priority.HasValue && url.Priority.Value is >= 0m and <= 1m
                ? url.Priority
                : null;

            var images = Filter(url.Images, ExtensionValidator.IsValidImageEntry);
            var videos = Filter(url.Videos, ExtensionValidator.IsValidVideoEntry);
            var news = Filter(url.News, ExtensionValidator.IsValidNewsEntry);
            var alternates = Filter(url.Alternates, ExtensionValidator.IsValidAlternateLink);

            filtered.Add(new UrlEntry(url.Loc, url.LastModified, changeFrequency, priority, images, videos, news, alternates));
        }

        return filtered;
    }

    private static IReadOnlyList<SitemapIndexEntry> FilterIndexEntries(IReadOnlyList<SitemapIndexEntry> entries)
    {
        if (entries.Count == 0)
        {
            return Array.Empty<SitemapIndexEntry>();
        }

        var filtered = new List<SitemapIndexEntry>(entries.Count);
        foreach (var entry in entries)
        {
            if (entry is null || entry.Loc is null || !entry.Loc.IsAbsoluteUri)
            {
                continue;
            }

            filtered.Add(entry);
        }

        return filtered;
    }

    private static IReadOnlyList<T> Filter<T>(IReadOnlyList<T> items, Func<T, bool> isValid)
    {
        if (items.Count == 0)
        {
            return Array.Empty<T>();
        }

        var filtered = new List<T>(items.Count);
        foreach (var item in items)
        {
            if (item is not null && isValid(item))
            {
                filtered.Add(item);
            }
        }

        return filtered.Count == 0 ? Array.Empty<T>() : filtered;
    }

    private static string ToChangeFrequencyValue(ChangeFrequency frequency)
    {
        return frequency.ToString().ToLowerInvariant();
    }

    private IReadOnlyList<UrlEntry> PrepareUrls(SitemapModel sitemap)
    {
        if (_options.StrictValidation)
        {
            SitemapValidator.ValidateSitemap(sitemap);
            return sitemap.Urls;
        }

        return FilterUrls(sitemap.Urls);
    }

    private IReadOnlyList<SitemapIndexEntry> PrepareIndexEntries(SitemapIndexModel sitemapIndex)
    {
        if (_options.StrictValidation)
        {
            SitemapValidator.ValidateSitemapIndex(sitemapIndex);
            return sitemapIndex.Sitemaps;
        }

        return FilterIndexEntries(sitemapIndex.Sitemaps);
    }

    private XmlWriter CreateXmlWriter(Stream stream)
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = _options.UseIndentation,
            CloseOutput = false,
        };

        return XmlWriter.Create(stream, settings);
    }

    private void WriteNamespaces(XmlWriter writer, NamespaceFlags namespaces)
    {
        if (namespaces.HasImage)
        {
            writer.WriteAttributeString("xmlns", "image", null, ImageNamespace);
        }

        if (namespaces.HasVideo)
        {
            writer.WriteAttributeString("xmlns", "video", null, VideoNamespace);
        }

        if (namespaces.HasNews)
        {
            writer.WriteAttributeString("xmlns", "news", null, NewsNamespace);
        }

        if (namespaces.HasXhtml)
        {
            writer.WriteAttributeString("xmlns", "xhtml", null, XhtmlNamespace);
        }
    }

    private void WriteSitemapIndexEntry(XmlWriter writer, SitemapIndexEntry entry)
    {
        writer.WriteStartElement("sitemap", SitemapNamespace);
        writer.WriteElementString("loc", SitemapNamespace, entry.Loc.AbsoluteUri);

        if (entry.LastModified.HasValue)
        {
            writer.WriteElementString("lastmod", SitemapNamespace, FormatDateTime(entry.LastModified.Value));
        }

        writer.WriteEndElement();
    }

    private void WriteUrl(XmlWriter writer, UrlEntry url, NamespaceFlags namespaces)
    {
        writer.WriteStartElement("url", SitemapNamespace);
        writer.WriteElementString("loc", SitemapNamespace, url.Loc.AbsoluteUri);

        if (url.LastModified.HasValue)
        {
            writer.WriteElementString("lastmod", SitemapNamespace, FormatDateTime(url.LastModified.Value));
        }

        if (url.ChangeFrequency.HasValue)
        {
            writer.WriteElementString("changefreq", SitemapNamespace, ToChangeFrequencyValue(url.ChangeFrequency.Value));
        }

        if (url.Priority.HasValue)
        {
            writer.WriteElementString("priority", SitemapNamespace, url.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture));
        }

        if (namespaces.HasImage)
        {
            foreach (var image in url.Images)
            {
                WriteImage(writer, image);
            }
        }

        if (namespaces.HasVideo)
        {
            foreach (var video in url.Videos)
            {
                WriteVideo(writer, video);
            }
        }

        if (namespaces.HasNews)
        {
            foreach (var news in url.News)
            {
                WriteNews(writer, news);
            }
        }

        if (namespaces.HasXhtml)
        {
            foreach (var alternate in url.Alternates)
            {
                WriteAlternate(writer, alternate);
            }
        }

        writer.WriteEndElement();
    }

    private void WriteImage(XmlWriter writer, ImageEntry image)
    {
        writer.WriteStartElement("image", "image", ImageNamespace);
        writer.WriteElementString("image", "loc", ImageNamespace, image.Loc.AbsoluteUri);

        if (!string.IsNullOrWhiteSpace(image.Caption))
        {
            writer.WriteElementString("image", "caption", ImageNamespace, image.Caption);
        }

        if (!string.IsNullOrWhiteSpace(image.Title))
        {
            writer.WriteElementString("image", "title", ImageNamespace, image.Title);
        }

        if (!string.IsNullOrWhiteSpace(image.GeoLocation))
        {
            writer.WriteElementString("image", "geo_location", ImageNamespace, image.GeoLocation);
        }

        if (image.License is not null)
        {
            writer.WriteElementString("image", "license", ImageNamespace, image.License.AbsoluteUri);
        }

        writer.WriteEndElement();
    }

    private void WriteVideo(XmlWriter writer, VideoEntry video)
    {
        writer.WriteStartElement("video", "video", VideoNamespace);
        writer.WriteElementString("video", "thumbnail_loc", VideoNamespace, video.ThumbnailLoc.AbsoluteUri);
        writer.WriteElementString("video", "title", VideoNamespace, video.Title);
        writer.WriteElementString("video", "description", VideoNamespace, video.Description);

        if (video.ContentLoc is not null)
        {
            writer.WriteElementString("video", "content_loc", VideoNamespace, video.ContentLoc.AbsoluteUri);
        }

        if (video.PlayerLoc is not null)
        {
            writer.WriteElementString("video", "player_loc", VideoNamespace, video.PlayerLoc.AbsoluteUri);
        }

        if (video.Duration.HasValue)
        {
            var durationSeconds = (int)Math.Round(video.Duration.Value.TotalSeconds, MidpointRounding.AwayFromZero);
            writer.WriteElementString("video", "duration", VideoNamespace, durationSeconds.ToString(CultureInfo.InvariantCulture));
        }

        if (video.PublicationDate.HasValue)
        {
            writer.WriteElementString("video", "publication_date", VideoNamespace, FormatDateTime(video.PublicationDate.Value));
        }

        if (video.FamilyFriendly.HasValue)
        {
            writer.WriteElementString("video", "family_friendly", VideoNamespace, video.FamilyFriendly.Value ? "yes" : "no");
        }

        if (!string.IsNullOrWhiteSpace(video.Category))
        {
            writer.WriteElementString("video", "category", VideoNamespace, video.Category);
        }

        if (!string.IsNullOrWhiteSpace(video.Uploader))
        {
            writer.WriteStartElement("video", "uploader", VideoNamespace);
            if (video.UploaderInfo is not null)
            {
                writer.WriteAttributeString("info", video.UploaderInfo.AbsoluteUri);
            }

            writer.WriteString(video.Uploader);
            writer.WriteEndElement();
        }

        if (video.ViewCount.HasValue)
        {
            writer.WriteElementString("video", "view_count", VideoNamespace, video.ViewCount.Value.ToString(CultureInfo.InvariantCulture));
        }

        var tags = video.Tags ?? Array.Empty<string>();
        foreach (var tag in tags)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                writer.WriteElementString("video", "tag", VideoNamespace, tag);
            }
        }

        writer.WriteEndElement();
    }

    private void WriteNews(XmlWriter writer, NewsEntry news)
    {
        writer.WriteStartElement("news", "news", NewsNamespace);

        writer.WriteStartElement("news", "publication", NewsNamespace);
        writer.WriteElementString("news", "name", NewsNamespace, news.PublicationName);
        writer.WriteElementString("news", "language", NewsNamespace, news.PublicationLanguage);
        writer.WriteEndElement();

        writer.WriteElementString("news", "publication_date", NewsNamespace, FormatDateTime(news.PublicationDate));
        writer.WriteElementString("news", "title", NewsNamespace, news.Title);

        if (!string.IsNullOrWhiteSpace(news.Genres))
        {
            writer.WriteElementString("news", "genres", NewsNamespace, news.Genres);
        }

        if (!string.IsNullOrWhiteSpace(news.Keywords))
        {
            writer.WriteElementString("news", "keywords", NewsNamespace, news.Keywords);
        }

        if (!string.IsNullOrWhiteSpace(news.StockTickers))
        {
            writer.WriteElementString("news", "stock_tickers", NewsNamespace, news.StockTickers);
        }

        writer.WriteEndElement();
    }

    private void WriteAlternate(XmlWriter writer, AlternateLink alternate)
    {
        writer.WriteStartElement("xhtml", "link", XhtmlNamespace);
        writer.WriteAttributeString("rel", "alternate");
        writer.WriteAttributeString("hreflang", alternate.HrefLang);
        writer.WriteAttributeString("href", alternate.Href.AbsoluteUri);
        writer.WriteEndElement();
    }

    private string FormatDateTime(DateTimeOffset value)
    {
        return value.ToUniversalTime().ToString(_options.DateTimeFormat, CultureInfo.InvariantCulture);
    }

    private struct NamespaceFlags
    {
        public bool HasImage;
        public bool HasVideo;
        public bool HasNews;
        public bool HasXhtml;

        public static NamespaceFlags FromUrls(IReadOnlyList<UrlEntry> urls)
        {
            var flags = default(NamespaceFlags);
            foreach (var url in urls)
            {
                if (!flags.HasImage && url.Images.Count > 0)
                {
                    flags.HasImage = true;
                }

                if (!flags.HasVideo && url.Videos.Count > 0)
                {
                    flags.HasVideo = true;
                }

                if (!flags.HasNews && url.News.Count > 0)
                {
                    flags.HasNews = true;
                }

                if (!flags.HasXhtml && url.Alternates.Count > 0)
                {
                    flags.HasXhtml = true;
                }

                if (flags.HasImage && flags.HasVideo && flags.HasNews && flags.HasXhtml)
                {
                    break;
                }
            }

            return flags;
        }
    }
}
