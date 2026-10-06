using System.IO;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AetherForge.App.Services;

public sealed record SpreadsheetSheet(
    int Index,
    string Name);

public static class SpreadsheetConversionService
{
    public static async Task<List<SpreadsheetSheet>> GetSheetsAsync(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);

        string extension =
            Path.GetExtension(inputPath).ToLowerInvariant();

        if (extension is not (".xls" or ".xlsx" or ".ods"))
        {
            throw new NotSupportedException(
                LanguageService.Get("UnsupportedOfficeConversion"));
        }

        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException(
                LanguageService.Get("InvalidOfficeOutput"),
                inputPath);
        }

        string invalidDocumentMessage =
            LanguageService.Get("InvalidOfficeOutput");

        string? temporaryDirectory = null;

        try
        {
            string readablePath = inputPath;

            if (extension != ".xlsx")
            {
                temporaryDirectory = Path.Combine(
                    Path.GetTempPath(),
                    "AetherForge",
                    Guid.NewGuid().ToString("N"));

                Directory.CreateDirectory(temporaryDirectory);

                readablePath = Path.Combine(
                    temporaryDirectory,
                    "workbook.xlsx");

                await LibreOfficeService.ConvertDocumentAsync(
                    inputPath,
                    readablePath,
                    cancellationToken);
            }

            return await Task.Run(
                () =>
                {
                    using SpreadsheetDocument document =
                        SpreadsheetDocument.Open(
                            readablePath,
                            false);

                    WorkbookPart? workbookPart =
                        document.WorkbookPart;

                    Sheets? sheets =
                        workbookPart?.Workbook?.Sheets;

                    if (workbookPart is null || sheets is null)
                    {
                        throw new InvalidDataException(
                            invalidDocumentMessage);
                    }

                    List<SpreadsheetSheet> result = [];

                    int index = 0;

                    foreach (Sheet sheet in sheets.Elements<Sheet>())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Position in the workbook, starting from 1.
                        index++;

                        string? relationshipId = sheet.Id?.Value;
                        string? name = sheet.Name?.Value;

                        if (string.IsNullOrWhiteSpace(relationshipId) ||
                            string.IsNullOrWhiteSpace(name))
                        {
                            throw new InvalidDataException(
                                invalidDocumentMessage);
                        }

                        // Chart sheets do not contain a table to export.
                        if (workbookPart.GetPartById(relationshipId)
                            is not WorksheetPart)
                        {
                            continue;
                        }

                        result.Add(
                            new SpreadsheetSheet(
                                index,
                                name));
                    }

                    if (result.Count == 0)
                    {
                        throw new InvalidDataException(
                            invalidDocumentMessage);
                    }

                    return result;
                },
                cancellationToken);
        }
        finally
        {
            if (temporaryDirectory is not null)
            {
                try
                {
                    Directory.Delete(
                        temporaryDirectory,
                        recursive: true);
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
}