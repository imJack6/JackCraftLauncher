using Avalonia;
using Avalonia.Media;

namespace JackCraftLauncher;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://JackCraftLauncher/Assets/Fonts/JetBrainsMono-Medium.ttf#",
                FontFallbacks = new[]
                {
                    new FontFallback
                    {
                        FontFamily = new FontFamily("avares://JackCraftLauncher/Assets/Fonts/MSYHMONO.ttf#")
                    }
                }
            });
    }
}