using System.Collections.Generic;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents a sitemap containing a collection of URL entries.
/// </summary>
public sealed class Sitemap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sitemap"/> class.
    /// </summary>
    /// <param name="urls">The URL entries contained in the sitemap.</param>
    public Sitemap(IReadOnlyList<UrlEntry> urls)
    {
        Urls = urls ?? throw new System.ArgumentNullException(nameof(urls));
    }

    /// <summary>
    /// Gets the URL entries contained in the sitemap.
    /// </summary>
    public IReadOnlyList<UrlEntry> Urls { get; }
}
