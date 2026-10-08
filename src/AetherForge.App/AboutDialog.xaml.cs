using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AetherForge.App.Services;

namespace AetherForge.App;

public partial class AboutDialog : Window
{
    private sealed record LicenseComponent(
        string Name,
        string[] Files);

    public AboutDialog()
    {
        InitializeComponent();

        Title = LanguageService.Get("AboutDialogTitle");

        InformationTab.Header =
            LanguageService.Get("AboutInformationTab");

        LicensesTab.Header =
            LanguageService.Get("AboutLicensesTab");

        LicensesExplanationTextBlock.Text =
            LanguageService.Get("AboutLicensesExplanation");
        
        AppLicenseTab.Header =
    LanguageService.Get("AboutAppLicenseTab");

AppLicenseExplanationTextBlock.Text =
    LanguageService.Get("AboutAppLicenseExplanation");

string appLicensePath = Path.Combine(
    AppContext.BaseDirectory,
    "Assets",
    "Licenses",
    "AetherForge-LICENSE.txt");

try
{
    AppLicenseTextBox.Text =
        File.ReadAllText(appLicensePath);
}
catch (Exception ex) when (
    ex is IOException or UnauthorizedAccessException)
{
    AppLicenseTextBox.Text =
        LanguageService.Get("AboutLicenseReadError");
}

        string version =
            typeof(App).Assembly.GetName().Version?.ToString(3)
            ?? "1.0.0";

        VersionTextBlock.Text = string.Format(
            LanguageService.Get("AboutVersionText"),
            version);

        AboutIntroductionRun.Text =
            LanguageService.Get("AboutDescription");

        AboutFeaturesHeading.Text =
            LanguageService.Get("AboutFeaturesHeading");

        AboutConversionHeadingRun.Text =
            LanguageService.Get("AboutConversionHeading");

        AboutConversionBodyRun.Text =
            LanguageService.Get("AboutConversionBody");

        AboutEditingHeadingRun.Text =
            LanguageService.Get("AboutEditingHeading");

        AboutEditingBodyRun.Text =
            LanguageService.Get("AboutEditingBody");

        AboutPdfHeadingRun.Text =
            LanguageService.Get("AboutPdfHeading");

        AboutPdfBodyRun.Text =
            LanguageService.Get("AboutPdfBody");

        AboutPrivacyHeadingRun.Text =
            LanguageService.Get("AboutPrivacyHeading");

        AboutPrivacyBodyRun.Text =
            LanguageService.Get("AboutPrivacyBody");

        AboutFooterTextBlock.Text =
            LanguageService.Get("AboutFooter");

        RepositoryButton.Content =
            LanguageService.Get("AboutRepositoryText");

        WebsiteButton.Content =
            LanguageService.Get("AboutWebsiteText");

        CloseButton.Content =
            LanguageService.Get("AboutCloseText");

        ComponentsListBox.ItemsSource = new LicenseComponent[]
        {
            new(
                "FFmpeg / FFprobe 9.0.2",
                ["FFmpeg-GPLv3-LICENSE.txt"]),

            new(
                "LibreOffice 26.8.0.3",
                [
                    "LibreOffice-LICENSE.txt",
                    "LibreOffice-NOTICE.txt"
                ]),

            new(
                "Magick.NET 14.17.2",
                [
                    "Magick.NET-LICENSE.txt",
                    "Magick.NET-NOTICE.txt"
                ]),

            new(
                "Noto Sans",
                ["NotoSans-LICENSE.txt"]),

            new(
                "Open XML SDK 3.5.1",
                ["OpenXML-SDK-LICENSE.txt"]),

            new(
                "PdfPig 0.1.14",
                ["PdfPig-LICENSE.txt"]),

            new(
                "qpdf 12.4.2",
                [
                    "qpdf-LICENSE.txt",
                    "qpdf-NOTICE.txt"
                ])
        };

        ComponentsListBox.SelectedIndex = 0;
    }

    private void ComponentsListBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (ComponentsListBox.SelectedItem
            is not LicenseComponent component)
        {
            return;
        }

        StringBuilder text = new();

        foreach (string fileName in component.Files)
        {
            if (text.Length > 0)
            {
                text.AppendLine();
                text.AppendLine();
            }

            text.AppendLine(fileName);
            text.AppendLine(new string('─', 40));
            text.AppendLine();

            string path = Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "Licenses",
                fileName);

            try
            {
                text.Append(File.ReadAllText(path));
            }
            catch (Exception ex) when (
                ex is IOException or UnauthorizedAccessException)
            {
                text.Append(
                    LanguageService.Get("AboutLicenseReadError"));
            }
        }

        LicenseTextBox.Text = text.ToString();
        LicenseTextBox.ScrollToHome();
    }


    private void WebsiteButton_Click(
    object sender,
    RoutedEventArgs e)
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = LanguageService.CurrentLanguage == "it"
            ? "https://aetherforgeconverter.pages.dev/it/"
            : "https://aetherforgeconverter.pages.dev/en/",
            UseShellExecute = true
        });
    }
    catch (Exception ex) when (
        ex is Win32Exception or InvalidOperationException)
    {
        MessageBox.Show(
            this,
            LanguageService.Get("AboutWebsiteError"),
            LanguageService.Get("AboutDialogTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}

    private void RepositoryButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName =
                    "https://github.com/matteo-ferranti/AetherForge",
                UseShellExecute = true
            });
        }
        catch (Exception ex) when (
            ex is Win32Exception or InvalidOperationException)
        {
            MessageBox.Show(
                this,
                LanguageService.Get("AboutRepositoryError"),
                LanguageService.Get("AboutDialogTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}