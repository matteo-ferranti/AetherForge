using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;

namespace AetherForge.App.Services;

public static class PdfMetadataService
{
    public static async Task RemoveMetadataAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        FileSafetyService.EnsureDifferentFile(
        inputPath,
        outputPath);
        
        inputPath = Path.GetFullPath(inputPath);
        outputPath = Path.GetFullPath(outputPath);

        if (string.Equals(
                inputPath,
                outputPath,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                LanguageService.Get("DocumentMetadataSamePath"));
        }

        if (!File.Exists(inputPath) ||
            !Path.GetExtension(inputPath).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase) ||
            !Path.GetExtension(outputPath).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw ProcessingError();
        }

        string executablePath = Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "QPDF",
            "bin",
            "qpdf.exe");

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(
                LanguageService.Get("QpdfNotFound"));
        }

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(temporaryDirectory);

            string temporaryInput = Path.Combine(
                temporaryDirectory,
                "input.pdf");

            string temporaryOutput = Path.Combine(
                temporaryDirectory,
                "output.pdf");

            string jsonPath = Path.Combine(
                temporaryDirectory,
                "metadata.json");

            cancellationToken.ThrowIfCancellationRequested();

            File.Copy(inputPath, temporaryInput);

            await RunAsync(
                executablePath,
                [
                    "--json=2",
                    "--json-key=qpdf",
                    "--json-stream-data=none",
                    temporaryInput,
                    jsonPath
                ],
                cancellationToken);

            string json = await File.ReadAllTextAsync(
                jsonPath,
                cancellationToken);

            JsonObject document =
                JsonNode.Parse(json)?.AsObject()
                ?? throw ProcessingError();

            JsonArray qpdf =
                document["qpdf"]?.AsArray()
                ?? throw ProcessingError();

            if (qpdf.Count != 2)
            {
                throw ProcessingError();
            }

            JsonObject objects =
                qpdf[1]?.AsObject()
                ?? throw ProcessingError();

            JsonObject trailer =
                objects["trailer"]?["value"]?.AsObject()
                ?? throw ProcessingError();

            if (trailer["/Encrypt"] is not null)
            {
                throw ProcessingError();
            }

            RemoveXmpAndCheckSignatures(
                objects,
                cancellationToken);

            // Elimina l'intero dizionario delle proprietà,
            // comprese le date e le proprietà personalizzate.
            string? informationReference =
                trailer["/Info"] is JsonValue informationValue &&
                informationValue.TryGetValue<string>(out var reference)
                    ? reference
                    : null;

            trailer.Remove("/Info");

            if (informationReference is not null &&
                objects.ContainsKey("obj:" + informationReference))
            {
                objects["obj:" + informationReference] =
                    new JsonObject
                    {
                        ["value"] = null
                    };
            }

            // qpdf genererà un nuovo identificatore del file.
            trailer.Remove("/ID");

            await File.WriteAllTextAsync(
                jsonPath,
                document.ToJsonString(),
                new UTF8Encoding(false),
                cancellationToken);

            await RunAsync(
                executablePath,
                [
                    temporaryInput,
                    "--update-from-json=" + jsonPath,
                    temporaryOutput
                ],
                cancellationToken);

            if (!File.Exists(temporaryOutput) ||
                new FileInfo(temporaryOutput).Length < 5)
            {
                throw ProcessingError();
            }

            await RunAsync(
                executablePath,
                ["--check", temporaryOutput],
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

await SafeOutputService.CopyFileAsync(
    inputPath,
    temporaryOutput,
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

    private static void RemoveXmpAndCheckSignatures(
        JsonNode? node,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (node is JsonObject dictionary)
        {
            bool signatureDictionary =
                IsName(dictionary["/Type"], "/Sig") ||
                dictionary.ContainsKey("/ByteRange");

            bool signedField =
                IsName(dictionary["/FT"], "/Sig") &&
                dictionary["/V"] is not null;

            if (signatureDictionary || signedField)
            {
                throw new NotSupportedException(
                    LanguageService.Get("SignedDocumentMetadata"));
            }

            dictionary.Remove("/Metadata");

            foreach (var property in dictionary.ToArray())
            {
                RemoveXmpAndCheckSignatures(
                    property.Value,
                    cancellationToken);
            }
        }
        else if (node is JsonArray array)
        {
            foreach (JsonNode? item in array)
            {
                RemoveXmpAndCheckSignatures(
                    item,
                    cancellationToken);
            }
        }
    }

    private static bool IsName(
        JsonNode? node,
        string expected)
    {
        return node is JsonValue value &&
            value.TryGetValue<string>(out var text) &&
            text == expected;
    }

    private static InvalidOperationException ProcessingError()
    {
        return new InvalidOperationException(
            LanguageService.Get("PdfMetadataError"));
    }

    private static async Task RunAsync(
        string executablePath,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = Path.GetDirectoryName(executablePath)!,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        if (!process.Start())
        {
            throw ProcessingError();
        }

        Task<string> standardOutput =
            process.StandardOutput.ReadToEndAsync();

        Task<string> standardError =
            process.StandardError.ReadToEndAsync();

        using var timeout =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeout.CancelAfter(TimeSpan.FromMinutes(2));

        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
            }

            await process.WaitForExitAsync();
            await Task.WhenAll(standardOutput, standardError);

            cancellationToken.ThrowIfCancellationRequested();

            throw ProcessingError();
        }

        await Task.WhenAll(standardOutput, standardError);

        // qpdf usa 3 per un'operazione riuscita con avvisi.
        if (process.ExitCode is not 0 and not 3)
        {
            throw ProcessingError();
        }
    }
}