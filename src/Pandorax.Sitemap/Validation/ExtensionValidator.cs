using System;
using Pandorax.Sitemap.Models;

namespace Pandorax.Sitemap.Validation;

internal static class ExtensionValidator
{
    public static void ValidateImageEntry(ImageEntry image)
    {
        if (!IsValidImageEntry(image))
        {
            throw new ArgumentException("Image entries must have an absolute loc URI.", nameof(image));
        }
    }

    public static void ValidateVideoEntry(VideoEntry video)
    {
        if (!IsValidVideoEntry(video))
        {
            throw new ArgumentException("Video entries must contain required fields and absolute URLs.", nameof(video));
        }
    }

    public static void ValidateNewsEntry(NewsEntry news)
    {
        if (!IsValidNewsEntry(news))
        {
            throw new ArgumentException("News entries must contain required fields.", nameof(news));
        }
    }

    public static void ValidateAlternateLink(AlternateLink alternate)
    {
        if (!IsValidAlternateLink(alternate))
        {
            throw new ArgumentException("Alternate links must have a hreflang and absolute href.", nameof(alternate));
        }
    }

    public static bool IsValidImageEntry(ImageEntry? image)
    {
        if (image is null || !IsAbsolute(image.Loc))
        {
            return false;
        }

        if (image.License is not null && !IsAbsolute(image.License))
        {
            return false;
        }

        return true;
    }

    public static bool IsValidVideoEntry(VideoEntry? video)
    {
        if (video is null)
        {
            return false;
        }

        if (!IsAbsolute(video.ThumbnailLoc))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(video.Title) || string.IsNullOrWhiteSpace(video.Description))
        {
            return false;
        }

        if (video.ContentLoc is null && video.PlayerLoc is null)
        {
            return false;
        }

        if (video.ContentLoc is not null && !IsAbsolute(video.ContentLoc))
        {
            return false;
        }

        if (video.PlayerLoc is not null && !IsAbsolute(video.PlayerLoc))
        {
            return false;
        }

        if (video.UploaderInfo is not null && !IsAbsolute(video.UploaderInfo))
        {
            return false;
        }

        return true;
    }

    public static bool IsValidNewsEntry(NewsEntry? news)
    {
        if (news is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(news.PublicationName)
            || string.IsNullOrWhiteSpace(news.PublicationLanguage)
            || string.IsNullOrWhiteSpace(news.Title))
        {
            return false;
        }

        if (news.PublicationDate == default)
        {
            return false;
        }

        return true;
    }

    public static bool IsValidAlternateLink(AlternateLink? alternate)
    {
        if (alternate is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(alternate.HrefLang))
        {
            return false;
        }

        return IsAbsolute(alternate.Href);
    }

    private static bool IsAbsolute(Uri? uri)
    {
        return uri is not null && uri.IsAbsoluteUri;
    }
}
