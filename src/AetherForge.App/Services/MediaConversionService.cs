using System.IO;
using System.Linq;

namespace AetherForge.App.Services;

public static class MediaConversionService
{
    public static async Task ConvertVideoAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        string inputExtension =
            Path.GetExtension(inputPath);

        string outputExtension =
            Path.GetExtension(outputPath);

        if (string.IsNullOrWhiteSpace(inputExtension))
        {
            throw new ArgumentException(
                "Input file has no extension.",
                nameof(inputPath));
        }

        if (string.IsNullOrWhiteSpace(outputExtension))
        {
            throw new ArgumentException(
                "Output file has no extension.",
                nameof(outputPath));
        }

        List<string> arguments =
        [
            "-hide_banner",
            "-nostdin",
            "-y",

            "-i",
            inputPath,

            outputPath
        ];

        await FFmpegService.RunFFmpegAsync(
            arguments,
            cancellationToken: cancellationToken);
    }

    public static async Task ExtractAudioAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        string outputExtension =
            Path.GetExtension(outputPath)
                .ToLowerInvariant();

        string[] supportedExtensions =
        [
            ".mp3",
            ".wav",
            ".flac",
            ".aac",
            ".m4a",
            ".ogg",
            ".opus"
        ];

        if (!supportedExtensions.Contains(
                outputExtension,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                $"Unsupported audio output format: {outputExtension}");
        }

        List<string> arguments =
        [
            "-hide_banner",
            "-nostdin",
            "-y",

            "-i",
            inputPath,

            "-map",
            "0:a:0",

            "-vn",

            outputPath
        ];

        await FFmpegService.RunFFmpegAsync(
            arguments,
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveAudioAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        List<string> arguments =
        [
            "-hide_banner",
            "-nostdin",
            "-y",

            "-i",
            inputPath,

            "-map",
            "0:v:0",

            "-c",
            "copy",

            "-an",

            outputPath
        ];

        await FFmpegService.RunFFmpegAsync(
            arguments,
            cancellationToken: cancellationToken);
    }

    public static async Task RemoveMetadataAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        List<string> arguments =
        [
            "-hide_banner",
            "-nostdin",
            "-y",

            "-i",
            inputPath,

            "-map",
            "0",

            "-map_metadata",
            "-1",

            "-c",
            "copy",

            outputPath
        ];

        await FFmpegService.RunFFmpegAsync(
            arguments,
            cancellationToken: cancellationToken);
    }

    public static async Task CreateGifAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        string outputExtension =
            Path.GetExtension(outputPath)
                .ToLowerInvariant();

        if (outputExtension != ".gif")
        {
            throw new NotSupportedException(
                "GIF output must use the .gif extension.");
        }

        List<string> arguments =
        [
            "-hide_banner",
            "-nostdin",
            "-y",

            "-i",
            inputPath,

            "-an",

            "-filter_complex",
            "[0:v]fps=12,scale='min(720,iw)':-2:flags=lanczos,split[s0][s1];" +
            "[s0]palettegen=max_colors=256[p];" +
            "[s1][p]paletteuse=dither=sierra2_4a",

            outputPath
        ];

        await FFmpegService.RunFFmpegAsync(
            arguments,
            cancellationToken: cancellationToken);
    }

    private static void ValidateInputFile(
        string inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentException(
                "Input path is required.",
                nameof(inputPath));
        }

        string fullPath =
            Path.GetFullPath(inputPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                "Input file was not found.",
                fullPath);
        }
    }

    private static void ValidateOutputPath(
        string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException(
                "Output path is required.",
                nameof(outputPath));
        }

        string fullPath =
            Path.GetFullPath(outputPath);

        string? directory =
            Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new ArgumentException(
                "Output directory is invalid.",
                nameof(outputPath));
        }

        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException(
                $"Output directory was not found: {directory}");
        }
    }
}