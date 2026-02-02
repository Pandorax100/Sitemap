using System;

namespace Pandorax.Sitemap.Models;

/// <summary>
/// Represents a news entry within a news sitemap extension.
/// </summary>
public sealed class NewsEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewsEntry"/> class.
    /// </summary>
    /// <param name="publicationName">The name of the publication.</param>
    /// <param name="publicationLanguage">The language of the publication.</param>
    /// <param name="publicationDate">The publication date.</param>
    /// <param name="title">The title of the news item.</param>
    public NewsEntry(string publicationName, string publicationLanguage, DateTimeOffset publicationDate, string title)
    {
        PublicationName = publicationName ?? throw new ArgumentNullException(nameof(publicationName));
        PublicationLanguage = publicationLanguage ?? throw new ArgumentNullException(nameof(publicationLanguage));
        PublicationDate = publicationDate;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    /// <summary>
    /// Gets the name of the publication.
    /// </summary>
    public string PublicationName { get; }

    /// <summary>
    /// Gets the language of the publication.
    /// </summary>
    public string PublicationLanguage { get; }

    /// <summary>
    /// Gets the publication date.
    /// </summary>
    public DateTimeOffset PublicationDate { get; }

    /// <summary>
    /// Gets the title of the news item.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets or sets the genres for the news item.
    /// </summary>
    public string? Genres { get; set; }

    /// <summary>
    /// Gets or sets the keywords for the news item.
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Gets or sets the stock tickers for the news item.
    /// </summary>
    public string? StockTickers { get; set; }
}
