using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Launch.AuthenticatorHandler;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.Launch;

namespace JackCraftLauncher.Views.MainMenus.LoginMenus;

public partial class ThirdPartyLoginControl : UserControl
{
    public ThirdPartyLoginControl()
    {
        InitializeComponent();
    }

    private async void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(AuthServerTextBox.Text))
        {
            DialogHost.Show(Resources["EnterAuthServerView"]!, "MainDialogHost");
        }
        else if (string.IsNullOrEmpty(UsernameEmailTextBox.Text))
        {
            DialogHost.Show(Resources["EnterUserNameOrEmailView"]!, "MainDialogHost");
        }
        else if (string.IsNullOrEmpty(PasswordTextBox.Text))
        {
            DialogHost.Show(Resources["EnterPasswordView"]!, "MainDialogHost");
        }
        else
        {
            DialogHost.Show(Resources["LoadingView"]!, "MainDialogHost");
            var yggdrasilAuthenticator = new YggdrasilAuthenticator
            {
                AuthServer = AuthServerTextBox.Text,
                Email = UsernameEmailTextBox.Text,
                LauncherAccountParser = new DefaultLauncherAccountParser(App.Core.RootPath!, App.Core.ClientToken),
                Password = PasswordTextBox.Text
            };
            var authResult = await yggdrasilAuthenticator.AuthTaskAsync();
            if (authResult.Error == null)
            {
                await AccountAuthenticatorHandler.Login(
                    new YggdrasilAuthenticator
                    {
                        AuthServer = AuthServerTextBox.Text,
                        Email = UsernameEmailTextBox.Text,
                        LauncherAccountParser =
                            new DefaultLauncherAccountParser(App.Core.RootPath!, App.Core.ClientToken),
                        Password = PasswordTextBox.Text
                    },
                    authResult.SelectedProfile!.Name);
                DialogHostUtils.Close();
            }
            else
            {
                DialogHostUtils.Close();
                await Task.Delay(1);
                var loginFailErrorMessage = string.Format(
                    Localizer.Localizer.Instance["LoginFailErrorMessage1"],
                    authResult.AuthStatus, authResult.Error.Error,
                    authResult.Error.ErrorMessage, authResult.Error.Cause);
                Console.WriteLine(loginFailErrorMessage);
                DialogHost.Show(
                    new WarningTemplateModel(Localizer.Localizer.Instance["LoginFail"], loginFailErrorMessage),
                    "MainDialogHost");
            }
        }
    }
}