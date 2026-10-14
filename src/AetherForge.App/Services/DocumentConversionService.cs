using System.IO;
using System.Text;

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

using DocumentFormat.OpenXml.Packaging;
using Word = DocumentFormat.OpenXml.Wordprocessing;

namespace AetherForge.App.Services;

public static class DocumentConversionService
{
public static async Task ConvertPdfToTextAsync(
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);

    if (!File.Exists(inputPath))
    {
        throw new FileNotFoundException(
            "Input PDF was not found.",
            inputPath);
    }

    if (!string.Equals(
            Path.GetExtension(inputPath),
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new NotSupportedException(
            "Only PDF input is supported.");
    }

    if (!string.Equals(
            Path.GetExtension(outputPath),
            ".txt",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new NotSupportedException(
            "Only TXT output is supported.");
    }

    string extractedText = await Task.Run(
        async () =>
        {
            using PdfDocument document =
                PdfDocument.Open(inputPath);

            StringBuilder result = new();

            foreach (var page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();

                string pageText =
                    ContentOrderTextExtractor.GetText(
                        page,
                        true);

                if (string.IsNullOrWhiteSpace(pageText))
                {
                    pageText = await OcrService.RecognizePdfPageAsync(
                        inputPath,
                        page.Number,
                        cancellationToken);
                }

                pageText = pageText
                    .Replace("\uFB00", "ff")
                    .Replace("\uFB01", "fi")
                    .Replace("\uFB02", "fl")
                    .Replace("\uFB03", "ffi")
                    .Replace("\uFB04", "ffl");

                if (result.Length > 0)
                {
                    result.AppendLine();
                    result.AppendLine();
                }

                result.Append(pageText.Trim());
            }

            string text = result.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    LanguageService.Get("OcrNoTextFound"));
            }

            return text;
        },
        cancellationToken);

    await SafeOutputService.WriteTextAsync(
        inputPath,
        outputPath,
        extractedText,
        new UTF8Encoding(false),
        cancellationToken);
}

    public static async Task ConvertTextToPdfAsync(
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    
    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
    
    ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

    if (!File.Exists(inputPath))
    {
        throw new FileNotFoundException(
            "Input TXT was not found.",
            inputPath);
    }

    if (!string.Equals(
            Path.GetExtension(inputPath),
            ".txt",
            StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(
            Path.GetExtension(outputPath),
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new NotSupportedException(
            "This conversion requires TXT input and PDF output.");
    }

    string fontPath = Path.Combine(
        AppContext.BaseDirectory,
        "Assets",
        "Fonts",
        "NotoSans-Regular.ttf");

    if (!File.Exists(fontPath))
    {
        throw new FileNotFoundException(
            "The PDF font was not found.",
            fontPath);
    }

    string text = await File.ReadAllTextAsync(
        inputPath,
        new UTF8Encoding(false, true),
        cancellationToken);

    byte[] fontBytes = await File.ReadAllBytesAsync(
        fontPath,
        cancellationToken);

    byte[] pdfBytes = await Task.Run(
        () =>
        {
            const double margin = 50;
            const double fontSize = 11;
            const double lineHeight = 16;

            PdfDocumentBuilder builder = new();

            var font = builder.AddTrueTypeFont(fontBytes);

            PdfPageBuilder page =
                builder.AddPage(PageSize.A4);

            double y =
                page.PageSize.Height - margin - fontSize;

            bool FitsOnLine(string value)
            {
                var letters = page.MeasureText(
                    value,
                    fontSize,
                    new PdfPoint(0, 0),
                    font);

                double width = letters.Count == 0
                    ? 0
                    : letters.Max(letter => letter.EndBaseLine.X);

                return width <= page.PageSize.Width - margin * 2;
            }

            void WriteLine(string value)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (y < margin)
                {
                    page = builder.AddPage(PageSize.A4);

                    y = page.PageSize.Height - margin - fontSize;
                }

                if (value.Length > 0)
                {
                    page.AddText(
                        value,
                        fontSize,
                        new PdfPoint(margin, y),
                        font);
                }

                y -= lineHeight;
            }

            using StringReader reader = new(text);

            string? sourceLine;

            while ((sourceLine = reader.ReadLine()) is not null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string remaining =
                    sourceLine.Replace("\t", "    ");

                if (remaining.Length == 0)
                {
                    WriteLine(string.Empty);
                    continue;
                }

                while (remaining.Length > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int low = 1;
                    int high = remaining.Length;
                    int fitLength = 0;

                    while (low <= high)
                    {
                        int middle = low + (high - low) / 2;

                        if (FitsOnLine(remaining[..middle]))
                        {
                            fitLength = middle;
                            low = middle + 1;
                        }
                        else
                        {
                            high = middle - 1;
                        }
                    }

                    if (fitLength == 0)
                    {
                        throw new InvalidOperationException(
                            "A character cannot fit inside the page margins.");
                    }

                    if (fitLength < remaining.Length)
                    {
                        int lastSpace =
                            remaining.LastIndexOf(
                                ' ',
                                fitLength - 1,
                                fitLength);

                        if (lastSpace > 0)
                        {
                            fitLength = lastSpace + 1;
                        }
                    }

                    WriteLine(
                        remaining[..fitLength].TrimEnd(' '));

                    remaining = remaining[fitLength..];
                }
            }

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

public static async Task ConvertDocxToTextAsync(
    string inputPath,
    string outputPath,
    CancellationToken cancellationToken = default)
{
    
    FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
    
    ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
    ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

    if (!File.Exists(inputPath))
    {
        throw new FileNotFoundException(
            "Input DOCX was not found.",
            inputPath);
    }

    if (!string.Equals(
            Path.GetExtension(inputPath),
            ".docx",
            StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(
            Path.GetExtension(outputPath),
            ".txt",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new NotSupportedException(
            "This conversion requires DOCX input and TXT output.");
    }

    string extractedText = await Task.Run(
        () =>
        {
            ArchiveSecurityService.ValidateArchive(
                inputPath,
                cancellationToken);

            using WordprocessingDocument document =
                WordprocessingDocument.Open(inputPath, false);

            Word.Body? body =
                document.MainDocumentPart?.Document?.Body;

            if (body is null)
            {
                throw new InvalidDataException(
                    "The DOCX document has no main body.");
            }

            StringBuilder result = new();

            foreach (Word.Paragraph paragraph in
                body.Descendants<Word.Paragraph>())
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var element in paragraph.Descendants())
                {
                    // Nested paragraphs are processed separately.
                    if (element.Ancestors<Word.Paragraph>()
                        .FirstOrDefault() != paragraph)
                    {
                        continue;
                    }

                    switch (element)
                    {
                        case Word.Text text:
                            result.Append(text.Text);
                            break;

                        case Word.TabChar:
                            result.Append('\t');
                            break;

                        case Word.Break:
                        case Word.CarriageReturn:
                            result.AppendLine();
                            break;
                    }
                }

                result.AppendLine();
            }

            string textResult = result.ToString();

            if (string.IsNullOrWhiteSpace(textResult))
            {
                throw new InvalidOperationException(
                    "No extractable text was found in the DOCX.");
            }

            return textResult;
        },
        cancellationToken);

await SafeOutputService.WriteTextAsync(
    inputPath,
    outputPath,
    extractedText,
    new UTF8Encoding(false),
    cancellationToken);
    }
}