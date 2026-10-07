using System.Globalization;
using System.IO;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

using Word = DocumentFormat.OpenXml.Wordprocessing;

namespace AetherForge.App.Services;

public static class CsvConversionService
{
    public static async Task ConvertCsvToPdfAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
        
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        if (!string.Equals(
                Path.GetExtension(inputPath),
                ".csv",
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(
                Path.GetExtension(outputPath),
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                LanguageService.Get("UnsupportedOfficeConversion"));
        }

        string invalidCsvMessage =
            LanguageService.Get("InvalidCsvImport");

        string sourceText = await File.ReadAllTextAsync(
            inputPath,
            new UTF8Encoding(false, true),
            cancellationToken);

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(temporaryDirectory);

        string temporaryDocument = Path.Combine(
            temporaryDirectory,
            "table.docx");

        try
        {
            await Task.Run(
                () =>
                {
                    List<string[]> rows = CsvImportHelper.ReadRows(
                        sourceText,
                        invalidCsvMessage,
                        cancellationToken);

                    if (rows.Count == 0)
                    {
                        throw new InvalidDataException(
                            invalidCsvMessage);
                    }

                    int columnCount = rows[0].Length;

                    // A4 landscape, with margins of approximately 1.27 cm.
                    const int pageWidth = 16838;
                    const int pageHeight = 11906;
                    const int margin = 720;
                    const int tableWidth = pageWidth - margin * 2;

                    double[] weights = new double[columnCount];

                    foreach (string[] row in rows)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        for (int column = 0; column < columnCount; column++)
                        {
                            int longestLine = row[column]
                                .Replace("\r\n", "\n")
                                .Replace('\r', '\n')
                                .Split('\n')
                                .Max(line => line.Length);

                            weights[column] = Math.Max(
                                weights[column],
                                Math.Clamp(longestLine + 2, 8, 50));
                        }
                    }

                    double totalWeight = weights.Sum();

                    int[] widths = weights
                        .Select(weight =>
                            (int)(tableWidth * weight / totalWeight))
                        .ToArray();

                    widths[^1] += tableWidth - widths.Sum();

                    using WordprocessingDocument document =
                        WordprocessingDocument.Create(
                            temporaryDocument,
                            WordprocessingDocumentType.Document);

                    MainDocumentPart mainPart =
                        document.AddMainDocumentPart();

                    Word.Body body = new();

                    mainPart.Document = new Word.Document(body);

                    Word.Table table = new();

                    table.AppendChild(
                        new Word.TableProperties(
                            new Word.TableWidth
                            {
                                Width = tableWidth.ToString(
                                    CultureInfo.InvariantCulture),
                                Type = Word.TableWidthUnitValues.Dxa
                            },
                            new Word.TableLayout
                            {
                                Type = Word.TableLayoutValues.Fixed
                            },
                            new Word.TableBorders(
                                new Word.TopBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                },
                                new Word.BottomBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                },
                                new Word.LeftBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                },
                                new Word.RightBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                },
                                new Word.InsideHorizontalBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                },
                                new Word.InsideVerticalBorder
                                {
                                    Val = Word.BorderValues.Single,
                                    Size = 4U
                                }),
                            new Word.TableCellMarginDefault(
                                new Word.TopMargin
                                {
                                    Width = "80",
                                    Type = Word.TableWidthUnitValues.Dxa
                                },
                                new Word.TableCellLeftMargin
                                {
                                    Width = 100,
                                    Type = Word.TableWidthValues.Dxa
                                },
                                new Word.BottomMargin
                                {
                                    Width = "80",
                                    Type = Word.TableWidthUnitValues.Dxa
                                },
                                new Word.TableCellRightMargin
                                {
                                    Width = 100,
                                    Type = Word.TableWidthValues.Dxa
                                })));

                    Word.TableGrid grid = new();

                    foreach (int width in widths)
                    {
                        grid.AppendChild(
                            new Word.GridColumn
                            {
                                Width = width.ToString(
                                    CultureInfo.InvariantCulture)
                            });
                    }

                    table.AppendChild(grid);

                    foreach (string[] fields in rows)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Word.TableRow row = new();

                        for (int column = 0; column < columnCount; column++)
                        {
                            Word.TableCell cell = new();

                            cell.AppendChild(
                                new Word.TableCellProperties(
                                    new Word.TableCellWidth
                                    {
                                        Width = widths[column].ToString(
                                            CultureInfo.InvariantCulture),
                                        Type = Word.TableWidthUnitValues.Dxa
                                    }));

                            string[] lines = fields[column]
                                .Replace("\r\n", "\n")
                                .Replace('\r', '\n')
                                .Split('\n');

                            foreach (string line in lines)
                            {
                                cell.AppendChild(
                                    new Word.Paragraph(
                                        new Word.ParagraphProperties(
                                            new Word.SpacingBetweenLines
                                            {
                                                Before = "0",
                                                After = "0"
                                            }),
                                        new Word.Run(
                                            new Word.RunProperties(
                                                new Word.FontSize
                                                {
                                                    Val = "20"
                                                }),
                                            new Word.Text(line)
                                            {
                                                Space =
                                                    SpaceProcessingModeValues.Preserve
                                            })));
                            }

                            row.AppendChild(cell);
                        }

                        table.AppendChild(row);
                    }

                    body.AppendChild(table);

                    // Word requires a paragraph after the table.
                    body.AppendChild(new Word.Paragraph());

                    body.AppendChild(
                        new Word.SectionProperties(
                            new Word.PageSize
                            {
                                Width = (uint)pageWidth,
                                Height = (uint)pageHeight,
                                Orient = Word.PageOrientationValues.Landscape
                            },
                            new Word.PageMargin
                            {
                                Top = margin,
                                Bottom = margin,
                                Left = (uint)margin,
                                Right = (uint)margin,
                                Header = 0U,
                                Footer = 0U,
                                Gutter = 0U
                            }));

                    mainPart.Document.Save();
                },
                cancellationToken);

            await LibreOfficeService.ConvertDocumentAsync(
                temporaryDocument,
                outputPath,
                cancellationToken);
        }
        finally
        {
            try
            {
                Directory.Delete(temporaryDirectory, true);
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