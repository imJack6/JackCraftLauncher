using System.Threading.Tasks;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Views.MainMenus;
using JackCraftLauncher.Views.MainMenus.LoginMenus;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.Auth;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.Interface;

namespace JackCraftLauncher.Class.Launch.AuthenticatorHandler;

public class AccountAuthenticatorHandler
{
    public static string AccessToken =>
        GlobalVariable.AccountAuthenticator?.GetLastAuthResult().AccessToken ?? string.Empty;

    public static void LogOut()
    {
        GlobalVariable.AccountAuthenticator = null!;
        MicrosoftLoginControl.Instance.LoginType1CancelLogin();
        LoginMenu.Instance.LoginTabControl.IsVisible = true;
        LoginMenu.Instance.LoginInGrid.IsVisible = false;
    }

    public static async Task Login(IAuthenticator authenticator, string username)
    {
        var LoginAs = Localizer.Localizer.Instance["LoginAs"];
        AuthResultBase authResult = null!;
        if (authenticator is OfflineAuthenticator)
        {
            LoginAs = string.Format(LoginAs, Localizer.Localizer.Instance["OfflineLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
        }
        else if (authenticator is MicrosoftAuthenticator)
        {
            LoginAs = string.Format(LoginAs, Localizer.Localizer.Instance["MicrosoftLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
        }
        else if (authenticator is YggdrasilAuthenticator)
        {
            LoginAs = string.Format(LoginAs, Localizer.Localizer.Instance["ThirdPartyLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
        }

        if (authResult.AuthStatus != AuthStatus.Succeeded)
        {
            var loginFailErrorMessage = string.Format(Localizer.Localizer.Instance["LoginFailErrorMessage1"], authResult.AuthStatus, authResult.Error.Error,
                authResult.Error.ErrorMessage, authResult.Error.Cause);
            DialogHost.Show(new WarningTemplateModel(Localizer.Localizer.Instance["LoginFail"], loginFailErrorMessage),
                "MainDialogHost");
            MicrosoftLoginControl.Instance.LoginType1CancelLogin();
        }
        else
        {
            LoginMenu.Instance.UserNameTextBlock.Text = username;
            LoginMenu.Instance.LoginAsTextBlock.Text = LoginAs;
            GlobalVariable.AccountAuthenticator = authenticator;
            LoginMenu.Instance.LoginTabControl.IsVisible = false;
            LoginMenu.Instance.LoginInGrid.IsVisible = true;
        }
    }
    /*public static async Task<AuthResultBase> Verify(IAuthenticator authenticator)
    {
        AuthResultBase authResult = null!;
        if (authenticator is OfflineAuthenticator)
        {
            authResult = await authenticator.AuthTaskAsync(false);
        }
        else if (authenticator is MicrosoftAuthenticator)
        {
            authResult = await authenticator.AuthTaskAsync(false);
        }
        else if (authenticator is YggdrasilAuthenticator)
        {
            authResult = await authenticator.AuthTaskAsync(false);
        }
        return authResult;
    }*/
}