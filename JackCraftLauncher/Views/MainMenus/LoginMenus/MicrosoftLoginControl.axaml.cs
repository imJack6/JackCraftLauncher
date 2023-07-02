using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DialogHostAvalonia;
using JackCraftLauncher.Class;
using JackCraftLauncher.Class.Launch.AuthenticatorHandler;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.Class.Model.Auth;
using ProjBobcat.Class.Model.Microsoft.Graph;
using ProjBobcat.DefaultComponent.Authenticator;

namespace JackCraftLauncher.Views.MainMenus.LoginMenus;

public partial class MicrosoftLoginControl : UserControl
{
    private static CancellationTokenSource _loginType1CancellationTokenSource = new();
    private static CancellationToken _loginType1CancellationToken = _loginType1CancellationTokenSource.Token;

    public MicrosoftLoginControl()
    {
        InitializeComponent();
        Instance = this;
    }

    public static MicrosoftLoginControl Instance { get; private set; } = null!;

    #region 任意登录方式

    private void LoadingAndCancelLoginViewCancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoginType1CancelLogin();
        DialogHostUtils.Close();
    }

    #endregion

    #region 登录方式一

    private void DeviceTokenNotifier(DeviceIdResponseModel deviceTokenNotifier)
    {
        //var currentTime = DateTime.Now;
        var expiresInDateTime = DateTime.Now.AddSeconds(deviceTokenNotifier.ExpiresIn);
        var expireInSecond = string.Format(Localizer.Localizer.Instance["ExpireInSecond"],
            deviceTokenNotifier.ExpiresIn, expiresInDateTime);
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            LoginType1AddressTextBox.Text = deviceTokenNotifier.VerificationUri;
            LoginType1CodeTextBox.Text = deviceTokenNotifier.UserCode;
            LoginType1CodeExpiresInTextBox.Text = expireInSecond;
            DialogHostUtils.Close();
        });
        /*Task.Run(async () =>
        {
            // 计算当前时间到目标时间的时间间隔
            TimeSpan delay = expiresInDateTime - currentTime;
            if (delay > TimeSpan.Zero)
            {
                // 如果目标时间还没有到，等待相应的时间
                await Task.Delay(delay);
            }
            // 到达指定时间，执行后续的代码
        });*/
    }

    private async void LoginType1LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        DialogHost.Show(Resources["LoadingAndCancelLoginView"]!, "MainDialogHost");
        _loginType1CancellationTokenSource.Cancel();
        _loginType1CancellationTokenSource = new CancellationTokenSource();
        _loginType1CancellationToken = _loginType1CancellationTokenSource.Token;
        var microsoftAuthenticator = new MicrosoftAuthenticator();
        var task = Task.Run(async () =>
        {
            var graphAuthResultModel = await microsoftAuthenticator.GetMSAuthResult(DeviceTokenNotifier);
            if (graphAuthResultModel is null)
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    DialogHostUtils.Close();
                    await Task.Delay(1);
                    DialogHost.Show(
                        new WarningTemplateModel(Localizer.Localizer.Instance["LoginFail"],
                            Localizer.Localizer.Instance["LoginErrorOrTimeOut"]), "MainDialogHost");
                    LoginType1CancelLogin();
                });
            else
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    DialogHost.Show(Resources["LoadingView"]!, "MainDialogHost");
                    microsoftAuthenticator = new MicrosoftAuthenticator
                    {
                        CacheTokenProvider = async () => (true, graphAuthResultModel),
                        LauncherAccountParser = App.Core.VersionLocator.LauncherAccountParser
                    };
                    var authResult =
                        (MicrosoftAuthResult)await microsoftAuthenticator.AuthTaskAsync();
                    if (authResult.Error == null)
                    {
                        GlobalVariable.MicrosoftLogin.CurrentAuthTime = authResult.CurrentAuthTime;
                        GlobalVariable.MicrosoftLogin.ExpiresIn = authResult.ExpiresIn;
                        GlobalVariable.MicrosoftLogin.RefreshToken = authResult.RefreshToken;
                        AuthenticatorVerify authVerify = new()
                        {
                            XblRefreshToken = GlobalVariable.MicrosoftLogin.RefreshToken,
                            LastRefreshedTime = GlobalVariable.MicrosoftLogin.CurrentAuthTime,
                            ExpiresIn = GlobalVariable.MicrosoftLogin.ExpiresIn
                        };
                        await AccountAuthenticatorHandler.Login(
                            new MicrosoftAuthenticator
                            {
                                CacheTokenProvider = authVerify.CacheTokenProviderAsync,
                                Email = authResult.Email,
                                LauncherAccountParser = App.Core.VersionLocator.LauncherAccountParser
                            },
                            authResult.User.UserName);
                        DialogHostUtils.Close();
                    }
                    else
                    {
                        DialogHostUtils.Close();
                    }
                });
        }, _loginType1CancellationToken);
        NoLoginType1StackPanel.IsVisible = false;
        LoginType1StackPanel.IsVisible = true;
        await task;
    }

    private void LoginType1CancelLoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoginType1CancelLogin();
    }

    public void LoginType1CancelLogin()
    {
        _loginType1CancellationTokenSource.Cancel();
        _loginType1CancellationTokenSource = new CancellationTokenSource();
        _loginType1CancellationToken = _loginType1CancellationTokenSource.Token;
        NoLoginType1StackPanel.IsVisible = true;
        LoginType1StackPanel.IsVisible = false;
        LoginType1AddressTextBox.Text = string.Empty;
        LoginType1CodeTextBox.Text = string.Empty;
        LoginType1CodeExpiresInTextBox.Text = string.Empty;
    }

    private void LoginType1CopyAddressButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginType1AddressTextBox.Text))
        {
            DialogHost.Show(
                new WarningTemplateModel(Localizer.Localizer.Instance["CopyFail"],
                    Localizer.Localizer.Instance["LoginAddressIsEmpty"]), "MainDialogHost");
        }
        else
        {
            if (MainWindow.Instance!.Clipboard != null)
                ClipboardUtils.SetTextAsync(LoginType1AddressTextBox.Text);
        }
    }

    private void LoginType1OpenAddressButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginType1AddressTextBox.Text))
            DialogHost.Show(
                new WarningTemplateModel(Localizer.Localizer.Instance["OpenFail"],
                    Localizer.Localizer.Instance["LoginAddressIsEmpty"]), "MainDialogHost");
        else
            Process.Start(new ProcessStartInfo(LoginType1AddressTextBox.Text) { UseShellExecute = true });
    }

    private void LoginType1CopyCodeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginType1CodeTextBox.Text))
        {
            DialogHost.Show(
                new WarningTemplateModel(Localizer.Localizer.Instance["CopyFail"],
                    Localizer.Localizer.Instance["LoginCodeIsEmpty"]), "MainDialogHost");
        }
        else
        {
            if (MainWindow.Instance!.Clipboard != null)
                ClipboardUtils.SetTextAsync(LoginType1CodeTextBox.Text);
        }
    }

    #endregion
}