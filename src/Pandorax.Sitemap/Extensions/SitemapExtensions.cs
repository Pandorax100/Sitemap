using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pandorax.Sitemap.Models;
using Pandorax.Sitemap.Options;
using Pandorax.Sitemap.Writers;
using SitemapIndexModel = Pandorax.Sitemap.Models.SitemapIndex;
using SitemapModel = Pandorax.Sitemap.Models.Sitemap;

namespace Pandorax.Sitemap.Extensions;

/// <summary>
/// Provides extension methods for generating sitemap XML.
/// </summary>
public static class SitemapExtensions
{
    /// <summary>
    /// Writes the sitemap to an XML string asynchronously.
    /// </summary>
    /// <param name="sitemap">The sitemap to serialize.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The sitemap XML string.</returns>
    public static async Task<string> ToXmlAsync(
        this SitemapModel sitemap,
        SitemapWriterOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        var writer = new SitemapWriter(options);
        await writer.WriteAsync(sitemap, stream, cancellationToken);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Writes the sitemap index to an XML string asynchronously.
    /// </summary>
    /// <param name="sitemapIndex">The sitemap index to serialize.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The sitemap index XML string.</returns>
    public static async Task<string> ToXmlAsync(
        this SitemapIndexModel sitemapIndex,
        SitemapWriterOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        var writer = new SitemapWriter(options);
        await writer.WriteAsync(sitemapIndex, stream, cancellationToken);
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
