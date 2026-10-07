using System.Globalization;
using System.Windows;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class PdfPageSelectionDialog : Window
{
    private readonly int _pageCount;

    public int FirstPage { get; private set; } = 1;

    public int LastPage { get; private set; } = 1;

    public PdfPageSelectionDialog(
        string fileName,
        int pageCount)
    {
        if (pageCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageCount));
        }

        InitializeComponent();

        _pageCount = pageCount;

        Title = LanguageService.Get("OperationExtractPages");

        FileNameTextBlock.Text = string.Format(
            LanguageService.Get("PdfPagesFileLabel"),
            fileName);

        PageCountTextBlock.Text = string.Format(
            LanguageService.Get("PdfPageCountLabel"),
            pageCount);

        FirstPageLabel.Text =
            LanguageService.Get("PdfFirstPageLabel");

        LastPageLabel.Text =
            LanguageService.Get("PdfLastPageLabel");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ExtractButton.Content =
            LanguageService.Get("OperationExtractPages");

        Loaded += (_, _) =>
        {
            FirstPageTextBox.Focus();
            FirstPageTextBox.SelectAll();
        };
    }

    private void ExtractButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        bool validFirstPage = int.TryParse(
            FirstPageTextBox.Text.Trim(),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int firstPage);

        bool validLastPage = int.TryParse(
            LastPageTextBox.Text.Trim(),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int lastPage);

        if (!validFirstPage ||
            !validLastPage ||
            firstPage < 1 ||
            lastPage < firstPage ||
            lastPage > _pageCount)
        {
            ErrorTextBlock.Text =
                LanguageService.Get("PdfPageRangeInvalid");

            ErrorTextBlock.Visibility = Visibility.Visible;

            return;
        }

        FirstPage = firstPage;
        LastPage = lastPage;

        DialogResult = true;
    }
}