using System.Diagnostics;
using System.IO;

namespace AetherForge.App.Services;

public static class FFmpegService
{
    private static readonly TimeSpan DefaultTimeout =
        TimeSpan.FromMinutes(10);

    private static string FFmpegPath =>
        Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "FFmpeg",
            "ffmpeg.exe");

    private static string FFprobePath =>
        Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "FFmpeg",
            "ffprobe.exe");

    public static bool IsAvailable()
    {
        return File.Exists(FFmpegPath) &&
               File.Exists(FFprobePath);
    }

    public static async Task<string> GetFFmpegVersionAsync()
    {
        return await RunProcessAsync(
            FFmpegPath,
            ["-version"]);
    }

    public static async Task<string> GetFFprobeVersionAsync()
    {
        return await RunProcessAsync(
            FFprobePath,
            ["-version"]);
    }

public static async Task<string> RunFFmpegAsync(
    IEnumerable<string> arguments,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    return await RunProcessAsync(
        FFmpegPath,
        arguments,
        timeout,
        cancellationToken);
}

public static async Task<string> RunFFprobeAsync(
    IEnumerable<string> arguments,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    return await RunProcessAsync(
        FFprobePath,
        arguments,
        timeout,
        cancellationToken);
}

    private static async Task<string> RunProcessAsync(
        string executablePath,
        IEnumerable<string> arguments,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(
                $"Executable not found: {executablePath}");
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = executablePath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = AppContext.BaseDirectory
        };

        startInfo.ArgumentList.Add("-protocol_whitelist");
        startInfo.ArgumentList.Add("file,pipe");

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = new()
        {
            StartInfo = startInfo
        };

        if (!process.Start())
        {
            throw new InvalidOperationException(
                "Unable to start FFmpeg process.");
        }

        Task<string> standardOutputTask =
            process.StandardOutput.ReadToEndAsync();

        Task<string> standardErrorTask =
            process.StandardError.ReadToEndAsync();

        TimeSpan effectiveTimeout =
            timeout ?? DefaultTimeout;

        using CancellationTokenSource timeoutSource =
            new(effectiveTimeout);

        using CancellationTokenSource linkedSource =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutSource.Token);

        try
        {
            await process.WaitForExitAsync(
                linkedSource.Token);
        }
        catch (OperationCanceledException)
        {
            TryKillProcess(process);

            await process.WaitForExitAsync(
                CancellationToken.None);

            await Task.WhenAll(
                standardOutputTask,
                standardErrorTask);

            cancellationToken.ThrowIfCancellationRequested();

            throw new TimeoutException(
                $"FFmpeg process exceeded the timeout of {effectiveTimeout}.");
        }

        string standardOutput =
            await standardOutputTask;

        string standardError =
            await standardErrorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(standardError)
                    ? $"FFmpeg exited with code {process.ExitCode}."
                    : standardError);
        }

        return !string.IsNullOrWhiteSpace(standardOutput)
            ? standardOutput
            : standardError;
    }

    private static void TryKillProcess(
        Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(
                    entireProcessTree: true);
            }
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
            // Il processo si è chiuso tra il controllo e Kill.
        }
    }
}