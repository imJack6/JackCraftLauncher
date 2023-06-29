using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JackCraftLauncher.Views.MainMenus;

public partial class DownloadMenu : UserControl
{
    public DownloadMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}