using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JackCraftLauncher.Views.MainMenus;

public partial class SettingMenu : UserControl
{
    public SettingMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}