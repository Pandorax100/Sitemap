using System;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents an entry within a sitemap index.
/// </summary>
public sealed class SitemapIndexEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapIndexEntry"/> class.
    /// </summary>
    /// <param name="loc">The absolute URL of the sitemap.</param>
    /// <param name="lastModified">The last modified timestamp for the sitemap.</param>
    public SitemapIndexEntry(Uri loc, DateTimeOffset? lastModified = null)
    {
        Loc = loc ?? throw new ArgumentNullException(nameof(loc));
        LastModified = lastModified;
    }

    /// <summary>
    /// Gets the absolute URL of the sitemap.
    /// </summary>
    public Uri Loc { get; }

    /// <summary>
    /// Gets the last modified timestamp for the sitemap.
    /// </summary>
    public DateTimeOffset? LastModified { get; }
}
