using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace AetherForge.App.Services;

public static class PdfPageService
{
    public static async Task<int> GetPageCountAsync(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        inputPath = Path.GetFullPath(inputPath);

        ValidateInput(inputPath);

        string result = await RunAsync(
            ["--show-npages", inputPath],
            cancellationToken);

        if (!int.TryParse(
                result.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out int pageCount) ||
            pageCount < 1)
        {
            throw ProcessingError();
        }

        return pageCount;
    }

    public static async Task ExtractPagesAsync(
        string inputPath,
        string outputPath,
        int firstPage,
        int lastPage,
        CancellationToken cancellationToken = default)
    {
        FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
        
        inputPath = Path.GetFullPath(inputPath);
        outputPath = Path.GetFullPath(outputPath);

        ValidateInput(inputPath);

        if (!Path.GetExtension(outputPath).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw ProcessingError();
        }

        if (string.Equals(
                inputPath,
                outputPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("DocumentMetadataSamePath"));
        }

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(temporaryDirectory);

            string temporaryInput = Path.Combine(
                temporaryDirectory,
                "input.pdf");

            string temporaryOutput = Path.Combine(
                temporaryDirectory,
                "output.pdf");

            cancellationToken.ThrowIfCancellationRequested();

            File.Copy(inputPath, temporaryInput);

            int pageCount = await GetPageCountAsync(
                temporaryInput,
                cancellationToken);

            if (firstPage < 1 ||
                lastPage < firstPage ||
                lastPage > pageCount)
            {
                throw new ArgumentException(
                    LanguageService.Get("PdfPageRangeInvalid"));
            }

            string range =
                firstPage.ToString(CultureInfo.InvariantCulture) +
                "-" +
                lastPage.ToString(CultureInfo.InvariantCulture);

            await RunAsync(
                [
                    "--empty",
                    "--pages",
                    temporaryInput,
                    range,
                    "--",
                    temporaryOutput
                ],
                cancellationToken);

            await RunAsync(
                ["--check", temporaryOutput],
                cancellationToken);

            int extractedPageCount = await GetPageCountAsync(
                temporaryOutput,
                cancellationToken);

            if (extractedPageCount != lastPage - firstPage + 1)
            {
                throw ProcessingError();
            }

            cancellationToken.ThrowIfCancellationRequested();

            File.Move(
                temporaryOutput,
                outputPath,
                overwrite: true);
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

    public static async Task MergePdfsAsync(
    IReadOnlyList<string> inputPaths,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    if (inputPaths.Count < 2)
    {
        throw new ArgumentException(
            LanguageService.Get("PdfMergeMinimumFiles"));
    }

    string[] fullInputPaths = inputPaths
        .Select(path => Path.GetFullPath(path))
        .ToArray();

    outputPath = Path.GetFullPath(outputPath);

    if (!Path.GetExtension(outputPath).Equals(
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw ProcessingError();
    }

        foreach (string inputPath in fullInputPaths)
        {
            ValidateInput(inputPath);

            FileSafetyService.EnsureDifferentFile(
                inputPath,
                outputPath);

        if (string.Equals(
                inputPath,
                outputPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("PdfMergeSamePath"));
        }
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
            "merged.pdf");

        List<string> arguments =
        [
            "--empty",
            "--pages"
        ];

        long expectedPageCount = 0;

        for (int index = 0; index < fullInputPaths.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string temporaryInput = Path.Combine(
                temporaryDirectory,
                "input_" +
                index.ToString(CultureInfo.InvariantCulture) +
                ".pdf");

            File.Copy(
                fullInputPaths[index],
                temporaryInput);

            int pageCount = await GetPageCountAsync(
                temporaryInput,
                cancellationToken);

            expectedPageCount += pageCount;

            arguments.Add(temporaryInput);
            arguments.Add("1-z");
        }

        arguments.Add("--");
        arguments.Add(temporaryOutput);

        await RunAsync(
            arguments,
            cancellationToken);

        await RunAsync(
            ["--check", temporaryOutput],
            cancellationToken);

        int mergedPageCount = await GetPageCountAsync(
            temporaryOutput,
            cancellationToken);

        if (mergedPageCount != expectedPageCount)
        {
            throw ProcessingError();
        }

        cancellationToken.ThrowIfCancellationRequested();

await SafeOutputService.CopyFileAsync(
    fullInputPaths,
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

    private static void ValidateInput(string inputPath)
    {
        if (!File.Exists(inputPath) ||
            !Path.GetExtension(inputPath).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw ProcessingError();
        }
    }

    private static InvalidOperationException ProcessingError()
    {
        return new InvalidOperationException(
            LanguageService.Get("PdfProcessingError"));
    }

    private static async Task<string> RunAsync(
        IEnumerable<string> arguments,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string executablePath = Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "QPDF",
            "bin",
            "qpdf.exe");

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(
                LanguageService.Get("QpdfNotFound"));
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = Path.GetDirectoryName(executablePath)!,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        if (!process.Start())
        {
            throw ProcessingError();
        }

        Task<string> standardOutput =
            process.StandardOutput.ReadToEndAsync();

        Task<string> standardError =
            process.StandardError.ReadToEndAsync();

        using var timeout =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeout.CancelAfter(TimeSpan.FromMinutes(2));

        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
            }

            await process.WaitForExitAsync();
            await Task.WhenAll(standardOutput, standardError);

            cancellationToken.ThrowIfCancellationRequested();

            throw ProcessingError();
        }

        await Task.WhenAll(standardOutput, standardError);

        if (process.ExitCode is not 0 and not 3)
        {
            throw ProcessingError();
        }

        return await standardOutput;
    }
}