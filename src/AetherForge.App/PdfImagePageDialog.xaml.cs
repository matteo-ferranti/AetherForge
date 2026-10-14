using System.Globalization;
using System.Windows;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class PdfImagePageDialog : Window
{
    private readonly int _pageCount;

    public int PageNumber { get; private set; } = 1;

    public PdfImagePageDialog(
        string fileName,
        int pageCount)
    {
        if (pageCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageCount));
        }

        InitializeComponent();

        _pageCount = pageCount;

        Title = LanguageService.Get("PdfImageDialogTitle");

        FileNameTextBlock.Text = string.Format(
            LanguageService.Get("PdfPagesFileLabel"),
            fileName);

        PageCountTextBlock.Text = string.Format(
            LanguageService.Get("PdfPageCountLabel"),
            pageCount);

        PageLabelTextBlock.Text =
            LanguageService.Get("PdfImagePageLabel");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ContinueButton.Content =
            LanguageService.Get("ImageTransformContinue");

        Loaded += (_, _) =>
        {
            PageNumberTextBox.Focus();
            PageNumberTextBox.SelectAll();
        };
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        bool validNumber = int.TryParse(
            PageNumberTextBox.Text.Trim(),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int pageNumber);

        if (!validNumber ||
            pageNumber < 1 ||
            pageNumber > _pageCount)
        {
            ErrorTextBlock.Text = string.Format(
                LanguageService.Get("PdfImagePageInvalid"),
                _pageCount);

            ErrorTextBlock.Visibility = Visibility.Visible;

            PageNumberTextBox.Focus();
            PageNumberTextBox.SelectAll();

            return;
        }

        PageNumber = pageNumber;

        DialogResult = true;
    }
}