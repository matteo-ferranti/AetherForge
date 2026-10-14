using System.IO;
using System.Text.Json;

namespace AetherForge.App.Services;

public static class UpdateSettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AetherForge", "update-settings.json");

    public static bool AutomaticChecksEnabled { get; private set; } =
        ReadPreference(SettingsPath);

    public static void SetAutomaticChecks(bool enabled)
    {
        WritePreference(SettingsPath, enabled);
        AutomaticChecksEnabled = enabled;
    }

    internal static bool ReadPreference(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return true;
            }

            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            return document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("automaticUpdateChecks", out var value) &&
                value.ValueKind is JsonValueKind.True or JsonValueKind.False
                    ? value.GetBoolean()
                    : true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return true;
        }
    }

    internal static void WritePreference(string path, bool enabled)
    {
        string directory = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(directory);
        string temporaryPath = Path.Combine(directory, $".updates-{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(temporaryPath,
                JsonSerializer.Serialize(new { automaticUpdateChecks = enabled }));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
            }
        }
    }
}
