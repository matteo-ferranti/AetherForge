using System.IO;

using DocumentFormat.OpenXml.Packaging;

namespace AetherForge.App.Services;

public static class DocumentMetadataService
{
    public static bool SupportsFormat(string format)
    {
        return format.ToUpperInvariant() is
            "PDF" or
            "DOCX" or "XLSX" or "PPTX" or
            "ODT" or "ODS" or "ODP";
    }

    public static async Task RemoveMetadataAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
        
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        inputPath = Path.GetFullPath(inputPath);
        outputPath = Path.GetFullPath(outputPath);

        string inputExtension =
            Path.GetExtension(inputPath).ToLowerInvariant();

        string outputExtension =
            Path.GetExtension(outputPath).ToLowerInvariant();

        if (!SupportsFormat(inputExtension.TrimStart('.')) ||
            inputExtension != outputExtension)
        {
            throw new NotSupportedException(
                LanguageService.Get("UnsupportedDocumentMetadata"));
        }

        if (string.Equals(
                inputPath,
                outputPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("DocumentMetadataSamePath"));
        }

        if (inputExtension == ".pdf")
            {
                await PdfMetadataService.RemoveMetadataAsync(
                    inputPath,
                    outputPath,
                    cancellationToken);

                return;
            }

        string signedDocumentMessage =
            LanguageService.Get("SignedDocumentMetadata");

        string invalidOpenDocumentMessage =
            LanguageService.Get("InvalidOpenDocumentMetadata");    

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(temporaryDirectory);

            string temporaryPath = Path.Combine(
                temporaryDirectory,
                "document" + inputExtension);

            await Task.Run(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    File.Copy(
                        inputPath,
                        temporaryPath,
                        overwrite: false);

                    ArchiveSecurityService.ValidateArchive(
                        temporaryPath,
                        cancellationToken);

                        if (inputExtension is ".odt" or ".ods" or ".odp")
                            {
                                OpenDocumentMetadataService.RemoveMetadata(
                                    temporaryPath,
                                    inputExtension,
                                    signedDocumentMessage,
                                    invalidOpenDocumentMessage,
                                    cancellationToken);

                                return;
                            }

                    using OpenXmlPackage package = inputExtension switch
                    {
                        ".docx" => WordprocessingDocument.Open(
                            temporaryPath,
                            true),

                        ".xlsx" => SpreadsheetDocument.Open(
                            temporaryPath,
                            true),

                        ".pptx" => PresentationDocument.Open(
                            temporaryPath,
                            true),

                        _ => throw new NotSupportedException()
                    };

                    const string signatureRelationship =
                        "http://schemas.openxmlformats.org/" +
                        "package/2006/relationships/digital-signature/origin";

                    if (package.Parts.Any(part =>
                        part.OpenXmlPart.RelationshipType ==
                        signatureRelationship))
                    {
                        throw new NotSupportedException(
                            signedDocumentMessage);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    var properties = package.PackageProperties;

                    properties.Creator = null;
                    properties.LastModifiedBy = null;
                    properties.Title = null;
                    properties.Subject = null;
                    properties.Description = null;
                    properties.Keywords = null;
                    properties.Category = null;
                    properties.ContentStatus = null;
                    properties.ContentType = null;
                    properties.Identifier = null;
                    properties.Language = null;
                    properties.Version = null;
                    properties.Revision = null;
                    properties.Created = null;
                    properties.Modified = null;
                    properties.LastPrinted = null;

                    OpenXmlPart[] metadataParts = package.Parts
                        .Select(part => part.OpenXmlPart)
                        .Where(part =>
                            part is ExtendedFilePropertiesPart or
                                CustomFilePropertiesPart or
                                ThumbnailPart)
                        .ToArray();

                    foreach (OpenXmlPart part in metadataParts)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        package.DeletePart(part);
                    }

                    package.Save();
                },
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await SafeOutputService.CopyFileAsync(
                inputPath,
                temporaryPath,
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
}