using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AetherForge.App.Services;

internal static class OpenDocumentMetadataService
{
    public static void RemoveMetadata(
        string path,
        string extension,
        string signedDocumentMessage,
        string invalidDocumentMessage,
        CancellationToken cancellationToken = default)
    {
        using ZipArchive archive = ZipFile.Open(
            path,
            ZipArchiveMode.Update);

        cancellationToken.ThrowIfCancellationRequested();

        bool hasSignature = archive.Entries.Any(entry =>
            entry.FullName.StartsWith(
                "META-INF/",
                StringComparison.OrdinalIgnoreCase) &&
            entry.FullName.EndsWith(
                "signatures.xml",
                StringComparison.OrdinalIgnoreCase));

        if (hasSignature)
        {
            throw new NotSupportedException(
                signedDocumentMessage);
        }

        string expectedMimeType = extension switch
        {
            ".odt" => "application/vnd.oasis.opendocument.text",
            ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".odp" => "application/vnd.oasis.opendocument.presentation",
            _ => throw new InvalidDataException(
                invalidDocumentMessage)
        };

        ZipArchiveEntry? mimeEntry = archive.GetEntry("mimetype");

        ZipArchiveEntry? manifestEntry =
            archive.GetEntry("META-INF/manifest.xml");

        if (mimeEntry is null ||
            manifestEntry is null ||
            archive.GetEntry("content.xml") is null)
        {
            throw new InvalidDataException(
                invalidDocumentMessage);
        }

        using (Stream stream = mimeEntry.Open())
        using (StreamReader reader = new(stream, Encoding.UTF8))
        {
            if (reader.ReadToEnd().Trim() != expectedMimeType)
            {
                throw new InvalidDataException(
                    invalidDocumentMessage);
            }
        }

        XNamespace manifestNamespace =
            "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";

        XDocument manifest = ReadXml(manifestEntry);

        if (manifest.Descendants(
                manifestNamespace + "encryption-data").Any())
        {
            throw new NotSupportedException(
                invalidDocumentMessage);
        }

        ZipArchiveEntry? metadataEntry =
            archive.GetEntry("meta.xml");

        // A document without this entry has no standard properties to clear.
        if (metadataEntry is null)
        {
            return;
        }

        XDocument metadata = ReadXml(metadataEntry);

        XNamespace officeNamespace =
            "urn:oasis:names:tc:opendocument:xmlns:office:1.0";

        if (metadata.Root?.Name !=
            officeNamespace + "document-meta")
        {
            throw new InvalidDataException(
                invalidDocumentMessage);
        }

        XElement? properties =
            metadata.Root.Element(officeNamespace + "meta");

        if (properties is null)
        {
            return;
        }

        properties.RemoveNodes();
        properties.RemoveAttributes();

        cancellationToken.ThrowIfCancellationRequested();

        metadataEntry.Delete();

        ZipArchiveEntry cleanedEntry = archive.CreateEntry(
            "meta.xml",
            CompressionLevel.Optimal);

        using Stream output = cleanedEntry.Open();

        using XmlWriter writer = XmlWriter.Create(
            output,
            new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = false
            });

        metadata.Save(writer);
    }

    private static XDocument ReadXml(ZipArchiveEntry entry)
    {
        using Stream stream = entry.Open();

        using XmlReader reader = XmlReader.Create(
            stream,
            new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            });

        return XDocument.Load(
            reader,
            LoadOptions.PreserveWhitespace);
    }
}