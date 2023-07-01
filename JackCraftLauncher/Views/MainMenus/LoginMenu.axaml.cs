using Avalonia.Controls;
using Avalonia.Interactivity;
using JackCraftLauncher.Class.Launch.AuthenticatorHandler;

namespace JackCraftLauncher.Views.MainMenus;

public partial class LoginMenu : UserControl
{
    public LoginMenu()
    {
        InitializeComponent();
        Instance = this;
    }

    public static LoginMenu Instance { get; private set; } = null!;

    private void LogOutOfLoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        AccountAuthenticatorHandler.LogOut();
    }
}