using System.IO;
using System.Text;

namespace AetherForge.App.Services;

internal static class SafeOutputService
{
    public static Task WriteTextAsync(
        string inputPath,
        string outputPath,
        string text,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        return WriteAsync(
            inputPath,
            outputPath,
            (temporaryPath, token) =>
                File.WriteAllTextAsync(
                    temporaryPath,
                    text,
                    encoding,
                    token),
            cancellationToken);
    }

    public static Task WriteBytesAsync(
        string inputPath,
        string outputPath,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        return WriteAsync(
            inputPath,
            outputPath,
            (temporaryPath, token) =>
                File.WriteAllBytesAsync(
                    temporaryPath,
                    bytes,
                    token),
            cancellationToken);
    }

    public static Task CopyFileAsync(
        string inputPath,
        string generatedFilePath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        return WriteAsync(
            inputPath,
            outputPath,
            async (temporaryPath, token) =>
            {
                await using var source = new FileStream(
                    generatedFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    65536,
                    useAsync: true);

                await using var destination = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    65536,
                    useAsync: true);

                await source.CopyToAsync(destination, token);
                await destination.FlushAsync(token);
            },
            cancellationToken);
    }


    public static Task CopyFileAsync(
    IReadOnlyList<string> inputPaths,
    string generatedFilePath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    if (inputPaths.Count == 0)
    {
        throw new ArgumentException(nameof(inputPaths));
    }

    string[] protectedPaths = inputPaths
        .Select(path => Path.GetFullPath(path))
        .ToArray();

    return WriteAsync(
        protectedPaths[0],
        outputPath,
        async (temporaryPath, token) =>
        {
            foreach (string inputPath in protectedPaths)
            {
                FileSafetyService.EnsureDifferentFile(
                    inputPath,
                    outputPath);
            }

            await using (var source = new FileStream(
                generatedFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                65536,
                useAsync: true))
            await using (var destination = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                65536,
                useAsync: true))
            {
                await source.CopyToAsync(destination, token);
                await destination.FlushAsync(token);
            }

            foreach (string inputPath in protectedPaths)
            {
                FileSafetyService.EnsureDifferentFile(
                    inputPath,
                    outputPath);
            }
        },
        cancellationToken);
}

    private static async Task WriteAsync(
        string inputPath,
        string outputPath,
        Func<string, CancellationToken, Task> write,
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

        try
        {
            await write(temporaryPath, cancellationToken);

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
}