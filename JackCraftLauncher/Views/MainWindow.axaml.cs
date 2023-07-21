using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using JackCraftLauncher.Class;
using JackCraftLauncher.Class.ConfigHandler;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Views.MainMenus;

namespace JackCraftLauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        Localizer.Localizer.Instance.LoadLanguage("zh-CN");
        DefaultConfigHandler.LoadSettingsConfig();
        Task.Run(async () =>
        {
            while (true)
            {
                #region 刷新内存占用

                if (SettingMenu.Instance is not null)
                {
                    GameHandler.RefreshStartMemory();
                    await Task.Delay(100);
                }

                #endregion
            }
        });
    }

    public static MainWindow Instance { get; private set; } = null!;

    private void TemplatedControl_OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        //SnackbarHost.Post("JackCraft Launcher", null, DispatcherPriority.Normal);
    }

    #region 菜单按钮事件

    private void MenuRadioButton_Click(object sender, RoutedEventArgs e)
    {
        var checkedButton = (Button)sender;
        switch (checkedButton.Name)
        {
            case "LoginRadioButton":
                MenuTabControl.SelectedIndex = 0;
                break;
            case "StartRadioButton":
                MenuTabControl.SelectedIndex = 1;
                break;
            case "DownloadRadioButton":
                MenuTabControl.SelectedIndex = 2;
                DownloadMenu.RefreshLocalMinecraftDownloadList();
                break;
            case "SettingRadioButton":
                MenuTabControl.SelectedIndex = 3;
                break;
        }
    }

    private async void StartGameButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoginRadioButton.IsEnabled = false;
        StartRadioButton.IsEnabled = false;
        DownloadRadioButton.IsEnabled = false;
        SettingRadioButton.IsEnabled = false;
        StartGameButton.IsEnabled = false;
        StartGameMenuItem.ClearStartGameLog();
        try
        {
            if (StartMenu.Instance!.EditionSelectTabControl.SelectedIndex == 0)
            {
                if (StartMenu.Instance.LocalGameListBox.SelectedIndex == -1)
                {
                    DialogHost.Show(
                        new WarningTemplateModel(Localizer.Localizer.Instance["NoVersionSelected"],
                            Localizer.Localizer.Instance["SelectJavaEditionGameVersion"]), "MainDialogHost");
                }
                else
                {
                    if (SettingMenu.Instance.StartJavaSelectComboBox.SelectedIndex == -1)
                    {
                        await DialogHost.Show(
                            new WarningTemplateModel(Localizer.Localizer.Instance["NotSelectJava"],
                                Localizer.Localizer.Instance["SettingNotSelectJava"]), "MainDialogHost");
                        SettingRadioButton.IsChecked = true;
                        MenuTabControl.SelectedIndex = 3;
                    }
                    else
                    {
                        if (GlobalVariable.AccountAuthenticator == null)
                        {
                            DialogHost.Show(
                                new WarningTemplateModel(Localizer.Localizer.Instance["NotLogin"],
                                    Localizer.Localizer.Instance["NoAccountLogin"]), "MainDialogHost");
                        }
                        else
                        {
                            MenuTabControl.SelectedIndex = 4;
                            await GameHandler.StartGame(
                                GlobalVariable.LocalGameList[StartMenu.Instance.LocalGameListBox.SelectedIndex]);
                        }
                    }
                }
            }
            else if (StartMenu.Instance.EditionSelectTabControl.SelectedIndex == 1)
            {
                await GameHandler.CheckMCBedrockInstalled();
                if (StartMenu.Instance.NotFoundMinecraftBedrockEditionTextBlock.IsVisible)
                {
                    await Task.Delay(1);
                    DialogHost.Show(
                        new WarningTemplateModel(Localizer.Localizer.Instance["MinecraftBedrockEditionNotInstalled"],
                            Localizer.Localizer.Instance["NotFoundMinecraftBedrockEdition"]), "MainDialogHost");
                }
                else
                {
                    MenuTabControl.SelectedIndex = 4;
                    await GameHandler.StartBedrockGame();
                }
            }
        }
        finally
        {
            LoginRadioButton.IsEnabled = true;
            StartRadioButton.IsEnabled = true;
            DownloadRadioButton.IsEnabled = true;
            SettingRadioButton.IsEnabled = true;
            StartGameButton.IsEnabled = true;
        }
    }

    #endregion

    #region 窗体控制

    private void WindowCloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void WindowMinimizedButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void WindowMaximizedButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
        else if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
    }

    private void TitleBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MoveDragWindow(e);
    }

    private void DialogHost_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DialogHost.IsDialogOpen("MainDialogHost"))
            MoveDragWindow(e);
    }

    private void MoveDragWindow(PointerPressedEventArgs e)
    {
        if (e.ClickCount <= 1)
            BeginMoveDrag(e);
        else if (e.ClickCount == 2)
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
    }

    #endregion
}