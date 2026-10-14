using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace AetherForge.App.Services;

public static class UpdateDialogService
{
    private static Task<UpdateCheckResult>? _pendingCheck;
    private static bool _manualCheckPending;
    private static Version? _lastAutomaticNotification;

    public static async Task CheckAndNotifyAsync(
        Window owner,
        bool automatic = false,
        Func<bool>? canNotify = null)
    {
        if (!automatic)
        {
            if (_manualCheckPending)
            {
                return;
            }
            _manualCheckPending = true;
        }

        try
        {
            // Both callers run on the WPF UI thread and can share one pending request.
            Task<UpdateCheckResult> request = _pendingCheck ??= UpdateService.CheckAsync();
            UpdateCheckResult result;
            try
            {
                result = await request;
            }
            finally
            {
                if (ReferenceEquals(_pendingCheck, request))
                {
                    _pendingCheck = null;
                }
            }

            if (!owner.IsVisible ||
                (automatic && (!UpdateSettingsService.AutomaticChecksEnabled ||
                    _manualCheckPending || canNotify?.Invoke() == false)))
            {
                return;
            }

            if (!result.IsUpdateAvailable)
            {
                if (!automatic)
                {
                    MessageBox.Show(owner,
                        LanguageService.Get("UpdateNoUpdate"),
                        LanguageService.Get("UpdateDialogTitle"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return;
            }

            if (automatic && _lastAutomaticNotification == result.LatestVersion)
            {
                return;
            }
            _lastAutomaticNotification = result.LatestVersion;

            MessageBoxResult choice = MessageBox.Show(owner,
                string.Format(LanguageService.Get("UpdateAvailable"),
                    result.LatestVersion.ToString(3)),
                LanguageService.Get("UpdateDialogTitle"),
                MessageBoxButton.YesNo, MessageBoxImage.Information,
                MessageBoxResult.No);

            if (choice == MessageBoxResult.Yes)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = result.ReleasePage.AbsoluteUri,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
                {
                    ShowError(owner, "UpdateOpenPageError");
                }
            }
        }
        catch (Exception ex) when (
            ex is HttpRequestException or OperationCanceledException
                or JsonException or InvalidDataException)
        {
            if (!automatic && owner.IsVisible)
            {
                ShowError(owner, "UpdateCheckFailed");
            }
        }
        finally
        {
            if (!automatic)
            {
                _manualCheckPending = false;
            }
        }
    }

    private static void ShowError(Window owner, string key)
    {
        MessageBox.Show(owner, LanguageService.Get(key),
            LanguageService.Get("UpdateDialogTitle"),
            MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
