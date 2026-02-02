using System.Collections.Generic;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents a sitemap index containing references to other sitemaps.
/// </summary>
public sealed class SitemapIndex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapIndex"/> class.
    /// </summary>
    /// <param name="sitemaps">The sitemap entries contained in the index.</param>
    public SitemapIndex(IReadOnlyList<SitemapIndexEntry> sitemaps)
    {
        Sitemaps = sitemaps ?? throw new System.ArgumentNullException(nameof(sitemaps));
    }

    /// <summary>
    /// Gets the sitemap entries contained in the index.
    /// </summary>
    public IReadOnlyList<SitemapIndexEntry> Sitemaps { get; }
}
