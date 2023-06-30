using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Views;
using ProjBobcat.DefaultComponent.Launch.GameCore;

namespace JackCraftLauncher;

public class App : Application
{
    public static DefaultGameCore Core { get; private set; } = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Core = LaunchHandler.InitLaunch();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
        
        base.OnFrameworkInitializationCompleted();
    }
}