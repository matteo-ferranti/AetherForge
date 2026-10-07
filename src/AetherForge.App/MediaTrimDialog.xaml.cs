using System.Globalization;
using System.Windows;

using AetherForge.App.Services;

namespace AetherForge.App;

public partial class MediaTrimDialog : Window
{
    private readonly TimeSpan _duration;

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public MediaTrimDialog(
        string fileName,
        TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        InitializeComponent();

        _duration = duration;

        string durationText =
            ((decimal)duration.Ticks / TimeSpan.TicksPerSecond)
            .ToString("0.#######", CultureInfo.InvariantCulture);

        Title = LanguageService.Get("OperationTrim");

        FileNameTextBlock.Text = string.Format(
            LanguageService.Get("MediaTrimFileLabel"),
            fileName);

        DurationTextBlock.Text = string.Format(
            LanguageService.Get("MediaTrimDurationLabel"),
            durationText);

        TimeInfoTextBlock.Text =
            LanguageService.Get("MediaTrimTimeInfo");

        StartLabel.Text =
            LanguageService.Get("MediaTrimStartLabel");

        EndLabel.Text =
            LanguageService.Get("MediaTrimEndLabel");

        EndTextBox.Text = durationText;

        CancelButton.Content =
            LanguageService.Get("CancelText");

        ContinueButton.Content =
            LanguageService.Get("ImageTransformContinue");

        Loaded += (_, _) =>
        {
            StartTextBox.Focus();
            StartTextBox.SelectAll();
        };
    }

    private bool TryParseTime(
        string text,
        out TimeSpan time)
    {
        time = TimeSpan.Zero;

        string normalized = text.Trim().Replace(',', '.');

        if (!decimal.TryParse(
                normalized,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal seconds))
        {
            return false;
        }

        decimal maximumSeconds =
            (decimal)_duration.Ticks / TimeSpan.TicksPerSecond;

        if (seconds < 0 || seconds > maximumSeconds)
        {
            return false;
        }

        long ticks = (long)(seconds * TimeSpan.TicksPerSecond);

        time = TimeSpan.FromTicks(ticks);

        return true;
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        bool validStart = TryParseTime(
            StartTextBox.Text,
            out TimeSpan start);

        bool validEnd = TryParseTime(
            EndTextBox.Text,
            out TimeSpan end);

        if (!validStart ||
            !validEnd ||
            end <= start)
        {
            ErrorTextBlock.Text =
                LanguageService.Get("MediaTrimInvalidRange");

            ErrorTextBlock.Visibility = Visibility.Visible;

            return;
        }

        StartTime = start;
        EndTime = end;

        DialogResult = true;
    }
}