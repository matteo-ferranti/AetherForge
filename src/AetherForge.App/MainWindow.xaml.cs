using AetherForge.App.Models;
using AetherForge.App.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AetherForge.App;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<LoadedFile> _loadedFiles = [];

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
        bool hasQualityOperation =
            _loadedFiles.Any(
                file =>
                    file.Category == "Image" &&
                    file.SelectedOperation?.Operation ==
                    ConversionOperation.ChangeQuality);

        QualityColumn.Visibility =
            hasQualityOperation
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void UpdateConvertButtonState()
    {
        ConvertButton.IsEnabled =
            _loadedFiles.Any(
                file =>
                    file.Category == "Image" &&
                    file.Status == ConversionStatus.Pending &&
                    file.SelectedOperation is not null &&
                    (
                        file.SelectedOperation.Operation ==
                            ConversionOperation.Convert
                        ||
                        file.SelectedOperation.Operation ==
                            ConversionOperation.Resize
                        ||
                        file.SelectedOperation.Operation ==
                            ConversionOperation.RemoveMetadata
                        ||
                        file.SelectedOperation.Operation ==
                            ConversionOperation.ChangeQuality
                    ));
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

    private void ConvertButton_Click(
    object sender,
    RoutedEventArgs e)
{
    List<LoadedFile> imageFiles =
        _loadedFiles
            .Where(file =>
                file.Category == "Image" &&
                file.Status == ConversionStatus.Pending &&
                file.SelectedOperation is not null)
            .ToList();

    if (imageFiles.Count == 0)
    {
        MessageBox.Show(
            LanguageService.Get("NoConvertibleImagesMessage"),
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        return;
    }

    bool isBatch =
        imageFiles.Count > 1;

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

    foreach (LoadedFile file in imageFiles)
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

                    bool convertsToJpeg =
                        file.SelectedOutputFormat is
                            "JPG" or
                            "JPEG";

                    bool requiresWhiteBackground =
                        convertsToJpeg &&
                        ImageConversionService.HasTransparency(
                            file.FullPath);

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

                    MarkFileCompleted(file);

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

                    MarkFileCompleted(file);

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

                    ImageConversionService.RemoveMetadata(
                        file.FullPath,
                        outputPath);

                    MarkFileCompleted(file);

                    processedFiles++;
                    break;
                }

                case ConversionOperation.ChangeQuality:
                {
                    if (!file.QualityPercent.HasValue ||
                        file.QualityPercent.Value < 1 ||
                        file.QualityPercent.Value > 100)
                    {
                        continue;
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

                    ImageConversionService.ChangeQuality(
                        file.FullPath,
                        outputPath,
                        (uint)file.QualityPercent.Value);

                    MarkFileCompleted(file);

                    processedFiles++;
                    break;
                }
            }
        }
        catch
        {
            file.Status =
                ConversionStatus.Error;

            file.StatusText =
                GetStatusText(
                    ConversionStatus.Error);

            failedFiles++;
        }
    }

    FilesDataGrid.Items.Refresh();
    UpdateConvertButtonState();

    if (processedFiles == 0 &&
        failedFiles == 0)
    {
        MessageBox.Show(
            LanguageService.Get("NoConvertibleImagesMessage"),
            LanguageService.Get("ConversionErrorTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        return;
    }

    if (failedFiles > 0)
    {
        string message =
            string.Format(
                LanguageService.Get(
                    "BatchCompletedWithErrorsMessage"),
                processedFiles,
                failedFiles);

        MessageBox.Show(
            message,
            LanguageService.Get("ConversionCompletedTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        return;
    }

    MessageBox.Show(
        LanguageService.Get("ConversionCompletedMessage"),
        LanguageService.Get("ConversionCompletedTitle"),
        MessageBoxButton.OK,
        MessageBoxImage.Information);
}

    private static void MarkFileCompleted(
        LoadedFile file)
    {
        file.Status =
            ConversionStatus.Completed;

        file.StatusText =
            GetStatusText(
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
        string defaultFileName =
            $"{Path.GetFileNameWithoutExtension(file.FullPath)}_{defaultSuffix}.{outputFormat.ToLowerInvariant()}";

        SaveFileDialog dialog = new()
        {
            FileName = defaultFileName,
            DefaultExt = "." + outputFormat.ToLowerInvariant(),
            AddExtension = true,
            OverwritePrompt = true,

            Title = LanguageService.CurrentLanguage == "it"
                ? "Salva file convertito"
                : "Save converted file"
        };

        bool? result =
            dialog.ShowDialog();

        return result == true
            ? dialog.FileName
            : null;
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

                "Document" =>
                [
                    "PDF",
                    "DOCX",
                    "ODT",
                    "RTF",
                    "TXT"
                ],

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
                    DisplayName =
                        LanguageService.Get("OperationConvert")
                },

                new OperationOption
                {
                    Operation = ConversionOperation.ExtractAudio,
                    DisplayName =
                        LanguageService.Get("OperationExtractAudio")
                },

                new OperationOption
                {
                    Operation = ConversionOperation.CreateGif,
                    DisplayName =
                        LanguageService.Get("OperationCreateGif")
                },

                new OperationOption
                {
                    Operation = ConversionOperation.RemoveAudio,
                    DisplayName =
                        LanguageService.Get("OperationRemoveAudio")
                },

                new OperationOption
                {
                    Operation = ConversionOperation.RemoveMetadata,
                    DisplayName =
                        LanguageService.Get("OperationRemoveMetadata")
                }
            ],

            "Audio" =>
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
                    Operation = ConversionOperation.ChangeQuality,
                    DisplayName =
                        LanguageService.Get("OperationChangeQuality")
                }
            ],

            "Document" =>
            [
                new OperationOption
                {
                    Operation = ConversionOperation.Convert,
                    DisplayName =
                        LanguageService.Get("OperationConvert")
                }
            ],

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
