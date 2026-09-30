using AetherForge.App.Services;
using System.Windows;

namespace AetherForge.App;

public partial class BatchFolderNameDialog : Window
{
    public string FolderName =>
        FolderNameTextBox.Text.Trim();

    public BatchFolderNameDialog()
    {
        InitializeComponent();

        Title =
            LanguageService.Get("BatchFolderDialogTitle");

        FolderNameTextBlock.Text =
            LanguageService.Get("BatchFolderNameText");

        OkButton.Content =
            LanguageService.Get("OkText");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        FolderNameTextBox.SelectAll();
        FolderNameTextBox.Focus();
    }

    private void OkButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            FolderNameTextBox.Text))
        {
            return;
        }

        DialogResult = true;
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}