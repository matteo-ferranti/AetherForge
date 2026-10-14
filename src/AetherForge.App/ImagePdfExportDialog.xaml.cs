using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using AetherForge.App.Models;
using AetherForge.App.Services;

namespace AetherForge.App;

public partial class ImagePdfExportDialog : Window
{
    private readonly ObservableCollection<LoadedFile> _files = [];

    public bool MergeIntoSinglePdf =>
        MergeRadioButton.IsChecked == true;

    public IReadOnlyList<LoadedFile> SelectedImages =>
        _files.ToArray();

    public ImagePdfExportDialog(
        IEnumerable<LoadedFile> files)
    {
        InitializeComponent();

        foreach (LoadedFile file in files)
        {
            _files.Add(file);
        }

        Title = LanguageService.Get("ImagePdfExportTitle");

        SeparateRadioButton.Content =
            LanguageService.Get("ImagePdfExportSeparate");

        MergeRadioButton.Content =
            LanguageService.Get("ImagePdfExportCombined");

        InfoTextBlock.Text =
            LanguageService.Get("ImagePdfExportInfo");

        RemoveButton.Content =
            LanguageService.Get("PdfMergeRemove");

        MoveUpButton.Content =
            LanguageService.Get("PdfMergeMoveUp");

        MoveDownButton.Content =
            LanguageService.Get("PdfMergeMoveDown");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ContinueButton.Content =
            LanguageService.Get("ImageTransformContinue");

        FilesListBox.ItemsSource = _files;

        SeparateRadioButton.Checked += (_, _) => UpdateControls();
        MergeRadioButton.Checked += (_, _) => UpdateControls();

        UpdateControls();
    }

    private void UpdateControls()
    {
        bool merge = MergeIntoSinglePdf;

        int index = FilesListBox.SelectedIndex;

        bool hasSelection =
            index >= 0 && index < _files.Count;

        FilesListBox.IsEnabled = merge;

        RemoveButton.IsEnabled =
            merge && hasSelection;

        MoveUpButton.IsEnabled =
            merge && hasSelection && index > 0;

        MoveDownButton.IsEnabled =
            merge && hasSelection && index < _files.Count - 1;

        ContinueButton.IsEnabled =
            !merge || _files.Count >= 2;
    }

    private void FilesListBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        UpdateControls();
    }

    private void RemoveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        int index = FilesListBox.SelectedIndex;

        if (index < 0 || index >= _files.Count)
        {
            return;
        }

        _files.RemoveAt(index);

        FilesListBox.SelectedIndex =
            _files.Count == 0
                ? -1
                : Math.Min(index, _files.Count - 1);

        UpdateControls();
    }

    private void MoveUpButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        int index = FilesListBox.SelectedIndex;

        if (index <= 0 || index >= _files.Count)
        {
            return;
        }

        _files.Move(index, index - 1);
        FilesListBox.SelectedIndex = index - 1;

        UpdateControls();
    }

    private void MoveDownButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        int index = FilesListBox.SelectedIndex;

        if (index < 0 || index >= _files.Count - 1)
        {
            return;
        }

        _files.Move(index, index + 1);
        FilesListBox.SelectedIndex = index + 1;

        UpdateControls();
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (MergeIntoSinglePdf && _files.Count < 2)
        {
            return;
        }

        DialogResult = true;
    }
}