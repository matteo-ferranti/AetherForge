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

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

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

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
    }

    public static async Task ExtractAudioAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

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

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
    }

    public static async Task RemoveAudioAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

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

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
    }

    public static async Task RemoveMetadataAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

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

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
    }

    public static async Task CreateGifAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInputFile(inputPath);
        ValidateOutputPath(outputPath);

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

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

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
    }


public static async Task ConvertAudioAsync(
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    ValidateInputFile(inputPath);
    ValidateOutputPath(outputPath);

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

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

        "-vn",

        outputPath
    ];

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
}


public static async Task ChangeAudioQualityAsync(
    string inputPath,
    string outputPath,
    int bitrateKbps,
    CancellationToken cancellationToken = default)
{
    ValidateInputFile(inputPath);
    ValidateOutputPath(outputPath);

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

    int[] supportedBitrates =
    [
        64, 96, 128, 160, 192, 256, 320
    ];

    if (!supportedBitrates.Contains(bitrateKbps))
    {
        throw new ArgumentOutOfRangeException(
            nameof(bitrateKbps),
            "Supported bitrates: 64, 96, 128, 160, 192, 256, 320 kbps.");
    }

    string inputExtension =
        Path.GetExtension(inputPath).ToLowerInvariant();

    string outputExtension =
        Path.GetExtension(outputPath).ToLowerInvariant();

    if (inputExtension != outputExtension)
    {
        throw new NotSupportedException(
            "Changing audio quality must preserve the file format.");
    }

    string codec = outputExtension switch
    {
        ".mp3" => "libmp3lame",
        ".aac" or ".m4a" => "aac",
        ".ogg" => "libvorbis",
        ".opus" => "libopus",

        _ => throw new NotSupportedException(
            $"Changing audio quality is not supported for: {outputExtension}")
    };

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

        "-c:a",
        codec,

        "-b:a",
        bitrateKbps.ToString(
            System.Globalization.CultureInfo.InvariantCulture) + "k",

        outputPath
    ];

await RunSafelyAsync(
    arguments,
    inputPath,
    outputPath,
    cancellationToken);
}

    private static async Task RunSafelyAsync(
    List<string> arguments,
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

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

    List<string> safeArguments = [.. arguments];

    // Nei sette metodi il percorso di uscita è l'ultimo argomento.
    if (safeArguments.Count == 0 ||
        !string.Equals(
            Path.GetFullPath(safeArguments[^1]),
            outputPath,
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            LanguageService.Get("InvalidMediaOutput"));
    }

    safeArguments[^1] = temporaryPath;

    try
    {
        await FFmpegService.RunFFmpegAsync(
            safeArguments,
            cancellationToken: cancellationToken);

        if (!File.Exists(temporaryPath) ||
            new FileInfo(temporaryPath).Length == 0)
        {
            throw new InvalidDataException(
                LanguageService.Get("InvalidMediaOutput"));
        }

        cancellationToken.ThrowIfCancellationRequested();

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