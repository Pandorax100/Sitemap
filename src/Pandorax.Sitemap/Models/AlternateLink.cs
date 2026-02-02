using System;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents an alternate language or regional link for a URL.
/// </summary>
public sealed class AlternateLink
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlternateLink"/> class.
    /// </summary>
    /// <param name="hrefLang">The language and regional code.</param>
    /// <param name="href">The absolute URL for the alternate link.</param>
    public AlternateLink(string hrefLang, Uri href)
    {
        HrefLang = hrefLang ?? throw new ArgumentNullException(nameof(hrefLang));
        Href = href ?? throw new ArgumentNullException(nameof(href));
    }

    /// <summary>
    /// Gets the language and regional code.
    /// </summary>
    public string HrefLang { get; }

    /// <summary>
    /// Gets the absolute URL for the alternate link.
    /// </summary>
    public Uri Href { get; }
}
