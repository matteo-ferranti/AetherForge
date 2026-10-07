using AetherForge.App.Models;

using AetherForge.App.Services;

using Microsoft.Win32;

using System.Collections.ObjectModel;

using System.IO;

using System.Linq;

using System.Windows;

using System.Windows.Controls;

using System.Diagnostics;



namespace AetherForge.App;



public partial class MainWindow : Window

{

    private readonly ObservableCollection<LoadedFile> _loadedFiles = [];

    private bool _isProcessing;


    private static readonly HashSet<string> ImageExtensions =

    [

        ".jpg",

        ".jpeg",

        ".jpe",

        ".jfif",

        ".png",

        ".webp",

        ".gif",

        ".bmp",

        ".tif",

        ".tiff",

        ".avif",

        ".heic",

        ".heif",

        ".ico",

        ".svg"

    ];



    private static readonly HashSet<string> VideoExtensions =

    [

        ".mp4",

        ".mkv",

        ".avi",

        ".mov",

        ".webm",

        ".wmv",

        ".flv",

        ".mpeg",

        ".mpg",

        ".m4v",

        ".3gp",

        ".ts",

        ".mts",

        ".m2ts"

    ];



    private static readonly HashSet<string> AudioExtensions =

    [

        ".mp3",

        ".wav",

        ".flac",

        ".aac",

        ".m4a",

        ".ogg",

        ".opus",

        ".wma",

        ".aiff",

        ".aif",

        ".alac",

        ".ac3"

    ];



    private static readonly HashSet<string> DocumentExtensions =

    [

        ".pdf",

        ".doc",

        ".docx",

        ".odt",

        ".rtf",

        ".txt",

        ".html",

        ".htm",

        ".epub",

        ".xls",

        ".xlsx",

        ".ods",

        ".csv",

        ".ppt",

        ".pptx",

        ".odp"

    ];

