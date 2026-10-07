using System.Diagnostics;
using System.IO;

namespace AetherForge.App.Services;

public static class LibreOfficeService
{
    public static async Task ConvertDocumentAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default,
        int? csvSheetIndex = null)
    {
        
        FileSafetyService.EnsureDifferentFile(
            inputPath,
            outputPath);
        
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        inputPath = Path.GetFullPath(inputPath);
        outputPath = Path.GetFullPath(outputPath);

        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException(
                "Input document was not found.",
                inputPath);
        }

  string inputExtension =
    Path.GetExtension(inputPath).ToLowerInvariant();

string outputExtension =
    Path.GetExtension(outputPath).ToLowerInvariant();

bool isSpreadsheet =
    inputExtension is ".xls" or ".xlsx" or ".ods" or ".csv";

bool isTextDocument =
    inputExtension is
        ".docx" or ".doc" or ".odt" or ".rtf" or ".txt" or
        ".html" or ".htm";

bool isPresentation =
    inputExtension is ".ppt" or ".pptx" or ".odp";

bool supportedInput =
    isSpreadsheet || isTextDocument || isPresentation;

bool supportedOutput =
    isPresentation
        ? outputExtension is ".pdf" or ".ppt" or ".pptx" or ".odp"
        : isSpreadsheet
            ? outputExtension == ".pdf" ||
              (inputExtension != ".csv" &&
               (outputExtension is ".xls" or ".xlsx" or ".ods" or ".csv"))
            : outputExtension is
                ".pdf" or ".docx" or ".odt" or ".rtf" or ".txt";

if (!supportedInput ||
    !supportedOutput ||
    inputExtension == outputExtension)
{
    throw new NotSupportedException(
        LanguageService.Get("UnsupportedOfficeConversion"));
}

if (outputExtension == ".csv" &&
    (!csvSheetIndex.HasValue || csvSheetIndex.Value < 1))
{
    throw new InvalidOperationException(
        LanguageService.Get("InvalidCsvSheet"));
}

if (inputExtension is ".docx" or ".xlsx" or ".pptx" or
    ".odt" or ".ods" or ".odp")
{
    await Task.Run(
        () => ArchiveSecurityService.ValidateArchive(
            inputPath,
            cancellationToken),
        cancellationToken);
}

