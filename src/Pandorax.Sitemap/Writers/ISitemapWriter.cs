using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pandorax.Sitemap.Models;
using SitemapIndexModel = Pandorax.Sitemap.Models.SitemapIndex;
using SitemapModel = Pandorax.Sitemap.Models.Sitemap;

namespace Pandorax.Sitemap.Writers;

/// <summary>
/// Defines a writer for sitemap documents.
/// </summary>
public interface ISitemapWriter
{
    /// <summary>
    /// Writes a sitemap to the provided stream.
    /// </summary>
    /// <param name="sitemap">The sitemap to write.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteAsync(SitemapModel sitemap, Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a sitemap index to the provided stream.
    /// </summary>
    /// <param name="sitemapIndex">The sitemap index to write.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteAsync(SitemapIndexModel sitemapIndex, Stream stream, CancellationToken cancellationToken = default);
}
