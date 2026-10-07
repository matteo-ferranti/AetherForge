using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class PdfMergeDialog : Window
{
    private sealed record PdfFileItem(
        string FullPath,
        string Name);

    private readonly ObservableCollection<PdfFileItem> _files = [];

    public IReadOnlyList<string> SelectedFiles =>
        _files.Select(file => file.FullPath).ToArray();

    public PdfMergeDialog()
    {
        InitializeComponent();

        Title = LanguageService.Get("PdfMergeTitle");

        InfoTextBlock.Text =
            LanguageService.Get("PdfMergeInfo");

        AddButton.Content =
            LanguageService.Get("PdfMergeAdd");

        RemoveButton.Content =
            LanguageService.Get("PdfMergeRemove");

        MoveUpButton.Content =
            LanguageService.Get("PdfMergeMoveUp");

        MoveDownButton.Content =
            LanguageService.Get("PdfMergeMoveDown");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        MergeButton.Content =
            LanguageService.Get("PdfMergeTitle");

        FilesListBox.ItemsSource = _files;

        UpdateButtons();
    }

 private void AddButton_Click(
    object sender,
    RoutedEventArgs e)
{
    var dialog = new OpenFileDialog
    {
        Title = LanguageService.Get("PdfMergeAdd"),
        Filter = LanguageService.Get("PdfMergeFileFilter"),
        Multiselect = true,
        CheckFileExists = true
    };

    if (dialog.ShowDialog(this) == true)
    {
        AddFiles(dialog.FileNames);
    }
}


private void AddFiles(IEnumerable<string> paths)
{
    foreach (string path in paths)
    {
        if (!File.Exists(path) ||
            !Path.GetExtension(path).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        string fullPath = Path.GetFullPath(path);

        bool alreadyAdded = _files.Any(file =>
            string.Equals(
                file.FullPath,
                fullPath,
                StringComparison.OrdinalIgnoreCase));

        if (alreadyAdded)
        {
            continue;
        }

        _files.Add(new PdfFileItem(
            fullPath,
            Path.GetFileName(fullPath)));
    }

    if (_files.Count > 0)
    {
        FilesListBox.SelectedIndex = _files.Count - 1;
    }

    UpdateButtons();
}

private void Window_DragOver(
    object sender,
    DragEventArgs e)
{
    e.Effects = DragDropEffects.None;

    if (e.Data.GetData(DataFormats.FileDrop) is string[] paths &&
        paths.Any(path =>
            File.Exists(path) &&
            Path.GetExtension(path).Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase)))
    {
        e.Effects = DragDropEffects.Copy;
    }

    e.Handled = true;
}

private void Window_Drop(
    object sender,
    DragEventArgs e)
{
    e.Handled = true;

    if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
    {
        AddFiles(paths);
    }
}


    private void RemoveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        int index = FilesListBox.SelectedIndex;

        if (index < 0)
        {
            return;
        }

        _files.RemoveAt(index);

        FilesListBox.SelectedIndex =
            _files.Count == 0
                ? -1
                : Math.Min(index, _files.Count - 1);

        UpdateButtons();
    }

    private void MoveUpButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MoveSelectedFile(-1);
    }

    private void MoveDownButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MoveSelectedFile(1);
    }

    private void MoveSelectedFile(int direction)
    {
        int index = FilesListBox.SelectedIndex;
        int destination = index + direction;

        if (index < 0 ||
            destination < 0 ||
            destination >= _files.Count)
        {
            return;
        }

        _files.Move(index, destination);

        FilesListBox.SelectedIndex = destination;
        FilesListBox.ScrollIntoView(_files[destination]);

        UpdateButtons();
    }

    private void FilesListBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        int index = FilesListBox.SelectedIndex;

        RemoveButton.IsEnabled = index >= 0;
        MoveUpButton.IsEnabled = index > 0;

        MoveDownButton.IsEnabled =
            index >= 0 &&
            index < _files.Count - 1;

        MergeButton.IsEnabled = _files.Count >= 2;
    }

    private void MergeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_files.Count < 2)
        {
            return;
        }

        DialogResult = true;
    }
}