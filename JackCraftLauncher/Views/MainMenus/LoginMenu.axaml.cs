using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JackCraftLauncher.Views.MainMenus;

public partial class LoginMenu : UserControl
{
    public LoginMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}