using System;
using Pandorax.Sitemap.Models;
using SitemapIndexModel = Pandorax.Sitemap.Models.SitemapIndex;
using SitemapModel = Pandorax.Sitemap.Models.Sitemap;

namespace Pandorax.Sitemap.Validation;

internal static class SitemapValidator
{
    private const int MaxSitemapEntries = 50_000;

    public static void ValidateSitemap(SitemapModel sitemap)
    {
        if (sitemap.Urls.Count > MaxSitemapEntries)
        {
            throw new InvalidOperationException($"A sitemap cannot contain more than {MaxSitemapEntries} URLs.");
        }

        foreach (var url in sitemap.Urls)
        {
            ValidateUrlEntry(url);
        }
    }

    public static void ValidateSitemapIndex(SitemapIndexModel sitemapIndex)
    {
        if (sitemapIndex.Sitemaps.Count > MaxSitemapEntries)
        {
            throw new InvalidOperationException($"A sitemap index cannot contain more than {MaxSitemapEntries} entries.");
        }

        foreach (var entry in sitemapIndex.Sitemaps)
        {
            if (entry is null)
            {
                throw new ArgumentException("Sitemap index entries cannot be null.", nameof(sitemapIndex));
            }

            if (entry.Loc is null || !entry.Loc.IsAbsoluteUri)
            {
                throw new ArgumentException("Sitemap index loc must be an absolute URI.", nameof(sitemapIndex));
            }
        }
    }

    private static void ValidateUrlEntry(UrlEntry url)
    {
        if (url is null)
        {
            throw new ArgumentException("Sitemap URLs cannot be null.", nameof(url));
        }

        if (url.Loc is null || !url.Loc.IsAbsoluteUri)
        {
            throw new ArgumentException("Sitemap URL loc must be an absolute URI.", nameof(url));
        }

        if (url.ChangeFrequency.HasValue && !Enum.IsDefined(typeof(ChangeFrequency), url.ChangeFrequency.Value))
        {
            throw new ArgumentException("Sitemap URL change frequency is invalid.", nameof(url));
        }

        if (url.Priority.HasValue && url.Priority.Value is < 0m or > 1m)
        {
            throw new ArgumentException("Sitemap URL priority must be between 0.0 and 1.0.", nameof(url));
        }

        foreach (var image in url.Images)
        {
            ExtensionValidator.ValidateImageEntry(image);
        }

        foreach (var video in url.Videos)
        {
            ExtensionValidator.ValidateVideoEntry(video);
        }

        foreach (var news in url.News)
        {
            ExtensionValidator.ValidateNewsEntry(news);
        }

        foreach (var alternate in url.Alternates)
        {
            ExtensionValidator.ValidateAlternateLink(alternate);
        }
    }
}
