using AetherForge.App.Models;
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
        ".jpg", ".jpeg", ".png", ".webp", ".gif",
        ".bmp", ".tif", ".tiff", ".avif", ".heic",
        ".heif", ".ico", ".svg"
    ];

    private static readonly HashSet<string> VideoExtensions =
    [
        ".mp4", ".mkv", ".avi", ".mov", ".webm",
        ".wmv", ".flv", ".mpeg", ".mpg", ".m4v",
        ".3gp", ".ts", ".mts", ".m2ts"
    ];

    private static readonly HashSet<string> AudioExtensions =
    [
        ".mp3", ".wav", ".flac", ".aac", ".m4a",
        ".ogg", ".opus", ".wma", ".aiff", ".aif",
        ".alac", ".ac3"
    ];

    private static readonly HashSet<string> DocumentExtensions =
    [
        ".pdf", ".doc", ".docx", ".odt", ".rtf",
        ".txt", ".html", ".htm", ".epub",
        ".xls", ".xlsx", ".ods", ".csv",
        ".ppt", ".pptx", ".odp"
    ];

    public MainWindow()
    {
        InitializeComponent();

        FilesDataGrid.ItemsSource = _loadedFiles;
    }

    private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Multiselect = true,
            Title = "Seleziona file da convertire"
        };

        if (dialog.ShowDialog() == true)
        {
            AddFiles(dialog.FileNames);
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            AddFiles(files);
        }
    }

    private void AddFiles(IEnumerable<string> files)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
                continue;

            if (_loadedFiles.Any(x =>
                    string.Equals(
                        x.FullPath,
                        file,
                        StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            FileInfo fileInfo = new(file);

            string extension = fileInfo.Extension.ToLowerInvariant();

            _loadedFiles.Add(new LoadedFile
            {
                FullPath = fileInfo.FullName,
                Name = fileInfo.Name,
                Category = GetCategory(extension),
                Format = GetFormat(extension),
                Size = FormatFileSize(fileInfo.Length)
            });
        }

        UpdateOutputFormats();
    }

    private void RemoveSelectedButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        List<LoadedFile> selectedItems = FilesDataGrid.SelectedItems
            .Cast<LoadedFile>()
            .ToList();

        foreach (LoadedFile item in selectedItems)
        {
            _loadedFiles.Remove(item);
        }

        UpdateOutputFormats();
    }

    private void ClearListButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        _loadedFiles.Clear();

        UpdateOutputFormats();
    }

    private void OutputFormatComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_loadedFiles.Count == 0)
        {
            ConvertButton.IsEnabled = false;
            return;
        }

        if (OutputFormatComboBox.SelectedItem is not string selectedFormat)
        {
            ConvertButton.IsEnabled = false;
            return;
        }

        ConvertButton.IsEnabled =
            selectedFormat != "Selezione mista";
    }

    private void UpdateOutputFormats()
    {
        OutputFormatComboBox.Items.Clear();
        ConvertButton.IsEnabled = false;

        if (_loadedFiles.Count == 0)
            return;

        List<string> categories = _loadedFiles
            .Select(x => x.Category)
            .Distinct()
            .ToList();

        if (categories.Count != 1)
        {
            OutputFormatComboBox.Items.Add("Selezione mista");
            OutputFormatComboBox.SelectedIndex = 0;

            return;
        }

        string category = categories[0];

        string[] formats = category switch
        {
            "Immagine" =>
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
                "OGG",
                "OPUS"
            ],

            "Documento" =>
            [
                "PDF",
                "DOCX",
                "ODT",
                "RTF",
                "TXT"
            ],

            _ => []
        };

        foreach (string format in formats)
        {
            OutputFormatComboBox.Items.Add(format);
        }

        if (OutputFormatComboBox.Items.Count > 0)
        {
            OutputFormatComboBox.SelectedIndex = 0;
        }
    }

    private static string GetCategory(string extension)
    {
        if (ImageExtensions.Contains(extension))
            return "Immagine";

        if (VideoExtensions.Contains(extension))
            return "Video";

        if (AudioExtensions.Contains(extension))
            return "Audio";

        if (DocumentExtensions.Contains(extension))
            return "Documento";

        return "Sconosciuto";
    }

    private static string GetFormat(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return "—";

        return extension
            .TrimStart('.')
            .ToUpperInvariant();
    }

    private static string FormatFileSize(long bytes)
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

        while (size >= 1024 &&
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