using ImageMagick;
using ImageMagick.Configuration;
using System.IO;

namespace AetherForge.App.Services;

internal static class ImageSecurityService
{
    
    private static string? _configurationDirectory;

    public static void Initialize()
    {
        var configuration = ConfigurationFiles.Default;

        // SVG usa il motore interno, senza programmi esterni.
        configuration.Delegates.Data = "<delegatemap/>";

        configuration.Policy.Data = """
            <policymap>
            <policy domain="delegate" rights="none" pattern="*" />
            <policy domain="filter" rights="none" pattern="*" />
            <policy domain="path" rights="none" pattern="@*" />
            <policy domain="coder" rights="none" pattern="*" />
            <policy domain="coder" rights="read|write" pattern="{GIF,JPEG,JPG,JPE,PNG*,WEBP,BMP*,TIFF,TIF,AVIF,HEIC,HEIF,ICO,ICON,DIB,SVG,MSVG,RSVG,SVGZ,XC}" />
            </policymap>
            """;

        _configurationDirectory = MagickNET.Initialize(configuration);
        
        const ulong mebibyte = 1024UL * 1024UL;

        // Cache dei pixel in memoria.
        ResourceLimits.Memory = 512UL * mebibyte;

        // Cache dei pixel su disco.
        ResourceLimits.Disk = 2048UL * mebibyte;

        // Oltre questa area, la cache viene spostata su disco.
        ResourceLimits.Area = 64UL * 1024UL * 1024UL;

        // Dimensioni massime consentite.
        ResourceLimits.Width = 20000;
        ResourceLimits.Height = 20000;

        // Numero massimo di immagini in una sequenza.
        ResourceLimits.ListLength = 1024;

        // Dimensione massima dei profili, ad esempio ICC.
        ResourceLimits.MaxProfileSize = 32UL * mebibyte;

        ResourceLimits.Thread =
            (ulong)Math.Min(Environment.ProcessorCount, 4);
    }

    public static void Cleanup()
{
    string? directory = _configurationDirectory;
    _configurationDirectory = null;

    if (directory is null)
    {
        return;
    }

    try
    {
        Directory.Delete(directory, recursive: true);
    }
    catch (IOException)
    {
        // La pulizia non deve impedire la chiusura dell'app.
    }
    catch (UnauthorizedAccessException)
    {
        // La pulizia non deve impedire la chiusura dell'app.
    }
}

}