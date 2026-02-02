namespace Pandorax.Sitemap.Models;

/// <summary>
/// Indicates how frequently the content at a URL is likely to change.
/// </summary>
public enum ChangeFrequency
{
    /// <summary>
    /// Content changes with every access.
    /// </summary>
    Always,

    /// <summary>
    /// Content changes hourly.
    /// </summary>
    Hourly,

    /// <summary>
    /// Content changes daily.
    /// </summary>
    Daily,

    /// <summary>
    /// Content changes weekly.
    /// </summary>
    Weekly,

    /// <summary>
    /// Content changes monthly.
    /// </summary>
    Monthly,

    /// <summary>
    /// Content changes yearly.
    /// </summary>
    Yearly,

    /// <summary>
    /// Content never changes.
    /// </summary>
    Never,
}
