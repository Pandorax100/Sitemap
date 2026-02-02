using System;
using System.Collections.Generic;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents a video entry within a video sitemap extension.
/// </summary>
public sealed class VideoEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoEntry"/> class.
    /// </summary>
    /// <param name="thumbnailLoc">The absolute URL of the video thumbnail.</param>
    /// <param name="title">The title of the video.</param>
    /// <param name="description">The description of the video.</param>
    public VideoEntry(Uri thumbnailLoc, string title, string description)
    {
        ThumbnailLoc = thumbnailLoc ?? throw new ArgumentNullException(nameof(thumbnailLoc));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    /// <summary>
    /// Gets the absolute URL of the video thumbnail.
    /// </summary>
    public Uri ThumbnailLoc { get; }

    /// <summary>
    /// Gets the title of the video.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the description of the video.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets or sets the absolute URL of the video content.
    /// </summary>
    public Uri? ContentLoc { get; set; }

    /// <summary>
    /// Gets or sets the absolute URL of the video player.
    /// </summary>
    public Uri? PlayerLoc { get; set; }

    /// <summary>
    /// Gets or sets the video duration.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the publication date of the video.
    /// </summary>
    public DateTimeOffset? PublicationDate { get; set; }

    /// <summary>
    /// Gets or sets whether the video is family friendly.
    /// </summary>
    public bool? FamilyFriendly { get; set; }

    /// <summary>
    /// Gets or sets the category of the video.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the uploader name for the video.
    /// </summary>
    public string? Uploader { get; set; }

    /// <summary>
    /// Gets or sets the uploader info URL.
    /// </summary>
    public Uri? UploaderInfo { get; set; }

    /// <summary>
    /// Gets or sets the number of views for the video.
    /// </summary>
    public long? ViewCount { get; set; }

    /// <summary>
    /// Gets or sets the tags for the video.
    /// </summary>
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
}
