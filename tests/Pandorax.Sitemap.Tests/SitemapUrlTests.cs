using System;
using Pandorax.Sitemap.Models;
using Xunit;

namespace Pandorax.Sitemap.Tests;

public sealed class SitemapUrlTests
{
    [Fact]
    public void ConstructorNullListsDefaultToEmpty()
    {
        var url = new SitemapUrl(
            loc: new Uri("https://example.com"),
            images: null,
            videos: null,
            news: null,
            alternates: null);

        Assert.NotNull(url.Images);
        Assert.NotNull(url.Videos);
        Assert.NotNull(url.News);
        Assert.NotNull(url.Alternates);

        Assert.Empty(url.Images);
        Assert.Empty(url.Videos);
        Assert.Empty(url.News);
        Assert.Empty(url.Alternates);
    }

    [Fact]
    public void InitNullListsDefaultToEmpty()
    {
        var url = new SitemapUrl(new Uri("https://example.com"))
        {
            Images = null,
            Videos = null,
            News = null,
            Alternates = null,
        };

        Assert.NotNull(url.Images);
        Assert.NotNull(url.Videos);
        Assert.NotNull(url.News);
        Assert.NotNull(url.Alternates);

        Assert.Empty(url.Images);
        Assert.Empty(url.Videos);
        Assert.Empty(url.News);
        Assert.Empty(url.Alternates);
    }
}
