using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JackCraftLauncher.Views.MainMenus;

public partial class StartMenu : UserControl
{
    public StartMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}