using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace AetherForge.App.Services;

public static class EpubConversionService
{
    public static async Task ConvertEpubToTextAsync(
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
                ".epub",
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(
                Path.GetExtension(outputPath),
                ".txt",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                LanguageService.Get("UnsupportedEpubConversion"));
        }

        string invalidMessage =
            LanguageService.Get("InvalidEpubFile");

        string noTextMessage =
            LanguageService.Get("EpubNoText");

        string extractedText = await Task.Run(
            () =>
            {
                ArchiveSecurityService.ValidateArchive(
                    inputPath,
                    cancellationToken);
                
                using ZipArchive archive =
                    ZipFile.OpenRead(inputPath);

                XNamespace containerNamespace =
                    "urn:oasis:names:tc:opendocument:xmlns:container";

                XNamespace packageNamespace =
                    "http://www.idpf.org/2007/opf";

                XDocument container = ReadXml(
                    archive,
                    "META-INF/container.xml",
                    invalidMessage);

                string? packagePath = container
                    .Descendants(containerNamespace + "rootfile")
                    .Where(element =>
                        (string?)element.Attribute("media-type") ==
                        "application/oebps-package+xml")
                    .Select(element =>
                        (string?)element.Attribute("full-path"))
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(packagePath))
                {
                    throw new InvalidDataException(invalidMessage);
                }

                XDocument package = ReadXml(
                    archive,
                    packagePath,
                    invalidMessage);

                XElement? manifest =
                    package.Root?.Element(packageNamespace + "manifest");

                XElement? spine =
                    package.Root?.Element(packageNamespace + "spine");

                if (manifest is null || spine is null)
                {
                    throw new InvalidDataException(invalidMessage);
                }

                Dictionary<string, XElement> items = [];

                foreach (XElement item in
                    manifest.Elements(packageNamespace + "item"))
                {
                    string? id = (string?)item.Attribute("id");

                    if (string.IsNullOrWhiteSpace(id) ||
                        !items.TryAdd(id, item))
                    {
                        throw new InvalidDataException(invalidMessage);
                    }
                }

                Uri packageUri = new(
                    new Uri("https://epub.invalid/"),
                    packagePath);

                StringBuilder result = new();

                foreach (XElement reference in
                    spine.Elements(packageNamespace + "itemref"))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string? id =
                        (string?)reference.Attribute("idref");

                    if (id is null ||
                        !items.TryGetValue(id, out XElement? item))
                    {
                        throw new InvalidDataException(invalidMessage);
                    }

                    string? href =
                        (string?)item.Attribute("href");

                    string? mediaType =
                        (string?)item.Attribute("media-type");

                    if (string.IsNullOrWhiteSpace(href) ||
                        mediaType != "application/xhtml+xml")
                    {
                        throw new InvalidDataException(invalidMessage);
                    }

                    Uri chapterUri = new(packageUri, href);

                    if (chapterUri.Scheme != "https" ||
                        chapterUri.Host != "epub.invalid")
                    {
                        throw new InvalidDataException(invalidMessage);
                    }

                    string chapterPath = Uri.UnescapeDataString(
                        chapterUri.AbsolutePath.TrimStart('/'));

                    XDocument chapter = ReadXml(
                        archive,
                        chapterPath,
                        invalidMessage);

                    XElement? body = chapter
                        .Descendants()
                        .FirstOrDefault(element =>
                            element.Name.LocalName == "body");

                    if (body is null)
                    {
                        throw new InvalidDataException(invalidMessage);
                    }

                    StringBuilder chapterText = new();

                    AppendText(
                        body,
                        chapterText,
                        cancellationToken);

                    string cleanedText = Regex.Replace(
                        chapterText.ToString(),
                        @"[^\S\r\n]+",
                        " ");

                    cleanedText = Regex.Replace(
                        cleanedText,
                        @" *\r?\n *",
                        "\n");

                    cleanedText = Regex.Replace(
                        cleanedText,
                        @"\n{3,}",
                        "\n\n").Trim();

                    if (cleanedText.Length == 0)
                    {
                        continue;
                    }

                    if (result.Length > 0)
                    {
                        result.AppendLine();
                        result.AppendLine();
                    }

                    result.Append(
                        cleanedText.Replace(
                            "\n",
                            Environment.NewLine));
                }

