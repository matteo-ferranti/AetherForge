using AetherForge.App.Models;

namespace AetherForge.App.Services;

public static class FormatCatalog
{
    public static readonly IReadOnlyList<FormatDefinition> Formats =
    [
        new()
        {
            Name = "JPEG",
            Extension = "JPG",
            Category = "Immagine",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.Resize,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "PNG",
            Extension = "PNG",
            Category = "Immagine",
            SupportsTransparency = true,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.Resize,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "WebP",
            Extension = "WEBP",
            Category = "Immagine",
            SupportsTransparency = true,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.Resize,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "MP4",
            Extension = "MP4",
            Category = "Video",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.ExtractAudio,
                ConversionOperation.CreateGif,
                ConversionOperation.RemoveAudio,
                ConversionOperation.RemoveMetadata
            ]
        },

        new()
        {
            Name = "MKV",
            Extension = "MKV",
            Category = "Video",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.ExtractAudio,
                ConversionOperation.CreateGif,
                ConversionOperation.RemoveAudio,
                ConversionOperation.RemoveMetadata
            ]
        },

        new()
        {
            Name = "MP3",
            Extension = "MP3",
            Category = "Audio",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "FLAC",
            Extension = "FLAC",
            Category = "Audio",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "OGG",
            Extension = "OGG",
            Category = "Audio",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata,
                ConversionOperation.ChangeQuality
            ]
        },

        new()
        {
            Name = "PDF",
            Extension = "PDF",
            Category = "Documento",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata
            ]
        },

        new()
        {
            Name = "DOCX",
            Extension = "DOCX",
            Category = "Documento",
            SupportsTransparency = false,
            SupportsMetadata = true,
            SupportedOperations =
            [
                ConversionOperation.Convert,
                ConversionOperation.RemoveMetadata
            ]
        }
    ];
}