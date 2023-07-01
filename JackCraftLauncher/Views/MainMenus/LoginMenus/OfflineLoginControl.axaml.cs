using Avalonia.Controls;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Launch.AuthenticatorHandler;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.Launch;

namespace JackCraftLauncher.Views.MainMenus.LoginMenus;

public partial class OfflineLoginControl : UserControl
{
    public OfflineLoginControl()
    {
        InitializeComponent();
    }

    private void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(UserNameTextBox.Text))
        {
            DialogHost.Show(Resources["EnterUserNameView"]!, "MainDialogHost");
        }
        else
        {
            var username = UserNameTextBox.Text!;
            AccountAuthenticatorHandler.Login(new OfflineAuthenticator
            {
                Username = username,
                LauncherAccountParser = new DefaultLauncherAccountParser(App.Core.RootPath!, App.Core.ClientToken)
            }, username);
        }
    }
}