                if (result.Length == 0)
                {
                    throw new InvalidDataException(noTextMessage);
                }

                return result.ToString();
            },
            cancellationToken);

await SafeOutputService.WriteTextAsync(
    inputPath,
    outputPath,
    extractedText,
    new UTF8Encoding(false),
    cancellationToken);
    }

    public static async Task ConvertEpubToPdfAsync(
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
            ".epub",
            StringComparison.OrdinalIgnoreCase) ||
        !string.Equals(
            Path.GetExtension(outputPath),
            ".pdf",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new NotSupportedException(
            LanguageService.Get("UnsupportedEpubPdfConversion"));
    }

    string temporaryDirectory = Path.Combine(
        Path.GetTempPath(),
        "AetherForge",
        Guid.NewGuid().ToString("N"));

    try
    {
        Directory.CreateDirectory(temporaryDirectory);

        string temporaryTextPath = Path.Combine(
            temporaryDirectory,
            "book.txt");

        await ConvertEpubToTextAsync(
            inputPath,
            temporaryTextPath,
            cancellationToken);

        await DocumentConversionService.ConvertTextToPdfAsync(
            temporaryTextPath,
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

    private static XDocument ReadXml(
        ZipArchive archive,
        string entryPath,
        string invalidMessage)
    {
        ZipArchiveEntry? entry = archive.GetEntry(entryPath);

        if (entry is null)
        {
            throw new InvalidDataException(invalidMessage);
        }

        string xml = ArchiveSecurityService.ReadXmlText(entry);

        // Convert common HTML entities into numeric XML entities.
        xml = Regex.Replace(
            xml,
            @"&[A-Za-z][A-Za-z0-9]+;",
            match =>
            {
                if (match.Value is
                    "&amp;" or "&lt;" or "&gt;" or
                    "&quot;" or "&apos;")
                {
                    return match.Value;
                }

                string decoded = WebUtility.HtmlDecode(match.Value);

                if (decoded == match.Value)
                {
                    return match.Value;
                }

                StringBuilder replacement = new();

                foreach (var rune in decoded.EnumerateRunes())
                {
                    replacement.Append("&#");
                    replacement.Append(
                        rune.Value.ToString(CultureInfo.InvariantCulture));
                    replacement.Append(';');
                }

                return replacement.ToString();
            });

        using StringReader xmlText = new(xml);

        using XmlReader xmlReader = XmlReader.Create(
            xmlText,
            new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            });

        try
        {
            return XDocument.Load(
                xmlReader,
                LoadOptions.PreserveWhitespace);
        }
        catch (XmlException exception)
        {
            throw new InvalidDataException(
                invalidMessage,
                exception);
        }
    }

    private static void AppendText(
        XNode node,
        StringBuilder result,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (node is XText text)
        {
            result.Append(
                Regex.Replace(text.Value, @"\s+", " "));

            return;
        }

        if (node is not XElement element)
        {
            return;
        }

        string name = element.Name.LocalName.ToLowerInvariant();

        if (name is "script" or "style" or "head")
        {
            return;
        }

        if (name == "br")
        {
            result.AppendLine();
            return;
        }

        bool isBlock = name is
            "p" or "div" or "section" or "article" or
            "h1" or "h2" or "h3" or "h4" or "h5" or "h6" or
            "li" or "blockquote" or "pre" or "tr";

        if (isBlock)
        {
            result.AppendLine();
        }

        foreach (XNode child in element.Nodes())
        {
            AppendText(child, result, cancellationToken);
        }

        if (name is "td" or "th")
        {
            result.Append(' ');
        }

        if (isBlock)
        {
            result.AppendLine();
            result.AppendLine();
        }
    }
}