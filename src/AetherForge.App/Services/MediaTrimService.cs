using System.Globalization;
using System.IO;

namespace AetherForge.App.Services;

public static class MediaTrimService
{
    public static string[] GetOutputFormats(bool audioOnly)
    {
        return audioOnly
            ? ["MP3", "WAV", "FLAC", "AAC", "M4A", "OGG", "OPUS"]
            : ["MP4", "MKV", "MOV", "WEBM"];
    }

    public static async Task<TimeSpan> GetDurationAsync(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        if (!FFmpegService.IsAvailable())
        {
            throw new FileNotFoundException(
                LanguageService.Get("MediaToolsMissing"));
        }

        inputPath = Path.GetFullPath(inputPath);

        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException(
                LanguageService.Get("MediaDurationUnavailable"));
        }

        string result = await FFmpegService.RunFFprobeAsync(
            [
                "-v", "error",
                "-show_entries", "format=duration",
                "-of", "default=noprint_wrappers=1:nokey=1",
                inputPath
            ],
            timeout: TimeSpan.FromSeconds(30),
            cancellationToken: cancellationToken);

        if (!double.TryParse(
                result.Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double seconds) ||
            !double.IsFinite(seconds) ||
            seconds <= 0 ||
            seconds >= TimeSpan.MaxValue.TotalSeconds)
        {
            throw new InvalidOperationException(
                LanguageService.Get("MediaDurationUnavailable"));
        }

        return TimeSpan.FromSeconds(seconds);
    }

    public static async Task TrimAsync(
        string inputPath,
        string outputPath,
        TimeSpan start,
        TimeSpan end,
        bool audioOnly,
        CancellationToken cancellationToken = default)
    {
        FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
        
        inputPath = Path.GetFullPath(inputPath);
        outputPath = Path.GetFullPath(outputPath);

        if (string.Equals(
                inputPath,
                outputPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("MediaTrimSamePath"));
        }

        string outputFormat = Path.GetExtension(outputPath)
            .TrimStart('.')
            .ToUpperInvariant();

        if (!GetOutputFormats(audioOnly).Contains(outputFormat))
        {
            throw new NotSupportedException(
                LanguageService.Get("MediaTrimUnsupportedOutput"));
        }

        TimeSpan duration = await GetDurationAsync(
            inputPath,
            cancellationToken);

        if (start < TimeSpan.Zero ||
            end <= start ||
            end > duration)
        {
            throw new ArgumentException(
                LanguageService.Get("MediaTrimInvalidRange"));
        }

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(temporaryDirectory);

            string temporaryOutput = Path.Combine(
                temporaryDirectory,
                "segment." + outputFormat.ToLowerInvariant());

            List<string> arguments =
            [
                "-hide_banner",
                "-nostdin",
                "-y",

                "-ss",
                start.TotalSeconds.ToString(
                    "0.#######",
                    CultureInfo.InvariantCulture),

                "-i",
                inputPath,

                "-t",
                (end - start).TotalSeconds.ToString(
                    "0.#######",
                    CultureInfo.InvariantCulture)
            ];

            if (audioOnly)
            {
                arguments.AddRange(
                [
                    "-map", "0:a:0",
                    "-vn",
                    "-sn",
                    "-dn"
                ]);
            }
            else
            {
                arguments.AddRange(
                [
                    "-map", "0:v:0",
                    "-map", "0:a:0?",
                    "-sn",
                    "-dn",
                    "-vf", "pad=ceil(iw/2)*2:ceil(ih/2)*2",
                    "-pix_fmt", "yuv420p"
                ]);

                if (outputFormat == "WEBM")
                {
                    arguments.AddRange(
                    [
                        "-c:v", "libvpx-vp9",
                        "-crf", "30",
                        "-b:v", "0",
                        "-c:a", "libopus"
                    ]);
                }
                else
                {
                    arguments.AddRange(
                    [
                        "-c:v", "libx264",
                        "-crf", "18",
                        "-preset", "medium",
                        "-c:a", "aac"
                    ]);

                    if (outputFormat is "MP4" or "MOV")
                    {
                        arguments.AddRange(
                            ["-movflags", "+faststart"]);
                    }
                }
            }

            arguments.Add(temporaryOutput);

            await FFmpegService.RunFFmpegAsync(
                arguments,
                cancellationToken: cancellationToken);

            if (!File.Exists(temporaryOutput) ||
                new FileInfo(temporaryOutput).Length == 0)
            {
                throw new InvalidOperationException(
                    LanguageService.Get("MediaTrimFailed"));
            }

            await GetDurationAsync(
                temporaryOutput,
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

await SafeOutputService.CopyFileAsync(
    inputPath,
    temporaryOutput,
    outputPath,
    cancellationToken);
        }
        finally
        {
            try
            {
                if (Directory.Exists(temporaryDirectory))
                {
                    Directory.Delete(
                        temporaryDirectory,
                        recursive: true);
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
}