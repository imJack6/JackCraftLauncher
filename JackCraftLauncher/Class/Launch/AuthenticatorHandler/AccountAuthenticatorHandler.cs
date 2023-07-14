using System.Threading.Tasks;
using DialogHostAvalonia;
using JackCraftLauncher.Class.ConfigHandler;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Models.LoginModels;
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
        DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode, LoginType.None);
        GlobalVariable.AccountAuthenticator = null!;
        MicrosoftLoginControl.Instance.LoginType1CancelLogin();
        LoginMenu.Instance.LoginTabControl.IsVisible = true;
        LoginMenu.Instance.LoginInGrid.IsVisible = false;
    }

    public static async Task Login(IAuthenticator authenticator, string username)
    {
        var loginAs = Localizer.Localizer.Instance["LoginAs"];
        AuthResultBase authResult = null!;
        if (authenticator is OfflineAuthenticator)
        {
            loginAs = string.Format(loginAs, Localizer.Localizer.Instance["OfflineLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode,
                LoginType.Offline);
        }
        else if (authenticator is MicrosoftAuthenticator)
        {
            loginAs = string.Format(loginAs, Localizer.Localizer.Instance["MicrosoftLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode,
                LoginType.Microsoft);
        }
        else if (authenticator is YggdrasilAuthenticator)
        {
            loginAs = string.Format(loginAs, Localizer.Localizer.Instance["ThirdPartyLogin"]);
            authResult = await authenticator.AuthTaskAsync(false);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode,
                LoginType.Yggdrasil);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.YggdrasilLoginAuthServerNode,
                ((YggdrasilAuthenticator)authenticator).AuthServer!);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.MicrosoftLoginRefreshTokenNode,
                ((YggdrasilAuthenticator)authenticator).Email);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.MicrosoftLoginRefreshTokenNode,
                EncryptHandler.JcEncrypt(((YggdrasilAuthenticator)authenticator).Password));
        }

        DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.UsernameNode, username);

        if (authResult.AuthStatus != AuthStatus.Succeeded)
        {
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode, LoginType.None);

            var loginFailErrorMessage = string.Format(Localizer.Localizer.Instance["LoginFailErrorMessage1"],
                authResult.AuthStatus, authResult.Error.Error,
                authResult.Error.ErrorMessage, authResult.Error.Cause);
            DialogHost.Show(new WarningTemplateModel(Localizer.Localizer.Instance["LoginFail"], loginFailErrorMessage),
                "MainDialogHost");
            MicrosoftLoginControl.Instance.LoginType1CancelLogin();
        }
        else
        {
            LoginMenu.Instance.UserNameTextBlock.Text = username;
            LoginMenu.Instance.LoginAsTextBlock.Text = loginAs;
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