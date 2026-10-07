using ImageMagick;

using System.IO;

namespace AetherForge.App.Services;

public static class ImageConversionService
{
    public static void ConvertImage(
        string inputPath,
        string outputPath,
        string outputFormat)
    {
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

        using MagickImage image = new(inputPath);

        image.Format =
            GetMagickFormat(outputFormat);

        WriteSafely(image, inputPath, outputPath);
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
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

        using MagickImage image = new(inputPath);

        image.BackgroundColor =
            MagickColors.White;

        image.Alpha(
            AlphaOption.Remove);

        image.Format =
            GetMagickFormat(outputFormat);

        WriteSafely(image, inputPath, outputPath);
    }

    public static void ResizeImage(
        string inputPath,
        string outputPath,
        uint width,
        uint height)
    {
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
        
        using MagickImage image = new(inputPath);

        image.Resize(
            width,
            height);

        WriteSafely(image, inputPath, outputPath);
    }

    public static void ChangeQuality(
        string inputPath,
        string outputPath,
        uint quality)
    {
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
        
        using MagickImage image = new(inputPath);

        image.Quality =
            quality;

        WriteSafely(image, inputPath, outputPath);
    }

    public static void RemoveMetadata(
        string inputPath,
        string outputPath)
    {
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
        
        using MagickImage image = new(inputPath);

        image.Strip();

        WriteSafely(image, inputPath, outputPath);
    }


    public static void RotateImage(
    string inputPath,
    string outputPath,
    int degrees)
{
    if (degrees is not 90 and not 180 and not 270)
    {
        throw new ArgumentOutOfRangeException(nameof(degrees));
    }

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

    using MagickImage image = new(inputPath);

    // Applica prima l'orientamento registrato nella foto.
    image.AutoOrient();

    image.Rotate(degrees);
    image.ResetPage();

    WriteSafely(image, inputPath, outputPath);
}

public static void FlipImage(
    string inputPath,
    string outputPath,
    bool horizontal)
{
    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
    
    using MagickImage image = new(inputPath);

    image.AutoOrient();

    if (horizontal)
    {
        // Scambia sinistra e destra.
        image.Flop();
    }
    else
    {
        // Scambia alto e basso.
        image.Flip();
    }

    image.ResetPage();

    WriteSafely(image, inputPath, outputPath);
}


    private static void WriteSafely(
    MagickImage image,
    string inputPath,
    string outputPath)
{
    inputPath = Path.GetFullPath(inputPath);
    outputPath = Path.GetFullPath(outputPath);

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

    string directory = Path.GetDirectoryName(outputPath)!;

    string temporaryPath = Path.Combine(
        directory,
        ".aetherforge-" +
        Guid.NewGuid().ToString("N") +
        Path.GetExtension(outputPath));

    try
    {
        image.Write(temporaryPath);

        if (!File.Exists(temporaryPath) ||
            new FileInfo(temporaryPath).Length == 0)
        {
            throw new InvalidDataException(
                LanguageService.Get("InvalidImageOutput"));
        }

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

        File.Move(
            temporaryPath,
            outputPath,
            overwrite: true);
    }
    finally
    {
        try
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
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