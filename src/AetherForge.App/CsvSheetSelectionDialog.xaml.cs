using System.Windows;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class CsvSheetSelectionDialog : Window
{
    public SpreadsheetSheet? SelectedSheet =>
        SheetComboBox.SelectedItem as SpreadsheetSheet;

    public CsvSheetSelectionDialog(
        string fileName,
        IReadOnlyList<SpreadsheetSheet> sheets)
    {
        InitializeComponent();


        Title = LanguageService.Get("CsvSheetDialogTitle");

        FileNameTextBlock.Text = string.Format(
            LanguageService.Get("CsvSheetFileLabel"),
            fileName);

        SheetLabelTextBlock.Text =
            LanguageService.Get("CsvSheetSelectionLabel");

        ExportInfoTextBlock.Text =
            LanguageService.Get("CsvExportInfo");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ExportButton.Content =
            LanguageService.Get("CsvExportButton");

        SheetComboBox.ItemsSource = sheets;
        SheetComboBox.SelectedIndex = 0;

        Loaded += (_, _) => SheetComboBox.Focus();
    }

    private void ExportButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (SelectedSheet is null)
        {
            return;
        }

        DialogResult = true;
    }
}