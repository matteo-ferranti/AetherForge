using System.IO;
using System.IO.Compression;
using System.Text;

namespace AetherForge.App.Services;

internal static class ArchiveSecurityService
{
    private const long Mebibyte = 1024L * 1024L;

    private const long MaximumArchiveBytes = 256L * Mebibyte;
    private const long MaximumEntryBytes = 128L * Mebibyte;
    private const long MaximumExpandedBytes = 512L * Mebibyte;
    private const int MaximumEntries = 10000;

    public static void ValidateArchive(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (new FileInfo(inputPath).Length > MaximumArchiveBytes)
        {
            throw ArchiveLimitError();
        }

        using ZipArchive archive = ZipFile.OpenRead(inputPath);

        if (archive.Entries.Count > MaximumEntries)
        {
            throw ArchiveLimitError();
        }

        long expandedBytes = 0;

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entry.Length > MaximumEntryBytes)
            {
                throw ArchiveLimitError();
            }

            expandedBytes += entry.Length;

            if (expandedBytes > MaximumExpandedBytes)
            {
                throw ArchiveLimitError();
            }
        }
    }

    public static string ReadXmlText(
        ZipArchiveEntry entry,
        CancellationToken cancellationToken = default)
    {
        const long maximumXmlBytes = 16L * Mebibyte;

        if (entry.Length > maximumXmlBytes)
        {
            throw XmlLimitError();
        }

        using Stream source = entry.Open();
        using MemoryStream content = new();

        byte[] buffer = new byte[8192];
        long totalBytes = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int bytesRead = source.Read(
                buffer,
                0,
                buffer.Length);

            if (bytesRead == 0)
            {
                break;
            }

            totalBytes += bytesRead;

            if (totalBytes > maximumXmlBytes)
            {
                throw XmlLimitError();
            }

            content.Write(buffer, 0, bytesRead);
        }

        content.Position = 0;

        using StreamReader reader = new(
            content,
            new UTF8Encoding(false, true),
            detectEncodingFromByteOrderMarks: true);

        return reader.ReadToEnd();
    }

    private static InvalidDataException ArchiveLimitError()
    {
        return new InvalidDataException(
            LanguageService.Get("ArchiveSafetyLimitExceeded"));
    }

    private static InvalidDataException XmlLimitError()
    {
        return new InvalidDataException(
            LanguageService.Get("DocumentXmlLimitExceeded"));
    }
}