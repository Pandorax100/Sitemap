namespace Pandorax.Sitemap.Options;

/// <summary>
/// Provides configuration options for writing sitemaps.
/// </summary>
public sealed class SitemapWriterOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether strict validation is enabled.
    /// When enabled, invalid entries will cause an exception to be thrown.
    /// </summary>
    public bool StrictValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the XML output should be indented.
    /// </summary>
    public bool UseIndentation { get; set; }

    /// <summary>
    /// Gets or sets the date/time format used for sitemap timestamps.
    /// </summary>
    public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ssZ";
}