string exportFilter = outputExtension switch
{
    ".pdf" => isPresentation
        ? "pdf:impress_pdf_Export"
        : isSpreadsheet
            ? "pdf:calc_pdf_Export"
            : "pdf:writer_pdf_Export",

    ".ppt" => "ppt:MS PowerPoint 97",
    ".pptx" => "pptx:Impress MS PowerPoint 2007 XML",
    ".odp" => "odp:impress8",

    ".xls" => "xls:MS Excel 97",
    ".xlsx" => "xlsx:Calc Office Open XML",
    ".ods" => "ods:calc8",
    ".csv" =>
        "csv:Text - txt - csv (StarCalc):" +
        "44,34,76,1,,0,true,false,true,false,false," +
        csvSheetIndex.GetValueOrDefault().ToString(
            System.Globalization.CultureInfo.InvariantCulture) +
        ",false,true,false",

    ".docx" => "docx:Office Open XML Text",
    ".odt" => "odt:writer8",
    ".rtf" => "rtf:Rich Text Format",
    ".txt" => "txt:Text (encoded):UTF8",

    _ => throw new NotSupportedException(
        LanguageService.Get("UnsupportedOfficeConversion"))
};

        string executablePath = Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "LibreOffice",
            "program",
            "soffice.com");

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(
                "The bundled LibreOffice executable was not found.",
                executablePath);
        }

        cancellationToken.ThrowIfCancellationRequested();

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "AetherForge",
            Guid.NewGuid().ToString("N"));

        string profileDirectory =
            Path.Combine(temporaryDirectory, "profile");

        string exportDirectory =
            Path.Combine(temporaryDirectory, "output");

        try
        {
            Directory.CreateDirectory(profileDirectory);
            Directory.CreateDirectory(exportDirectory);

            await LibreOfficeSecurityService.PrepareProfileAsync(
                profileDirectory,
                cancellationToken);

            string conversionInputPath = inputPath;
string? csvImportFilter = null;

if (inputExtension is ".txt" or ".csv")
{
    string inputDirectory =
        Path.Combine(temporaryDirectory, "input");

    Directory.CreateDirectory(inputDirectory);

    conversionInputPath = Path.Combine(
        inputDirectory,
        Path.GetFileName(inputPath));

    string sourceText = await File.ReadAllTextAsync(
        inputPath,
        new System.Text.UTF8Encoding(false, true),
        cancellationToken);

    if (inputExtension == ".csv")
    {
        string invalidCsvMessage =
            LanguageService.Get("InvalidCsvImport");

        csvImportFilter = await Task.Run(
            () => CsvImportHelper.BuildImportFilter(
                sourceText,
                invalidCsvMessage,
                cancellationToken),
            cancellationToken);
    }

    await File.WriteAllTextAsync(
        conversionInputPath,
        sourceText,
        new System.Text.UTF8Encoding(false),
        cancellationToken);
}

            string profileUri = new Uri(
                profileDirectory + Path.DirectorySeparatorChar)
                .AbsoluteUri;

            ProcessStartInfo startInfo = new()
            {
                FileName = executablePath,
                WorkingDirectory =
                    Path.GetDirectoryName(executablePath)!,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.Environment["SAL_DISABLE_PRINTERLIST"] = "1";
            startInfo.Environment["SAL_DISABLE_DEFAULTPRINTER"] = "1";

            startInfo.ArgumentList.Add(
                "-env:UserInstallation=" + profileUri);

            startInfo.ArgumentList.Add("--headless");
            startInfo.ArgumentList.Add("--nologo");
            startInfo.ArgumentList.Add("--nodefault");
            startInfo.ArgumentList.Add("--norestore");

            startInfo.ArgumentList.Add("--convert-to");
            startInfo.ArgumentList.Add(exportFilter);

            startInfo.ArgumentList.Add("--outdir");
            startInfo.ArgumentList.Add(exportDirectory);

            if (inputExtension == ".txt")
            {
                startInfo.ArgumentList.Add(
                    "--infilter=Text (encoded):UTF8");
            }
            else if (inputExtension is ".html" or ".htm")
            {
                startInfo.ArgumentList.Add(
                    "--infilter=HTML (StarWriter)");
            }
            else if (inputExtension == ".csv")
            {
                startInfo.ArgumentList.Add(csvImportFilter!);
            }

            startInfo.ArgumentList.Add(conversionInputPath);

            using Process process = new()
            {
                StartInfo = startInfo
            };

            if (!process.Start())
            {
                throw new InvalidOperationException(
                    "LibreOffice could not be started.");
            }

            Task<string> standardOutputTask =
                process.StandardOutput.ReadToEndAsync();

            Task<string> standardErrorTask =
                process.StandardError.ReadToEndAsync();

            using CancellationTokenSource timeout =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            timeout.CancelAfter(TimeSpan.FromMinutes(2));

            try
            {
                await process.WaitForExitAsync(timeout.Token);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }

                await process.WaitForExitAsync();

                await Task.WhenAll(
                    standardOutputTask,
                    standardErrorTask);

                cancellationToken.ThrowIfCancellationRequested();

                throw new TimeoutException(
                    "LibreOffice conversion exceeded two minutes.");
            }

            string standardOutput = await standardOutputTask;
            string standardError = await standardErrorTask;

            cancellationToken.ThrowIfCancellationRequested();

string generatedFilePath;

if (outputExtension == ".csv")
{
    string[] generatedCsvFiles = Directory.GetFiles(
        exportDirectory,
        "*.csv",
        SearchOption.TopDirectoryOnly);

    if (generatedCsvFiles.Length != 1)
    {
        throw new InvalidOperationException(
            LanguageService.Get("OfficeConversionFailed"));
    }

    generatedFilePath = generatedCsvFiles[0];
}
else
{
    generatedFilePath = Path.Combine(
        exportDirectory,
        Path.GetFileNameWithoutExtension(inputPath) + outputExtension);
}

if (process.ExitCode != 0 ||
    !File.Exists(generatedFilePath))
{
    throw new InvalidOperationException(
        LanguageService.Get("OfficeConversionFailed") +
        Environment.NewLine +
        standardOutput +
        Environment.NewLine +
        standardError);
}

if (new FileInfo(generatedFilePath).Length == 0)
{
    throw new InvalidDataException(
        LanguageService.Get("InvalidOfficeOutput"));
}

if (outputExtension == ".pdf")
{
    byte[] header = new byte[5];

    await using (FileStream pdfStream =
        File.OpenRead(generatedFilePath))
    {
        if (pdfStream.Length < header.Length)
        {
            throw new InvalidDataException(
                LanguageService.Get("InvalidOfficeOutput"));
        }

        await pdfStream.ReadExactlyAsync(
            header.AsMemory(),
            cancellationToken);
    }

    if (System.Text.Encoding.ASCII.GetString(header) != "%PDF-")
    {
        throw new InvalidDataException(
            LanguageService.Get("InvalidOfficeOutput"));
    }
}

cancellationToken.ThrowIfCancellationRequested();

FileSafetyService.EnsureDifferentFile(
    inputPath,
    outputPath);

await SafeOutputService.CopyFileAsync(
    inputPath,
    generatedFilePath,
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
                // Temporary files may still be locked.
            }
            catch (UnauthorizedAccessException)
            {
                // Cleanup must not hide the conversion result.
            }
        }
    }

        public static Task ConvertDocumentToPdfAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(
                Path.GetExtension(outputPath),
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                LanguageService.Get("UnsupportedDocumentPdfConversion"));
        }

        return ConvertDocumentAsync(
            inputPath,
            outputPath,
            cancellationToken);
    }
}