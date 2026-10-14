using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Exceptions;

namespace AetherForge.App.Services;

public static class OcrService
{
    public static async Task<string> RecognizePdfPageAsync(
        string inputPath,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string dataPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "OCR",
            "tessdata");

        if (!File.Exists(Path.Combine(dataPath, "eng.traineddata")) ||
            !File.Exists(Path.Combine(dataPath, "ita.traineddata")))
        {
            throw new InvalidOperationException(
                LanguageService.Get("OcrModelsMissing"));
        }

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            string imagePath = Path.Combine(
                temporaryDirectory,
                "page.png");

            await PdfImageService.ConvertPdfPageToImageAsync(
                inputPath,
                imagePath,
                pageNumber,
                dpi: 300,
                cancellationToken: cancellationToken);

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var engine = new Engine(
                        dataPath,
                        "eng+ita",
                        EngineMode.LstmOnly);

                    using var image =
                        TesseractOCR.Pix.Image.LoadFromFile(imagePath);

                    using var page = engine.Process(
                        image,
                        PageSegMode.Auto);

                    string text = page.Text ?? string.Empty;

                    cancellationToken.ThrowIfCancellationRequested();

                    return text.Trim();
                }
                catch (Exception exception) when (
                    exception is DllNotFoundException ||
                    exception is BadImageFormatException ||
                    exception is TypeInitializationException)
                {
                    throw new InvalidOperationException(
                        LanguageService.Get("OcrEngineUnavailable"),
                        exception);
                }
                catch (TesseractException exception)
                {
                    throw new InvalidOperationException(
                        LanguageService.Get("OcrProcessingFailed"),
                        exception);
                }
            }, cancellationToken);
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