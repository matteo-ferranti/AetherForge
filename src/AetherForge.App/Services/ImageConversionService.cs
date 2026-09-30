using ImageMagick;

namespace AetherForge.App.Services;

public static class ImageConversionService
{
    public static void ConvertImage(
        string inputPath,
        string outputPath,
        string outputFormat)
    {
        using MagickImage image = new(inputPath);

        image.Format =
            GetMagickFormat(outputFormat);

        image.Write(outputPath);
    }

    public static bool HasTransparency(
        string inputPath)
    {
        using MagickImage image = new(inputPath);

        return image.HasAlpha &&
               !image.IsOpaque;
    }

    public static void ConvertImageWithWhiteBackground(
        string inputPath,
        string outputPath,
        string outputFormat)
    {
        using MagickImage image = new(inputPath);

        image.BackgroundColor =
            MagickColors.White;

        image.Alpha(
            AlphaOption.Remove);

        image.Format =
            GetMagickFormat(outputFormat);

        image.Write(outputPath);
    }

    public static void ResizeImage(
        string inputPath,
        string outputPath,
        uint width,
        uint height)
    {
        using MagickImage image = new(inputPath);

        image.Resize(
            width,
            height);

        image.Write(outputPath);
    }

    public static void ChangeQuality(
        string inputPath,
        string outputPath,
        uint quality)
    {
        using MagickImage image = new(inputPath);

        image.Quality =
            quality;

        image.Write(outputPath);
    }

    public static void RemoveMetadata(
        string inputPath,
        string outputPath)
    {
        using MagickImage image = new(inputPath);

        image.Strip();

        image.Write(outputPath);
    }

    private static MagickFormat GetMagickFormat(
        string format)
    {
        return format.ToUpperInvariant() switch
        {
            "JPG" => MagickFormat.Jpeg,
            "JPEG" => MagickFormat.Jpeg,
            "JPE" => MagickFormat.Jpeg,
            "JFIF" => MagickFormat.Jpeg,
            "PNG" => MagickFormat.Png,
            "WEBP" => MagickFormat.WebP,
            "BMP" => MagickFormat.Bmp,
            "TIFF" => MagickFormat.Tiff,
            "AVIF" => MagickFormat.Avif,

            _ => throw new NotSupportedException(
                $"Unsupported image format: {format}")
        };
    }
}