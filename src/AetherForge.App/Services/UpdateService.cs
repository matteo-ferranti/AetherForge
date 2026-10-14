using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace AetherForge.App.Services;

public sealed record UpdateCheckResult(
    Version LatestVersion,
    Uri ReleasePage,
    bool IsUpdateAvailable);

public static class UpdateService
{
    private const string LatestReleaseUrl =
        "https://api.github.com/repos/matteo-ferranti/AetherForge/releases/latest";

    public static Version CurrentVersion
    {
        get
        {
            Version version = typeof(UpdateService).Assembly.GetName().Version
                ?? new Version(1, 1, 0);
            return new Version(version.Major, version.Minor, Math.Max(0, version.Build));
        }
    }

    public static async Task<UpdateCheckResult> CheckAsync(
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClient();
        return await CheckAsync(client, CurrentVersion, cancellationToken);
    }

    internal static HttpClient CreateHttpClient(HttpMessageHandler? handler = null)
    {
        return new HttpClient(handler ?? new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = false
        })
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = 1024 * 1024
        };

    }

    internal static async Task<UpdateCheckResult> CheckAsync(
        HttpClient client,
        Version currentVersion,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, LatestReleaseUrl);
        request.Headers.UserAgent.ParseAdd($"AetherForge/{currentVersion.ToString(3)}");
        request.Headers.Accept.ParseAdd("application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

        using HttpResponseMessage response =
            await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        return ParseRelease(json, currentVersion);
    }

    internal static UpdateCheckResult ParseRelease(string json, Version currentVersion)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("tag_name", out JsonElement tagElement) ||
            tagElement.ValueKind != JsonValueKind.String ||
            !root.TryGetProperty("html_url", out JsonElement urlElement) ||
            urlElement.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException("Invalid release information.");
        }

        string tag = tagElement.GetString() ?? string.Empty;
        if (tag.StartsWith('v') || tag.StartsWith('V'))
        {
            tag = tag[1..];
        }

        if (!Version.TryParse(tag, out Version? version) ||
            version.Build < 0 || version.Revision > 0 ||
            !Uri.TryCreate(urlElement.GetString(), UriKind.Absolute, out Uri? page) ||
            page.Scheme != Uri.UriSchemeHttps ||
            !page.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
            !page.IsDefaultPort || page.UserInfo.Length != 0 ||
            !page.AbsolutePath.StartsWith(
                "/matteo-ferranti/AetherForge/releases/tag/", StringComparison.Ordinal))
        {
            throw new InvalidDataException("Invalid release version or page.");
        }

        Version latest = new(version.Major, version.Minor, version.Build);
        bool excluded =
            (root.TryGetProperty("draft", out JsonElement draft) &&
                draft.ValueKind == JsonValueKind.True) ||
            (root.TryGetProperty("prerelease", out JsonElement prerelease) &&
                prerelease.ValueKind == JsonValueKind.True);

        bool hasInstaller = false;
        string installerName = $"AetherForge-Setup-{latest.ToString(3)}-win-x64.exe";

        if (root.TryGetProperty("assets", out JsonElement assets) &&
            assets.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement asset in assets.EnumerateArray())
            {
                if (asset.ValueKind == JsonValueKind.Object &&
                    asset.TryGetProperty("name", out JsonElement name) &&
                    name.ValueKind == JsonValueKind.String &&
                    name.GetString() == installerName &&
                    asset.TryGetProperty("state", out JsonElement state) &&
                    state.ValueKind == JsonValueKind.String &&
                    state.GetString() == "uploaded" &&
                    asset.TryGetProperty("size", out JsonElement size) &&
                    size.ValueKind == JsonValueKind.Number &&
                    size.TryGetInt64(out long bytes) && bytes > 0)
                {
                    hasInstaller = true;
                    break;
                }
            }
        }

        return new UpdateCheckResult(
            latest, page, !excluded && hasInstaller && latest > currentVersion);
    }
}