   private void OpenRowOutputButton_Click(
    object sender,
    RoutedEventArgs e)
{
    if (sender is Button button &&
        button.DataContext is LoadedFile file)
    {
        OpenResult(file, showInFolder: false);
    }

    e.Handled = true;
}

private void ShowRowOutputFolderButton_Click(
    object sender,
    RoutedEventArgs e)
{
    if (sender is Button button &&
        button.DataContext is LoadedFile file)
    {
        OpenResult(file, showInFolder: true);
    }

    e.Handled = true;
}

private void OpenResult(
    LoadedFile file,
    bool showInFolder)
{
    if (file.Status != ConversionStatus.Completed ||
        string.IsNullOrWhiteSpace(file.OutputPath))
    {
        return;
    }

    string outputPath = file.OutputPath;

    if (!File.Exists(outputPath))
    {
        MessageBox.Show(
            this,
            LanguageService.Get("OutputFileUnavailable"),
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        return;
    }

    try
    {
        if (showInFolder)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{outputPath}\"",
                UseShellExecute = true
            });
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = true
            });
        }
    }
    catch (Exception)
    {
        MessageBox.Show(
            this,
            LanguageService.Get(
                showInFolder
                    ? "OpenOutputFolderFailed"
                    : "OpenOutputFailed"),
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}



    public MainWindow()

    {

        InitializeComponent();



        FilesDataGrid.ItemsSource = _loadedFiles;



        UpdateLocalizedTexts();

        UpdateLanguageButtons();

        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    // ---------------------------------------------------------

    // LOCALIZATION

    // ---------------------------------------------------------



    private void UpdateLocalizedTexts()

    {

        DragFilesTextBlock.Text =

            LanguageService.Get("DragFilesText");



        OrTextBlock.Text =

            LanguageService.Get("OrText");



        SelectFilesButton.Content =

            LanguageService.Get("SelectFilesText");



        NameColumn.Header =

            LanguageService.Get("NameText");



        CategoryColumn.Header =

            LanguageService.Get("CategoryText");



        InputColumn.Header =

            LanguageService.Get("InputText");



        OperationColumn.Header =

            LanguageService.Get("OperationText");



        ResizeWidthColumn.Header =

            LanguageService.Get("ResizeWidthText");



        ResizeHeightColumn.Header =

            LanguageService.Get("ResizeHeightText");



        QualityColumn.Header =

            LanguageService.Get("QualityText");



        AudioBitrateColumn.Header =
            LanguageService.Get("AudioBitrateText");



        OutputColumn.Header =

            LanguageService.Get("OutputText");



        SizeColumn.Header =

            LanguageService.Get("SizeText");



        StatusColumn.Header =

            LanguageService.Get("StatusText");



        RemoveSelectedButton.Content =

            LanguageService.Get("RemoveSelectedText");



        ClearListButton.Content =

            LanguageService.Get("ClearListText");



        ConvertButton.Content =

            LanguageService.Get("ConvertText");

        MergePdfsButton.Content =

            LanguageService.Get("PdfMergeTitle");

        Resources["OpenOutputTooltip"] =
            LanguageService.Get("OpenOutputText");

        Resources["ShowOutputFolderTooltip"] =
            LanguageService.Get("ShowOutputFolderText");

    }



    private static string GetStatusText(

        ConversionStatus status)

    {

        return status switch

        {

            ConversionStatus.Pending =>

                LanguageService.Get("StatusPending"),



            ConversionStatus.Completed =>

                LanguageService.Get("StatusCompleted"),



            ConversionStatus.Error =>

                LanguageService.Get("StatusError"),



            _ => string.Empty

        };

    }



    private void RefreshStatusTranslations()

    {

        foreach (LoadedFile file in _loadedFiles)

        {

            file.StatusText =

                GetStatusText(file.Status);

        }



        FilesDataGrid.Items.Refresh();

    }



    // ---------------------------------------------------------

    // COLUMN VISIBILITY

    // ---------------------------------------------------------



    private void UpdateResizeColumnsVisibility()

    {

        bool hasResizeOperation =

            _loadedFiles.Any(

                file =>

                    file.SelectedOperation?.Operation ==

                    ConversionOperation.Resize);



        Visibility visibility =

            hasResizeOperation

                ? Visibility.Visible

                : Visibility.Collapsed;



        ResizeWidthColumn.Visibility = visibility;

        ResizeHeightColumn.Visibility = visibility;

    }



 private void UpdateQualityColumnVisibility()
{
    bool hasImageQualityOperation =
        _loadedFiles.Any(
            file =>
                file.Category == "Image" &&
                file.SelectedOperation?.Operation ==
                    ConversionOperation.ChangeQuality);

    bool hasAudioQualityOperation =
        _loadedFiles.Any(
            file =>
                file.Category == "Audio" &&
                file.SelectedOperation?.Operation ==
                    ConversionOperation.ChangeQuality);

    QualityColumn.Visibility =
        hasImageQualityOperation
            ? Visibility.Visible
            : Visibility.Collapsed;

    AudioBitrateColumn.Visibility =
        hasAudioQualityOperation
            ? Visibility.Visible
            : Visibility.Collapsed;
}


        private async void MergePdfsButton_Click(
    object sender,
    RoutedEventArgs e)
{
    if (_isProcessing)
    {
        return;
    }

    var mergeDialog = new PdfMergeDialog
    {
        Owner = this
    };

    if (mergeDialog.ShowDialog() != true)
    {
        return;
    }

    IReadOnlyList<string> inputPaths =
        mergeDialog.SelectedFiles;

    var saveDialog = new SaveFileDialog
    {
        Title = LanguageService.Get("PdfMergeSaveTitle"),
        Filter = LanguageService.Get("PdfMergeFileFilter"),
        FileName = LanguageService.Get("PdfMergeDefaultFileName"),
        DefaultExt = ".pdf",
        AddExtension = true,
        OverwritePrompt = true
    };

    if (saveDialog.ShowDialog(this) != true)
    {
        return;
    }

    string outputPath = Path.GetFullPath(saveDialog.FileName);

    if (!Path.GetExtension(outputPath).Equals(
        ".pdf",
        StringComparison.OrdinalIgnoreCase))
{
    MessageBox.Show(
        this,
        string.Format(
            LanguageService.Get("OutputExtensionMismatch"),
            ".pdf"),
        LanguageService.Get("ConversionErrorTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Warning);

    return;
}

if (_loadedFiles.Any(file =>
    string.Equals(
        Path.GetFullPath(file.FullPath),
        outputPath,
        StringComparison.OrdinalIgnoreCase)))
{
    MessageBox.Show(
        this,
        LanguageService.Get("OutputMatchesOriginal"),
        LanguageService.Get("ConversionErrorTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Warning);

    return;
}

    if (!Path.GetExtension(outputPath).Equals(
        ".pdf",
        StringComparison.OrdinalIgnoreCase))
{
    MessageBox.Show(
        this,
        string.Format(
            LanguageService.Get("OutputExtensionMismatch"),
            ".pdf"),
        LanguageService.Get("ConversionErrorTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Warning);

    return;
}

if (_loadedFiles.Any(file =>
    string.Equals(
        Path.GetFullPath(file.FullPath),
        outputPath,
        StringComparison.OrdinalIgnoreCase)))
{
    MessageBox.Show(
        this,
        LanguageService.Get("OutputMatchesOriginal"),
        LanguageService.Get("ConversionErrorTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Warning);

    return;
}

    if (inputPaths.Any(path =>
        string.Equals(
            Path.GetFullPath(path),
            outputPath,
            StringComparison.OrdinalIgnoreCase)))
    {
        MessageBox.Show(
            this,
            LanguageService.Get("PdfMergeSamePath"),
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        return;
    }

    _isProcessing = true;
    UpdateConvertButtonState();

    string? errorMessage = null;

    try
    {
        await PdfPageService.MergePdfsAsync(
            inputPaths,
            outputPath);
    }


    catch (Exception exception)
    {
        string qpdfPath = Path.Combine(
            AppContext.BaseDirectory,
            "Tools",
            "QPDF",
            "bin",
            "qpdf.exe");

        if (!File.Exists(qpdfPath))
        {
            errorMessage = LanguageService.Get("QpdfNotFound");
        }
        else
        {
            errorMessage = LanguageService.GetUserFriendlyError(
                exception,
                "PdfMergeFailed");
        }
    }
    finally
    {
        _isProcessing = false;
        UpdateConvertButtonState();
    }

    if (errorMessage is not null)
    {
        MessageBox.Show(
            this,
            errorMessage,
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        return;
    }

    MessageBox.Show(
        this,
        string.Format(
            LanguageService.Get("PdfMergeCompleted"),
            outputPath),
        LanguageService.Get("ConversionCompletedTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Information);
}


        private void UpdateConvertButtonState()
        {
            ConvertButton.IsEnabled =
                !_isProcessing &&
                _loadedFiles.Any(CanProcessFile);

            MergePdfsButton.IsEnabled = !_isProcessing;
        }



    private static bool CanProcessFile(

        LoadedFile file)

    {

        if (file.Status != ConversionStatus.Pending ||

            file.SelectedOperation is null)

        {

            return false;

        }



        if (file.Category == "Image")
        {
            return file.SelectedOperation.Operation is
                ConversionOperation.Convert or
                ConversionOperation.Resize or
                ConversionOperation.RemoveMetadata or
                ConversionOperation.ChangeQuality or
                ConversionOperation.Rotate or
                ConversionOperation.Flip;
        }



        if (file.Category == "Video")
        {
            return file.SelectedOperation.Operation is
                ConversionOperation.Convert or
                ConversionOperation.ExtractAudio or
                ConversionOperation.RemoveAudio or
                ConversionOperation.RemoveMetadata or
                ConversionOperation.CreateGif or
                ConversionOperation.Trim;
        }

        if (file.Category == "Audio")
        {
            return file.SelectedOperation.Operation switch
            {
                ConversionOperation.Convert => true,
                ConversionOperation.RemoveMetadata => true,
                ConversionOperation.ChangeQuality =>
                    SupportsAudioQuality(file.Format),
                ConversionOperation.Trim => true,
                _ => false
            };
        }

        if (file.Category == "Document")
        {
            return file.SelectedOperation.Operation switch
            {
                ConversionOperation.Convert =>
                    file.Format is
                        "PDF" or "TXT" or "DOCX" or "DOC" or "ODT" or "RTF" or
                        "HTML" or "HTM" or "XLS" or "XLSX" or "ODS" or "CSV" or
                        "PPT" or "PPTX" or "ODP" or "EPUB",

                ConversionOperation.RemoveMetadata =>
                    DocumentMetadataService.SupportsFormat(file.Format),

                ConversionOperation.ExtractPages =>
                    file.Format == "PDF",

                _ => false
            };
        }

      return false;
    }

    // ---------------------------------------------------------

    // LANGUAGE

    // ---------------------------------------------------------



    private void EnglishButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        LanguageService.SetLanguage("en");



        UpdateLocalizedTexts();

        RefreshOperationTranslations();

        RefreshStatusTranslations();

        UpdateLanguageButtons();

        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    private void ItalianButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        LanguageService.SetLanguage("it");



        UpdateLocalizedTexts();

        RefreshOperationTranslations();

        RefreshStatusTranslations();

        UpdateLanguageButtons();

        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    private void UpdateLanguageButtons()

    {

        var tealBrush =

            (System.Windows.Media.Brush)

            Application.Current.Resources["TealBrush"];



        var bronzeBrush =

            (System.Windows.Media.Brush)

            Application.Current.Resources["BronzeLightBrush"];



        if (LanguageService.CurrentLanguage == "en")

        {

            EnglishButton.BorderBrush = tealBrush;

            EnglishButton.BorderThickness = new Thickness(2);



            ItalianButton.BorderBrush = bronzeBrush;

            ItalianButton.BorderThickness = new Thickness(1);

        }

        else

        {

            ItalianButton.BorderBrush = tealBrush;

            ItalianButton.BorderThickness = new Thickness(2);



            EnglishButton.BorderBrush = bronzeBrush;

            EnglishButton.BorderThickness = new Thickness(1);

        }

    }



    // ---------------------------------------------------------

    // FILE SELECTION

    // ---------------------------------------------------------



    private void SelectFilesButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        OpenFileDialog dialog = new()

        {

            Multiselect = true,



            Title = LanguageService.CurrentLanguage == "it"

                ? "Seleziona i file da convertire"

                : "Select files to convert"

        };



        if (dialog.ShowDialog() == true)

        {

            AddFiles(dialog.FileNames);

        }

    }



    // ---------------------------------------------------------

    // DRAG & DROP

    // ---------------------------------------------------------



    private void Window_DragOver(

        object sender,

        DragEventArgs e)

    {

        e.Effects =

            e.Data.GetDataPresent(DataFormats.FileDrop)

                ? DragDropEffects.Copy

                : DragDropEffects.None;



        e.Handled = true;

    }



    private void Window_Drop(

        object sender,

        DragEventArgs e)

    {

        if (!e.Data.GetDataPresent(DataFormats.FileDrop))

            return;



        if (e.Data.GetData(DataFormats.FileDrop) is string[] files)

        {

            AddFiles(files);

        }

    }



    // ---------------------------------------------------------

    // FILE MANAGEMENT

    // ---------------------------------------------------------



    private void AddFiles(IEnumerable<string> files)

    {

        foreach (string file in files)

        {

            if (!File.Exists(file))

                continue;



            bool alreadyLoaded =

                _loadedFiles.Any(

                    loadedFile =>

                        string.Equals(

                            loadedFile.FullPath,

                            file,

                            StringComparison.OrdinalIgnoreCase));



            if (alreadyLoaded)

                continue;



            FileInfo fileInfo = new(file);



            string extension =

                fileInfo.Extension.ToLowerInvariant();



            string category =

                GetCategory(extension);



            string inputFormat =

                GetFormat(extension);



            ObservableCollection<string> outputFormats =

                new(

                    GetAvailableOutputFormats(

                        category,

                        inputFormat));



            ObservableCollection<OperationOption> operations =

                new(

                    GetAvailableOperations(

                        category,

                        inputFormat));



            LoadedFile loadedFile = new()

            {

                FullPath = fileInfo.FullName,

                Name = fileInfo.Name,

                Category = category,

                Format = inputFormat,

                Size = FormatFileSize(fileInfo.Length),



                Status = ConversionStatus.Pending,

                StatusText = GetStatusText(

                    ConversionStatus.Pending),



                AvailableOperations = operations,



                SelectedOperation =

                    operations.FirstOrDefault(),



                AvailableOutputFormats = outputFormats,



                SelectedOutputFormat =

                    outputFormats.FirstOrDefault()

            };



            _loadedFiles.Add(loadedFile);

        }



        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    private void RemoveSelectedButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        List<LoadedFile> selectedItems =

            FilesDataGrid.SelectedItems

                .Cast<LoadedFile>()

                .ToList();



        foreach (LoadedFile item in selectedItems)

        {

            _loadedFiles.Remove(item);

        }



        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    private void ClearListButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        _loadedFiles.Clear();



        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    // ---------------------------------------------------------

    // CONVERSION

    // ---------------------------------------------------------



    private async void ConvertButton_Click(

        object sender,

        RoutedEventArgs e)

    {

        List<LoadedFile> filesToProcess =

            _loadedFiles

                .Where(CanProcessFile)

                .ToList();



        if (filesToProcess.Count == 0)

        {

            MessageBox.Show(

                LanguageService.Get("NoConvertibleFilesMessage"),

                LanguageService.Get("ConversionErrorTitle"),

                MessageBoxButton.OK,

                MessageBoxImage.Information);



            return;

        }



        bool isBatch =

            filesToProcess.Count > 1;



        string? batchOutputDirectory =

            null;



        if (isBatch)

        {

            batchOutputDirectory =

                AskBatchOutputDirectory();



            if (string.IsNullOrWhiteSpace(

                batchOutputDirectory))

            {

                return;

            }

        }



        int processedFiles = 0;

        int failedFiles = 0;

        List<(string FileName, Exception Error)> errorDetails = [];



        _isProcessing = true;
        UpdateConvertButtonState();



        try

        {

            foreach (LoadedFile file in filesToProcess)

            {

                if (file.SelectedOperation is null)

                    continue;



                try

                {

                    switch (file.SelectedOperation.Operation)

                    {

                        case ConversionOperation.Convert:

                        {

                            if (string.IsNullOrWhiteSpace(

                                file.SelectedOutputFormat))

                            {

                                continue;

                            }


int? csvSheetIndex = null;

if (file.Category == "Document" &&
    (file.Format is "XLS" or "XLSX" or "ODS") &&
    file.SelectedOutputFormat == "CSV")
{
    List<SpreadsheetSheet> sheets =
        await SpreadsheetConversionService.GetSheetsAsync(
            file.FullPath);

    CsvSheetSelectionDialog sheetDialog = new(
        file.Name,
        sheets)
    {
        Owner = this
    };

    if (sheetDialog.ShowDialog() != true ||
        sheetDialog.SelectedSheet is null)
    {
        continue;
    }

    csvSheetIndex = sheetDialog.SelectedSheet.Index;
}


                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        file.SelectedOutputFormat,

                                        "converted")

                                    : AskSingleOutputPath(

                                        file,

                                        "converted",

                                        file.SelectedOutputFormat);



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



if (file.Category == "Document")
{
    if (file.Format == "PDF" &&
        file.SelectedOutputFormat == "TXT")
    {
        await DocumentConversionService.ConvertPdfToTextAsync(
            file.FullPath,
            outputPath);
    }
    else if (file.Format == "EPUB" &&
            (file.SelectedOutputFormat is "PDF" or "TXT"))
    {
        if (file.SelectedOutputFormat == "PDF")
        {
            await EpubConversionService.ConvertEpubToPdfAsync(
                file.FullPath,
                outputPath);
        }
        else
        {
            await EpubConversionService.ConvertEpubToTextAsync(
                file.FullPath,
                outputPath);
        }
    }
    else if (file.Format == "TXT" &&
             file.SelectedOutputFormat == "PDF")
    {
        await DocumentConversionService.ConvertTextToPdfAsync(
            file.FullPath,
            outputPath);
    }
    else if (file.Format == "DOCX" &&
             file.SelectedOutputFormat == "TXT")
    {
        await DocumentConversionService.ConvertDocxToTextAsync(
            file.FullPath,
            outputPath);
    }
    else if (file.Format == "CSV" &&
         file.SelectedOutputFormat == "PDF")
{
    await CsvConversionService.ConvertCsvToPdfAsync(
        file.FullPath,
        outputPath);
}
else if ((file.Format is "PPT" or "PPTX" or "ODP") &&
         (file.SelectedOutputFormat is
             "PDF" or "PPT" or "PPTX" or "ODP"))
{
    await LibreOfficeService.ConvertDocumentAsync(
        file.FullPath,
        outputPath);
}
else if ((file.Format is "XLS" or "XLSX" or "ODS") &&
         (file.SelectedOutputFormat is
             "PDF" or "XLS" or "XLSX" or "ODS" or "CSV"))
{
    await LibreOfficeService.ConvertDocumentAsync(
        file.FullPath,
        outputPath,
        csvSheetIndex: csvSheetIndex);
}

    else if ((file.Format is "DOCX" or "DOC" or "ODT" or "RTF" or "TXT" or "HTML" or "HTM") &&
             (file.SelectedOutputFormat is
                 "PDF" or "DOCX" or "ODT" or "RTF" or "TXT"))
    {
        await LibreOfficeService.ConvertDocumentAsync(
            file.FullPath,
            outputPath);
    }
    else
    {
        throw new NotSupportedException(
            string.Format(
                LanguageService.Get("UnsupportedDocumentConversion"),
                file.Format,
                file.SelectedOutputFormat));
    }
}

else if (file.Category == "Audio")
                            {
                                await MediaConversionService.ConvertAudioAsync(
                                    file.FullPath,
                                    outputPath);
                            }
                            else if (file.Category == "Video")
                            {
                                await MediaConversionService.ConvertVideoAsync(
                                    file.FullPath,
                                    outputPath);
                            }

                            else

                            {

                                bool convertsToJpeg =

                                    file.SelectedOutputFormat is

                                        "JPG" or

                                        "JPEG";



                                bool requiresWhiteBackground =

                                    convertsToJpeg &&

                                    ImageConversionService.HasTransparency(

                                        file.FullPath);



                                if (requiresWhiteBackground)

                                {

                                    ImageConversionService

                                        .ConvertImageWithWhiteBackground(

                                            file.FullPath,

                                            outputPath,

                                            file.SelectedOutputFormat);

                                }

                                else

                                {

                                    ImageConversionService.ConvertImage(

                                        file.FullPath,

                                        outputPath,

                                        file.SelectedOutputFormat);

                                }

                            }



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }



                        case ConversionOperation.ExtractAudio:

                        {

                            if (string.IsNullOrWhiteSpace(

                                file.SelectedOutputFormat))

                            {

                                continue;

                            }



                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        file.SelectedOutputFormat,

                                        "audio")

                                    : AskSingleOutputPath(

                                        file,

                                        "audio",

                                        file.SelectedOutputFormat);



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



                            await MediaConversionService.ExtractAudioAsync(

                                file.FullPath,

                                outputPath);



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }



                        case ConversionOperation.CreateGif:

                        {

                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        "GIF",

                                        "gif")

                                    : AskSingleOutputPath(

                                        file,

                                        "gif",

                                        "GIF");



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



                            await MediaConversionService.CreateGifAsync(

                                file.FullPath,

                                outputPath);



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }



                        case ConversionOperation.RemoveAudio:

                        {

                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        file.Format,

                                        "no_audio")

                                    : AskSingleOutputPath(

                                        file,

                                        "no_audio",

                                        file.Format);



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



                            await MediaConversionService.RemoveAudioAsync(

                                file.FullPath,

                                outputPath);



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }

case ConversionOperation.Rotate:
case ConversionOperation.Flip:
{
    bool rotating =
        file.SelectedOperation.Operation ==
        ConversionOperation.Rotate;

    var dialog = new ImageTransformDialog(
        file.Name,
        rotating)
    {
        Owner = this
    };

    if (dialog.ShowDialog() != true)
    {
        continue;
    }

    int degrees = dialog.SelectedDegrees;
    bool horizontal = dialog.SelectedHorizontal;

    string suffix = rotating
        ? $"rotated_{degrees}"
        : horizontal
            ? "flipped_horizontal"
            : "flipped_vertical";

    string? outputPath = isBatch
        ? GenerateSafeBatchOutputPath(
            file,
            batchOutputDirectory!,
            file.Format,
            suffix)
        : AskSingleOutputPath(
            file,
            suffix,
            file.Format);

    if (string.IsNullOrWhiteSpace(outputPath))
    {
        continue;
    }

    await Task.Run(() =>
    {
        if (rotating)
        {
            ImageConversionService.RotateImage(
                file.FullPath,
                outputPath,
                degrees);
        }
        else
        {
            ImageConversionService.FlipImage(
                file.FullPath,
                outputPath,
                horizontal);
        }
    });

    MarkFileCompleted(file, outputPath);
    processedFiles++;

    break;
}

case ConversionOperation.ExtractPages:
{
    int pageCount = await PdfPageService.GetPageCountAsync(
        file.FullPath);

    var dialog = new PdfPageSelectionDialog(
        file.Name,
        pageCount)
    {
        Owner = this
    };

    if (dialog.ShowDialog() != true)
    {
        continue;
    }

    string suffix =
        $"pages_{dialog.FirstPage}-{dialog.LastPage}";

    string? outputPath = isBatch
        ? GenerateSafeBatchOutputPath(
            file,
            batchOutputDirectory!,
            "PDF",
            suffix)
        : AskSingleOutputPath(
            file,
            suffix,
            "PDF");

    if (string.IsNullOrWhiteSpace(outputPath))
    {
        continue;
    }

    await PdfPageService.ExtractPagesAsync(
        file.FullPath,
        outputPath,
        dialog.FirstPage,
        dialog.LastPage);

    MarkFileCompleted(file, outputPath);
    processedFiles++;

    break;
}


case ConversionOperation.Trim:
{
    string? outputFormat = file.SelectedOutputFormat;

    if (string.IsNullOrWhiteSpace(outputFormat))
    {
        continue;
    }

    bool audioOnly = file.Category == "Audio";

    TimeSpan duration = await MediaTrimService.GetDurationAsync(
        file.FullPath);

    var dialog = new MediaTrimDialog(
        file.Name,
        duration)
    {
        Owner = this
    };

    if (dialog.ShowDialog() != true)
    {
        continue;
    }

    string? outputPath = isBatch
        ? GenerateSafeBatchOutputPath(
            file,
            batchOutputDirectory!,
            outputFormat,
            "trimmed")
        : AskSingleOutputPath(
            file,
            "trimmed",
            outputFormat);

    if (string.IsNullOrWhiteSpace(outputPath))
    {
        continue;
    }

    await MediaTrimService.TrimAsync(
        file.FullPath,
        outputPath,
        dialog.StartTime,
        dialog.EndTime,
        audioOnly);

    MarkFileCompleted(file, outputPath);
    processedFiles++;

    break;
}


                        case ConversionOperation.Resize:

                        {

                            if (!file.ResizeWidth.HasValue ||

                                !file.ResizeHeight.HasValue ||

                                file.ResizeWidth.Value <= 0 ||

                                file.ResizeHeight.Value <= 0)

                            {

                                continue;

                            }



                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        file.Format,

                                        "resized")

                                    : AskSingleOutputPath(

                                        file,

                                        "resized",

                                        file.Format);



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



                            ImageConversionService.ResizeImage(

                                file.FullPath,

                                outputPath,

                                (uint)file.ResizeWidth.Value,

                                (uint)file.ResizeHeight.Value);



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }



                        case ConversionOperation.RemoveMetadata:

                        {

                            string? outputPath =

                                isBatch

                                    ? GenerateSafeBatchOutputPath(

                                        file,

                                        batchOutputDirectory!,

                                        file.Format,

                                        "no_metadata")

                                    : AskSingleOutputPath(

                                        file,

                                        "no_metadata",

                                        file.Format);



                            if (string.IsNullOrWhiteSpace(outputPath))

                                continue;



                           if (file.Category == "Document")
                                {
                                    await DocumentMetadataService.RemoveMetadataAsync(
                                        file.FullPath,
                                        outputPath);
                                }
                                else if (file.Category is "Video" or "Audio")
                                {
                                    await MediaConversionService.RemoveMetadataAsync(
                                        file.FullPath,
                                        outputPath);
                                }
                                else
                                {
                                    ImageConversionService.RemoveMetadata(
                                        file.FullPath,
                                        outputPath);
                                }



                            MarkFileCompleted(file, outputPath);



                            processedFiles++;

                            break;

                        }



case ConversionOperation.ChangeQuality:
{
    bool isAudio = file.Category == "Audio";

    if (isAudio)
    {
        if (!SupportsAudioQuality(file.Format))
            continue;

        if (!file.AudioBitrateKbps.HasValue ||
            !file.AvailableAudioBitrates.Contains(
                file.AudioBitrateKbps.Value))
        {
            continue;
        }
    }
    else
    {
        if (!file.QualityPercent.HasValue ||
            file.QualityPercent.Value < 1 ||
            file.QualityPercent.Value > 100)
        {
            continue;
        }
    }

    string? outputPath =
        isBatch
            ? GenerateSafeBatchOutputPath(
                file,
                batchOutputDirectory!,
                file.Format,
                "quality")
            : AskSingleOutputPath(
                file,
                "quality",
                file.Format);

    if (string.IsNullOrWhiteSpace(outputPath))
        continue;

    if (isAudio)
    {
        await MediaConversionService.ChangeAudioQualityAsync(
            file.FullPath,
            outputPath,
            file.AudioBitrateKbps!.Value);
    }
    else
    {
        ImageConversionService.ChangeQuality(
            file.FullPath,
            outputPath,
            (uint)file.QualityPercent!.Value);
    }

    MarkFileCompleted(file, outputPath);

    processedFiles++;

    break;
}

                    }

                }

                catch (Exception exception)
                {
                    file.Status = ConversionStatus.Error;

                    file.StatusText = GetStatusText(
                        ConversionStatus.Error);

                    failedFiles++;

                    if (errorDetails.Count < 10)
                    {
                        errorDetails.Add((
                            Path.GetFileName(file.FullPath),
                            exception));
                    }
                }
            } // Chiude foreach
        } // Chiude try
        finally

        {
            _isProcessing = false;

            FilesDataGrid.Items.Refresh();

            UpdateConvertButtonState();

        }



        if (processedFiles == 0 &&

            failedFiles == 0)

        {

            MessageBox.Show(

                LanguageService.Get("NoConvertibleFilesMessage"),

                LanguageService.Get("ConversionErrorTitle"),

                MessageBoxButton.OK,

                MessageBoxImage.Information);



            return;

        }



 if (failedFiles > 0)
{
    string message = string.Format(
        LanguageService.Get("BatchCompletedWithErrorsMessage"),
        processedFiles,
        failedFiles);

    foreach (var detail in errorDetails)
    {
        message +=
            Environment.NewLine +
            Environment.NewLine +
            detail.FileName + ": " +
            LanguageService.GetUserFriendlyError(detail.Error);
    }

    if (failedFiles > errorDetails.Count)
    {
        message +=
            Environment.NewLine +
            Environment.NewLine +
            LanguageService.Get("ErrorDetailsLimited");
    }

    MessageBox.Show(
        this,
        message,
        LanguageService.Get("ConversionErrorTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Warning);

    return;
}



        MessageBox.Show(

            LanguageService.Get("ConversionCompletedMessage"),

            LanguageService.Get("ConversionCompletedTitle"),

            MessageBoxButton.OK,

            MessageBoxImage.Information);

    }



    private void MarkFileCompleted(
        LoadedFile file,
        string outputPath)
    {
        file.OutputPath = Path.GetFullPath(outputPath);

        file.Status = ConversionStatus.Completed;

        file.StatusText = GetStatusText(
            ConversionStatus.Completed);
    }


    // ---------------------------------------------------------

    // OUTPUT PATHS

    // ---------------------------------------------------------



    private string? AskSingleOutputPath(
    LoadedFile file,
    string defaultSuffix,
    string outputFormat)
{
    string extension = outputFormat.ToLowerInvariant();

    string defaultFileName =
        $"{Path.GetFileNameWithoutExtension(file.FullPath)}_" +
        $"{defaultSuffix}.{extension}";

    var dialog = new SaveFileDialog
    {
        FileName = defaultFileName,
        DefaultExt = "." + extension,
        AddExtension = true,
        OverwritePrompt = true,
        CheckPathExists = true,

        Filter = string.Format(
            LanguageService.Get("OutputFormatFilter"),
            outputFormat.ToUpperInvariant(),
            extension),

        Title = LanguageService.Get("SaveOutputTitle")
    };

    while (dialog.ShowDialog(this) == true)
    {
        string outputPath = Path.GetFullPath(dialog.FileName);

        if (!Path.GetExtension(outputPath).Equals(
                "." + extension,
                StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                this,
                string.Format(
                    LanguageService.Get("OutputExtensionMismatch"),
                    "." + extension),
                LanguageService.Get("ConversionErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            continue;
        }

        bool matchesOriginal =
            string.Equals(
                outputPath,
                Path.GetFullPath(file.FullPath),
                StringComparison.OrdinalIgnoreCase) ||
            _loadedFiles.Any(loadedFile =>
                string.Equals(
                    outputPath,
                    Path.GetFullPath(loadedFile.FullPath),
                    StringComparison.OrdinalIgnoreCase));

        if (matchesOriginal)
        {
            MessageBox.Show(
                this,
                LanguageService.Get("OutputMatchesOriginal"),
                LanguageService.Get("ConversionErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            continue;
        }

        return outputPath;
    }

    return null;
}



    private string? AskBatchOutputDirectory()

    {

        try

        {

            OpenFolderDialog parentFolderDialog = new()

            {

                Title =

                    LanguageService.Get(

                        "SelectBatchFolderText")

            };



            if (parentFolderDialog.ShowDialog() != true)

                return null;



            string parentDirectory =

                parentFolderDialog.FolderName;



            if (string.IsNullOrWhiteSpace(

                parentDirectory))

            {

                return null;

            }



            BatchFolderNameDialog nameDialog = new()

            {

                Owner = this

            };



            if (nameDialog.ShowDialog() != true)

                return null;



            string folderName =

                SanitizeFileName(

                    nameDialog.FolderName);



            if (string.IsNullOrWhiteSpace(

                folderName))

            {

                folderName =

                    "AetherForge_Output";

            }



            return GenerateSafeDirectoryPath(

                parentDirectory,

                folderName);

        }

        catch (Exception ex)

        {

            MessageBox.Show(

                $"{LanguageService.Get("ConversionErrorMessage")}\n\n{ex.Message}",

                LanguageService.Get("ConversionErrorTitle"),

                MessageBoxButton.OK,

                MessageBoxImage.Error);



            return null;

        }

    }



    private static string GenerateSafeDirectoryPath(

        string parentDirectory,

        string folderName)

    {

        string outputDirectory =

            Path.Combine(

                parentDirectory,

                folderName);



        int counter = 1;



        while (Directory.Exists(outputDirectory))

        {

            outputDirectory =

                Path.Combine(

                    parentDirectory,

                    $"{folderName}_{counter}");



            counter++;

        }



        Directory.CreateDirectory(

            outputDirectory);



        return outputDirectory;

    }



    private static string SanitizeFileName(

        string fileName)

    {

        char[] invalidCharacters =

            Path.GetInvalidFileNameChars();



        foreach (char character in invalidCharacters)

        {

            fileName =

                fileName.Replace(

                    character,

                    '_');

        }



        return fileName.Trim();

    }



    private static string GenerateSafeBatchOutputPath(

        LoadedFile file,

        string outputDirectory,

        string outputFormat,

        string suffix)

    {

        string fileNameWithoutExtension =

            Path.GetFileNameWithoutExtension(

                file.FullPath);



        string extension =

            "." + outputFormat.ToLowerInvariant();



        string outputPath =

            Path.Combine(

                outputDirectory,

                $"{fileNameWithoutExtension}_{suffix}{extension}");



        int counter = 1;



        while (File.Exists(outputPath))

        {

            outputPath =

                Path.Combine(

                    outputDirectory,

                    $"{fileNameWithoutExtension}_{suffix}_{counter}{extension}");



            counter++;

        }



        return outputPath;

    }



    // ---------------------------------------------------------

    // RESIZE VALIDATION

    // ---------------------------------------------------------



    private void PositiveIntegerTextBox_PreviewTextInput(

        object sender,

        System.Windows.Input.TextCompositionEventArgs e)

    {

        if (sender is not TextBox textBox)

            return;



        string newText =

            GetTextAfterInsertion(

                textBox,

                e.Text);



        e.Handled =

            !int.TryParse(

                newText,

                out int value)

            || value <= 0;

    }



    private void PositiveIntegerTextBox_Pasting(

        object sender,

        DataObjectPastingEventArgs e)

    {

        if (sender is not TextBox textBox)

        {

            e.CancelCommand();

            return;

        }



        if (!e.DataObject.GetDataPresent(DataFormats.Text))

        {

            e.CancelCommand();

            return;

        }



        string pastedText =

            e.DataObject.GetData(DataFormats.Text)

                as string

            ?? string.Empty;



        string newText =

            GetTextAfterInsertion(

                textBox,

                pastedText);



        bool isValid =

            int.TryParse(

                newText,

                out int value)

            && value > 0;



        if (!isValid)

        {

            e.CancelCommand();

        }

    }



    // ---------------------------------------------------------

    // QUALITY VALIDATION

    // ---------------------------------------------------------



    private void PercentageTextBox_PreviewTextInput(

        object sender,

        System.Windows.Input.TextCompositionEventArgs e)

    {

        if (sender is not TextBox textBox)

            return;



        string newText =

            GetTextAfterInsertion(

                textBox,

                e.Text);



        e.Handled =

            !int.TryParse(

                newText,

                out int value)

            || value < 1

            || value > 100;

    }



    private void PercentageTextBox_Pasting(

        object sender,

        DataObjectPastingEventArgs e)

    {

        if (sender is not TextBox textBox)

        {

            e.CancelCommand();

            return;

        }



        if (!e.DataObject.GetDataPresent(DataFormats.Text))

        {

            e.CancelCommand();

            return;

        }



        string pastedText =

            e.DataObject.GetData(DataFormats.Text)

                as string

            ?? string.Empty;



        string newText =

            GetTextAfterInsertion(

                textBox,

                pastedText);



        bool isValid =

            int.TryParse(

                newText,

                out int value)

            && value >= 1

            && value <= 100;



        if (!isValid)

        {

            e.CancelCommand();

        }

    }



    private static string GetTextAfterInsertion(

        TextBox textBox,

        string insertedText)

    {

        string currentText =

            textBox.Text ?? string.Empty;



        int selectionStart =

            textBox.SelectionStart;



        int selectionLength =

            textBox.SelectionLength;



        return currentText

            .Remove(

                selectionStart,

                selectionLength)

            .Insert(

                selectionStart,

                insertedText);

    }



    // ---------------------------------------------------------

    // OPERATION SELECTION

    // ---------------------------------------------------------



    private void OperationComboBox_SelectionChanged(

        object sender,

        SelectionChangedEventArgs e)

    {

        if (sender is not ComboBox comboBox)

            return;



        if (comboBox.DataContext is not LoadedFile file)

            return;



        if (file.SelectedOperation is null)

            return;



        UpdateOutputFormatsForFile(file);

        UpdateResizeColumnsVisibility();

        UpdateQualityColumnVisibility();

        UpdateConvertButtonState();

    }



    private static void UpdateOutputFormatsForFile(

        LoadedFile file)

    {

        if (file.SelectedOperation is null)

            return;



        IEnumerable<string> formats =

            file.SelectedOperation.Operation switch

            {

                ConversionOperation.Convert =>

                    GetAvailableOutputFormats(

                        file.Category,

                        file.Format),



                ConversionOperation.ExtractAudio =>

                [

                    "MP3",

                    "WAV",

                    "FLAC",

                    "AAC",

                    "M4A",

                    "OGG",

                    "OPUS"

                ],



                ConversionOperation.CreateGif =>

                [

                    "GIF"

                ],



                ConversionOperation.RemoveAudio =>

                [

                    file.Format

                ],



                ConversionOperation.RemoveMetadata =>

                [

                    file.Format

                ],



                ConversionOperation.Resize =>

                [

                    file.Format

                ],



                ConversionOperation.ChangeQuality =>

                [

                    file.Format

                ],

                ConversionOperation.Rotate => [file.Format],
                ConversionOperation.Flip => [file.Format],

                ConversionOperation.ExtractPages => ["PDF"],

                ConversionOperation.Trim =>
                    MediaTrimService.GetOutputFormats(file.Category == "Audio")
                        .OrderByDescending(format => format == file.Format),

                _ => []

            };



        file.AvailableOutputFormats.Clear();



        foreach (string format in formats)

        {

            file.AvailableOutputFormats.Add(format);

        }



        file.SelectedOutputFormat =

            file.AvailableOutputFormats.FirstOrDefault();

    }



    // ---------------------------------------------------------

    // AVAILABLE OUTPUT FORMATS

    // ---------------------------------------------------------



    private static IEnumerable<string>

        GetAvailableOutputFormats(

            string category,

            string inputFormat)

    {

        string[] formats =

            category switch

            {

                "Image" =>

                [

                    "JPG",

                    "PNG",

                    "WEBP",

                    "BMP",

                    "TIFF",

                    "AVIF"

                ],



                "Video" =>

                [

                    "MP4",

                    "MKV",

                    "MOV",

                    "WEBM",

                    "AVI"

                ],



                "Audio" =>

                [

                    "MP3",

                    "WAV",

                    "FLAC",

                    "AAC",

                    "M4A",

                    "OGG",

                    "OPUS"

                ],



                "Document" => inputFormat switch
                {
                    "PDF" => new[] { "TXT" },
                    "EPUB" => new[] { "PDF", "TXT" },
                    "TXT" => new[] { "PDF", "DOCX", "ODT", "RTF" },
                    "DOCX" => new[] { "PDF", "TXT", "ODT", "RTF" },
                    "DOC" => new[] { "PDF", "DOCX", "ODT", "RTF", "TXT" },
                    "ODT" => new[] { "PDF", "DOCX", "RTF", "TXT" },
                    "RTF" => new[] { "PDF", "DOCX", "ODT", "TXT" },
                    "HTML" or "HTM" => new[] { "PDF", "DOCX", "ODT", "RTF", "TXT" },
                    "XLS" => new[] { "PDF", "XLSX", "ODS", "CSV" },
                    "XLSX" => new[] { "PDF", "XLS", "ODS", "CSV" },
                    "ODS" => new[] { "PDF", "XLSX", "XLS", "CSV" },
                    "CSV" => new[] { "PDF" },
                    "PPT" => new[] { "PDF", "PPTX", "ODP" },
                    "PPTX" => new[] { "PDF", "PPT", "ODP" },
                    "ODP" => new[] { "PDF", "PPTX", "PPT" },

                    _ => Array.Empty<string>()
                },

                _ => []
            };


        return formats.Where(

            format =>

                !string.Equals(

                    format,

                    inputFormat,

                    StringComparison.OrdinalIgnoreCase));

    }



    // ---------------------------------------------------------

    // AVAILABLE OPERATIONS

    // ---------------------------------------------------------

private static bool SupportsAudioQuality(string format)
{
    return format.ToUpperInvariant() is
        "MP3" or "AAC" or "M4A" or "OGG" or "OPUS";
}

private static IEnumerable<OperationOption>
    GetAvailableAudioOperations(string inputFormat)
{
    List<OperationOption> operations =
    [
        new OperationOption
        {
            Operation = ConversionOperation.Convert,
            DisplayName = LanguageService.Get("OperationConvert")
        },
        new OperationOption
        {
            Operation = ConversionOperation.RemoveMetadata,
            DisplayName = LanguageService.Get("OperationRemoveMetadata")
        }
    ];

    operations.Add(new OperationOption
    {
        Operation = ConversionOperation.Trim,
        DisplayName = LanguageService.Get("OperationTrim")
    });

    if (SupportsAudioQuality(inputFormat))
    {
        operations.Add(
            new OperationOption
            {
                Operation = ConversionOperation.ChangeQuality,
                DisplayName = LanguageService.Get("OperationChangeQuality")
            });
    }

    return operations;
}


private static IEnumerable<OperationOption>
    GetAvailableDocumentOperations(string inputFormat)
{
    List<OperationOption> operations =
    [
        new OperationOption
        {
            Operation = ConversionOperation.Convert,
            DisplayName = LanguageService.Get("OperationConvert")
        }
    ];

    if (inputFormat == "PDF")
    {
        operations.Add(new OperationOption
        {
            Operation = ConversionOperation.ExtractPages,
            DisplayName = LanguageService.Get("OperationExtractPages")
        });
    }

    if (DocumentMetadataService.SupportsFormat(inputFormat))
    {
        operations.Add(
            new OperationOption
            {
                Operation = ConversionOperation.RemoveMetadata,
                DisplayName =
                    LanguageService.Get("OperationRemoveMetadata")
            });
    }

    return operations;
}


    private static IEnumerable<OperationOption>

        GetAvailableOperations(

            string category,

            string inputFormat)

    {

        if (category == "Image")

        {

            List<OperationOption> operations =

            [

                new OperationOption

                {

                    Operation = ConversionOperation.Convert,

                    DisplayName =

                        LanguageService.Get("OperationConvert")

                },



                new OperationOption

                {

                    Operation = ConversionOperation.RemoveMetadata,

                    DisplayName =

                        LanguageService.Get("OperationRemoveMetadata")

                },



                new OperationOption

                {

                    Operation = ConversionOperation.Resize,

                    DisplayName =

                        LanguageService.Get("OperationResize")

                }

            ];

            operations.Add(new OperationOption
{
    Operation = ConversionOperation.Rotate,
    DisplayName = LanguageService.Get("OperationRotate")
});

operations.Add(new OperationOption
{
    Operation = ConversionOperation.Flip,
    DisplayName = LanguageService.Get("OperationFlip")
});



            bool supportsQuality =

                inputFormat is

                    "JPG" or

                    "JPEG" or

                    "JPE" or

                    "JFIF" or

                    "WEBP" or

                    "AVIF";



            if (supportsQuality)

            {

                operations.Add(

                    new OperationOption

                    {

                        Operation =

                            ConversionOperation.ChangeQuality,



                        DisplayName =

                            LanguageService.Get(

                                "OperationChangeQuality")

                    });

            }



            return operations;

        }



        return category switch

        {

           "Video" =>
[
    new OperationOption
    {
        Operation = ConversionOperation.Convert,
        DisplayName = LanguageService.Get("OperationConvert")
    },
    new OperationOption
    {
        Operation = ConversionOperation.ExtractAudio,
        DisplayName = LanguageService.Get("OperationExtractAudio")
    },
    new OperationOption
    {
        Operation = ConversionOperation.CreateGif,
        DisplayName = LanguageService.Get("OperationCreateGif")
    },
    new OperationOption
    {
        Operation = ConversionOperation.RemoveAudio,
        DisplayName = LanguageService.Get("OperationRemoveAudio")
    },
    new OperationOption
    {
        Operation = ConversionOperation.RemoveMetadata,
        DisplayName = LanguageService.Get("OperationRemoveMetadata")
    },
    new OperationOption
    {
        Operation = ConversionOperation.Trim,
        DisplayName = LanguageService.Get("OperationTrim")
    }
],



            "Audio" => GetAvailableAudioOperations(inputFormat),



            "Document" => GetAvailableDocumentOperations(inputFormat),

            _ => []

        };

    }



    // ---------------------------------------------------------

    // REFRESH OPERATION TRANSLATIONS

    // ---------------------------------------------------------



    private void RefreshOperationTranslations()

    {

        foreach (LoadedFile file in _loadedFiles)

        {

            ConversionOperation? previousOperation =

                file.SelectedOperation?.Operation;



            List<OperationOption> translatedOperations =

                GetAvailableOperations(

                    file.Category,

                    file.Format)

                .ToList();



            file.AvailableOperations.Clear();



            foreach (OperationOption operation in translatedOperations)

            {

                file.AvailableOperations.Add(operation);

            }



            file.SelectedOperation =

                previousOperation.HasValue

                    ? file.AvailableOperations.FirstOrDefault(

                        operation =>

                            operation.Operation ==

                            previousOperation.Value)

                    : file.AvailableOperations.FirstOrDefault();

        }



        FilesDataGrid.Items.Refresh();

    }



    // ---------------------------------------------------------

    // FILE TYPE DETECTION

    // ---------------------------------------------------------



    private static string GetCategory(

        string extension)

    {

        if (ImageExtensions.Contains(extension))

            return "Image";



        if (VideoExtensions.Contains(extension))

            return "Video";



        if (AudioExtensions.Contains(extension))

            return "Audio";



        if (DocumentExtensions.Contains(extension))

            return "Document";



        return "Unknown";

    }



    private static string GetFormat(

        string extension)

    {

        if (string.IsNullOrWhiteSpace(extension))

            return "—";



        return extension

            .TrimStart('.')

            .ToUpperInvariant();

    }



    // ---------------------------------------------------------

    // FILE SIZE

    // ---------------------------------------------------------



    private static string FormatFileSize(

        long bytes)

    {

        string[] units =

        [

            "B",

            "KB",

            "MB",

            "GB",

            "TB"

        ];



        double size = bytes;

        int unitIndex = 0;



        while (

            size >= 1024 &&

            unitIndex < units.Length - 1)

        {

            size /= 1024;

            unitIndex++;

        }



        return unitIndex == 0

            ? $"{size:0} {units[unitIndex]}"

            : $"{size:0.##} {units[unitIndex]}";

    }

}
