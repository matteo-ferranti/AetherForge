using AetherForge.App.Services;
using System.Windows;

namespace AetherForge.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        LanguageService.SetLanguage("en");

        MainWindow window = new();
        window.Show();
    }
}