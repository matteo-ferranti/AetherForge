using System.IO;

using PDFtoImage;
using PDFtoImage.Exceptions;

using SkiaSharp;

using ImageMagick;

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace AetherForge.App.Services;

public static class PdfImageService
{
    public static Task<int> GetPageCountAsync(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(inputPath);

        return Task.Run(
            () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var stream = File.OpenRead(inputPath);

                    return Conversion.GetPageCount(stream);
                }
                catch (PdfException exception)
                {
                    throw ProcessingError(exception);
                }
            },
            cancellationToken);
    }

    public static async Task ConvertPdfPageToImageAsync(
        string inputPath,
        string outputPath,
        int pageNumber,
        int dpi = 300,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);

        string extension =
            Path.GetExtension(outputPath).ToLowerInvariant();

        SKEncodedImageFormat format = extension switch
        {
            ".png" => SKEncodedImageFormat.Png,
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,

            _ => throw new NotSupportedException(
                string.Format(
                    LanguageService.Get(
                        "UnsupportedDocumentConversion"),
                    "PDF",
                    extension.TrimStart('.').ToUpperInvariant()))
        };

        if (dpi is < 72 or > 300)
        {
            throw new ArgumentOutOfRangeException(nameof(dpi));
        }

        byte[] imageBytes = await Task.Run(
            () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var stream = File.OpenRead(inputPath);

                    int pageCount = Conversion.GetPageCount(
                        stream,
                        leaveOpen: true);

                    if (pageNumber < 1 || pageNumber > pageCount)
                    {
                        throw new ArgumentException(
                            LanguageService.Get(
                                "PdfPageRangeInvalid"));
                    }

                    int pageIndex = pageNumber - 1;

                    stream.Position = 0;

                    var pageSize = Conversion.GetPageSize(
                        stream,
                        page: pageIndex,
                        leaveOpen: true);

                    double width = Math.Ceiling(
                        pageSize.Width * dpi / 72.0);

                    double height = Math.Ceiling(
                        pageSize.Height * dpi / 72.0);

                    if (!double.IsFinite(width) ||
                        !double.IsFinite(height) ||
                        width < 1 ||
                        height < 1 ||
                        width > 10000 ||
                        height > 10000 ||
                        width * height > 40000000)
                    {
                        throw new InvalidOperationException(
                            LanguageService.Get(
                                "ImageSafetyLimitExceeded"));
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    stream.Position = 0;

                    using SKBitmap bitmap = Conversion.ToImage(
                        stream,
                        page: pageIndex,
                        leaveOpen: true,
                        options: new RenderOptions(
                            Dpi: dpi,
                            Width: (int)width,
                            Height: (int)height,
                            BackgroundColor: SKColors.White));

                    cancellationToken.ThrowIfCancellationRequested();

                    using SKData? encoded = bitmap.Encode(
                        format,
                        quality: 95);

                    if (encoded is null || encoded.Size == 0)
                    {
                        throw new InvalidOperationException(
                            LanguageService.Get(
                                "PdfProcessingError"));
                    }

                    return encoded.ToArray();
                }
                catch (PdfException exception)
                {
                    throw ProcessingError(exception);
                }
            },
            cancellationToken);

        await SafeOutputService.WriteBytesAsync(
            inputPath,
            outputPath,
            imageBytes,
            cancellationToken);
    }

    public static async Task<string> ConvertPdfPagesToImagesAsync(
    string inputPath,
    string parentDirectory,
    string outputFormat,
    int firstPage,
    int lastPage,
    CancellationToken cancellationToken = default)
{
    ValidateInput(inputPath);

    ArgumentException.ThrowIfNullOrWhiteSpace(parentDirectory);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputFormat);

    parentDirectory = Path.GetFullPath(parentDirectory);

    if (!Directory.Exists(parentDirectory))
    {
        throw new DirectoryNotFoundException(parentDirectory);
    }

    string extension = outputFormat.ToLowerInvariant();

    if (extension is not ("png" or "jpg" or "jpeg"))
    {
        throw new NotSupportedException(
            string.Format(
                LanguageService.Get("UnsupportedDocumentConversion"),
                "PDF",
                outputFormat));
    }

    // Mantiene l'originale aperto in sola lettura durante l'esportazione.
    using var inputLock = File.Open(
        inputPath,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read);

    int pageCount = await GetPageCountAsync(
        inputPath,
        cancellationToken);

    if (firstPage < 1 ||
        lastPage < firstPage ||
        lastPage > pageCount)
    {
        throw new ArgumentException(
            LanguageService.Get("PdfPageRangeInvalid"));
    }

    string temporaryDirectory = Path.Combine(
        parentDirectory,
        ".aetherforge-" + Guid.NewGuid().ToString("N"));

    string folderName =
        Path.GetFileNameWithoutExtension(inputPath) + "_images";

    try
    {
        Directory.CreateDirectory(temporaryDirectory);

        for (int page = firstPage; page <= lastPage; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string imagePath = Path.Combine(
                temporaryDirectory,
                $"page_{page:D4}.{extension}");

            await ConvertPdfPageToImageAsync(
                inputPath,
                imagePath,
                page,
                cancellationToken: cancellationToken);
        }

        int counter = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string destinationDirectory = Path.Combine(
                parentDirectory,
                counter == 0
                    ? folderName
                    : $"{folderName}_{counter}");

            if (Directory.Exists(destinationDirectory) ||
                File.Exists(destinationDirectory))
            {
                counter++;
                continue;
            }

            try
            {
                Directory.Move(
                    temporaryDirectory,
                    destinationDirectory);

                return destinationDirectory;
            }
            catch (IOException) when (
                Directory.Exists(destinationDirectory) ||
                File.Exists(destinationDirectory))
            {
                counter++;
            }
        }
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
    
    public static async Task ConvertImageToPdfAsync(
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

    if (!File.Exists(inputPath) ||
        !Path.GetExtension(outputPath).Equals(
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            LanguageService.Get("ImageToPdfFailed"));
    }

    byte[] pdfBytes = await Task.Run(
        () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using MagickImage image = new(inputPath);

            image.AutoOrient();

            if (image.Width == 0 ||
                image.Height == 0 ||
                (ulong)image.Width * image.Height > 40000000UL)
            {
                throw new InvalidOperationException(
                    LanguageService.Get("ImageSafetyLimitExceeded"));
            }

            // La trasparenza viene applicata su uno sfondo bianco.
            image.BackgroundColor = MagickColors.White;
            image.Alpha(AlphaOption.Remove);

            // Non trasferisce i profili e i metadati dell'immagine.
            image.Strip();

            byte[] pngBytes = image.ToByteArray(
                MagickFormat.Png24);

            cancellationToken.ThrowIfCancellationRequested();

            PdfDocumentBuilder builder = new();

            PdfPageBuilder page = builder.AddPage(
                PageSize.A4,
                isPortrait: image.Width <= image.Height);

            const double margin = 20;

            double availableWidth =
                page.PageSize.Width - margin * 2;

            double availableHeight =
                page.PageSize.Height - margin * 2;

            double scale = Math.Min(
                availableWidth / image.Width,
                availableHeight / image.Height);

            double imageWidth = image.Width * scale;
            double imageHeight = image.Height * scale;

            double left =
                (page.PageSize.Width - imageWidth) / 2;

            double bottom =
                (page.PageSize.Height - imageHeight) / 2;

            page.AddPng(
                pngBytes,
                new PdfRectangle(
                    left,
                    bottom,
                    left + imageWidth,
                    bottom + imageHeight));

            cancellationToken.ThrowIfCancellationRequested();

            return builder.Build();
        },
        cancellationToken);

    await SafeOutputService.WriteBytesAsync(
        inputPath,
        outputPath,
        pdfBytes,
        cancellationToken);
}
    
    public static async Task ConvertImagesToPdfAsync(
    IReadOnlyList<string> inputPaths,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(inputPaths);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

    if (inputPaths.Count < 2 ||
        !Path.GetExtension(outputPath).Equals(
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new ArgumentException(
            LanguageService.Get("ImageToPdfFailed"));
    }

    string[] originalPaths = inputPaths
        .Select(path => Path.GetFullPath(path))
        .ToArray();

    foreach (string inputPath in originalPaths)
    {
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
    }

    string temporaryDirectory = Path.Combine(
        Path.GetTempPath(),
        "AetherForge",
        Guid.NewGuid().ToString("N"));

    try
    {
        Directory.CreateDirectory(temporaryDirectory);

        List<string> generatedPdfs = [];

        for (int index = 0; index < originalPaths.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string temporaryPdf = Path.Combine(
                temporaryDirectory,
                $"image_{index:D4}.pdf");

            await ConvertImageToPdfAsync(
                originalPaths[index],
                temporaryPdf,
                cancellationToken);

            generatedPdfs.Add(temporaryPdf);
        }

        string mergedPdf = Path.Combine(
            temporaryDirectory,
            "merged.pdf");

        await PdfPageService.MergePdfsAsync(
            generatedPdfs,
            mergedPdf,
            cancellationToken);

        await SafeOutputService.CopyFileAsync(
            originalPaths,
            mergedPdf,
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
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);

        if (!File.Exists(inputPath) ||
            !Path.GetExtension(inputPath).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("PdfProcessingError"));
        }
    }

    private static InvalidOperationException ProcessingError(
        Exception exception)
    {
        return new InvalidOperationException(
            LanguageService.Get("PdfProcessingError"),
            exception);
    }
}