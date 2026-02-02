namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents a URL entry within a sitemap.
/// </summary>
public sealed class SitemapUrl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapUrl"/> class.
    /// </summary>
    /// <param name="loc">The absolute URL of the page.</param>
    /// <param name="lastModified">The last modified timestamp for the page.</param>
    /// <param name="changeFrequency">How frequently the page changes.</param>
    /// <param name="priority">The priority of this URL relative to other URLs on the site.</param>
    /// <param name="images">Image sitemap entries associated with this URL.</param>
    /// <param name="videos">Video sitemap entries associated with this URL.</param>
    /// <param name="news">News sitemap entries associated with this URL.</param>
    /// <param name="alternates">Alternate language or regional links for this URL.</param>
    public SitemapUrl(
        Uri loc,
        DateTimeOffset? lastModified = null,
        ChangeFrequency? changeFrequency = null,
        decimal? priority = null,
        IReadOnlyList<ImageEntry>? images = null,
        IReadOnlyList<VideoEntry>? videos = null,
        IReadOnlyList<NewsEntry>? news = null,
        IReadOnlyList<AlternateLink>? alternates = null)
    {
        Loc = loc ?? throw new ArgumentNullException(nameof(loc));
        LastModified = lastModified;
        ChangeFrequency = changeFrequency;
        Priority = priority;
        Images = images ?? Array.Empty<ImageEntry>();
        Videos = videos ?? Array.Empty<VideoEntry>();
        News = news ?? Array.Empty<NewsEntry>();
        Alternates = alternates ?? Array.Empty<AlternateLink>();
    }

    /// <summary>
    /// Gets the absolute URL of the page.
    /// </summary>
    public Uri Loc { get; }

    /// <summary>
    /// Gets the last modified timestamp for the page.
    /// </summary>
    public DateTimeOffset? LastModified { get; init; }

    /// <summary>
    /// Gets how frequently the page changes.
    /// </summary>
    public ChangeFrequency? ChangeFrequency { get; init; }

    /// <summary>
    /// Gets the priority of this URL relative to other URLs on the site.
    /// </summary>
    public decimal? Priority { get; init; }

    /// <summary>
    /// Gets the image sitemap entries associated with this URL.
    /// </summary>
    public IReadOnlyList<ImageEntry> Images
    {
        get;
        init => field = value ?? Array.Empty<ImageEntry>();
    }

    /// <summary>
    /// Gets the video sitemap entries associated with this URL.
    /// </summary>
    public IReadOnlyList<VideoEntry> Videos
    {
        get;
        init => field = value ?? Array.Empty<VideoEntry>();
    }

    /// <summary>
    /// Gets the news sitemap entries associated with this URL.
    /// </summary>
    public IReadOnlyList<NewsEntry> News
    {
        get;
        init => field = value ?? Array.Empty<NewsEntry>();
    }

    /// <summary>
    /// Gets the alternate language or regional links for this URL.
    /// </summary>
    public IReadOnlyList<AlternateLink> Alternates
    {
        get;
        init => field = value ?? Array.Empty<AlternateLink>();
    }
}
