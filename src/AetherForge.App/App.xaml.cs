using AetherForge.App.Services;
using System.Windows;

namespace AetherForge.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        LanguageService.SetLanguage("en");

        ImageSecurityService.Initialize();

        MainWindow window = new();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ImageSecurityService.Cleanup();

        base.OnExit(e);
    }
}