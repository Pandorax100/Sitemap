using System;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents an image entry within an image sitemap extension.
/// </summary>
public sealed class ImageEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageEntry"/> class.
    /// </summary>
    /// <param name="loc">The absolute URL of the image.</param>
    public ImageEntry(Uri loc)
    {
        Loc = loc ?? throw new ArgumentNullException(nameof(loc));
    }

    /// <summary>
    /// Gets the absolute URL of the image.
    /// </summary>
    public Uri Loc { get; }

    /// <summary>
    /// Gets or sets the image caption.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the image title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the image license URL.
    /// </summary>
    public Uri? License { get; set; }

    /// <summary>
    /// Gets or sets the geographic location of the image.
    /// </summary>
    public string? GeoLocation { get; set; }
}
