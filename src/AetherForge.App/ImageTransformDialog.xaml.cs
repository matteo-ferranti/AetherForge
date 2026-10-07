using System.Windows;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class ImageTransformDialog : Window
{
    private sealed record TransformChoice(
        int Value,
        string DisplayName);

    public int SelectedDegrees =>
        (OptionComboBox.SelectedItem as TransformChoice)?.Value
        ?? 90;

    public bool SelectedHorizontal =>
        (OptionComboBox.SelectedItem as TransformChoice)?.Value == 0;

    public ImageTransformDialog(
        string fileName,
        bool rotating)
    {
        InitializeComponent();

        Title = LanguageService.Get(
            rotating ? "OperationRotate" : "OperationFlip");

        FileNameTextBlock.Text = string.Format(
            LanguageService.Get("ImageTransformFileLabel"),
            fileName);

        OptionLabelTextBlock.Text = LanguageService.Get(
            rotating ? "RotationAngleLabel" : "FlipDirectionLabel");

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ContinueButton.Content =
            LanguageService.Get("ImageTransformContinue");

        TransformChoice[] choices = rotating
            ?
            [
                new(90, "90°"),
                new(180, "180°"),
                new(270, "270°")
            ]
            :
            [
                new(
                    0,
                    LanguageService.Get("FlipHorizontalLabel")),
                new(
                    1,
                    LanguageService.Get("FlipVerticalLabel"))
            ];

        OptionComboBox.ItemsSource = choices;
        OptionComboBox.SelectedIndex = 0;

        Loaded += (_, _) => OptionComboBox.Focus();
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (OptionComboBox.SelectedItem is not TransformChoice)
        {
            return;
        }

        DialogResult = true;
    }
}