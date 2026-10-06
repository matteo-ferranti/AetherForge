using System.Globalization;
using System.IO;

using Microsoft.VisualBasic.FileIO;

namespace AetherForge.App.Services;

public static class CsvImportHelper
{
    public static string BuildImportFilter(
        string text,
        string invalidCsvMessage,
        CancellationToken cancellationToken = default)
    {
        char[] separators = [',', ';', '\t'];
        int firstRow = 1;

        using (StringReader reader = new(text))
        {
            string? firstLine = reader.ReadLine();

            string declaration =
                firstLine?.Trim().Trim('"') ?? string.Empty;

            if (declaration.StartsWith(
                    "sep=",
                    StringComparison.OrdinalIgnoreCase) &&
                declaration.Length == 5)
            {
                separators = [declaration[4]];
                firstRow = 2;

                text = reader.ReadToEnd();
            }
        }

        List<(char Separator, int Columns)> candidates = [];

        bool hasSingleColumn = false;

        foreach (char separator in separators)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int columns = GetColumnCount(
                text,
                separator,
                cancellationToken);

            if (columns > 1)
            {
                candidates.Add((separator, columns));
            }
            else if (columns == 1)
            {
                hasSingleColumn = true;
            }
        }

        char selectedSeparator;
        int selectedColumns;

        if (candidates.Count == 1)
        {
            selectedSeparator = candidates[0].Separator;
            selectedColumns = candidates[0].Columns;
        }
        else if (candidates.Count == 0 && hasSingleColumn)
        {
            selectedSeparator = separators[0];
            selectedColumns = 1;
        }
        else
        {
            throw new InvalidDataException(invalidCsvMessage);
        }

        string columnFormats = string.Join(
            "/",
            Enumerable.Range(1, selectedColumns)
                .Select(
                    column =>
                        column.ToString(
                            CultureInfo.InvariantCulture) + "/2"));

        string[] options =
        [
            ((int)selectedSeparator).ToString(
                CultureInfo.InvariantCulture),
            "34",
            "76",
            firstRow.ToString(CultureInfo.InvariantCulture),
            columnFormats,
            "0",
            "true",
            "false",
            "true",
            "false",
            "false",
            "0",
            "false",
            "false",
            "false"
        ];

        return "--infilter=Text - txt - csv (StarCalc):" +
            string.Join(",", options);
    }


    public static List<string[]> ReadRows(
    string text,
    string invalidCsvMessage,
    CancellationToken cancellationToken = default)
{
    string filter = BuildImportFilter(
        text,
        invalidCsvMessage,
        cancellationToken);

    string[] options =
        filter[(filter.IndexOf(':') + 1)..].Split(',');

    char separator = (char)int.Parse(
        options[0],
        CultureInfo.InvariantCulture);

    int firstRow = int.Parse(
        options[3],
        CultureInfo.InvariantCulture);

    using StringReader reader = new(text);

    if (firstRow == 2)
    {
        reader.ReadLine();
    }

    using TextFieldParser parser = new(reader)
    {
        TextFieldType = FieldType.Delimited,
        HasFieldsEnclosedInQuotes = true,
        TrimWhiteSpace = false
    };

    parser.SetDelimiters(separator.ToString());

    List<string[]> rows = [];

    while (!parser.EndOfData)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string[]? fields = parser.ReadFields();

        if (fields is not null)
        {
            rows.Add(fields);
        }
    }

    return rows;
}


    private static int GetColumnCount(
        string text,
        char separator,
        CancellationToken cancellationToken)
    {
        using StringReader reader = new(text);

        using TextFieldParser parser = new(reader)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = false
        };

        parser.SetDelimiters(separator.ToString());

        int expectedColumns = 0;

        try
        {
            while (!parser.EndOfData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string[]? fields = parser.ReadFields();

                if (fields is null)
                    break;

                if (expectedColumns == 0)
                {
                    expectedColumns = fields.Length;
                }
                else if (fields.Length != expectedColumns)
                {
                    return 0;
                }
            }
        }
        catch (MalformedLineException)
        {
            return 0;
        }

        return expectedColumns;
    }
}