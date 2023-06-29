using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using JackCraftLauncher.Class.Launch;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        Localizer.Localizer.Instance.LoadLanguage("zh-CN");
    }

    public static MainWindow? Instance { get; private set; }

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
        MenuTabControl.SelectedIndex = 4;
        StartGameMenuItem.ClearStartGameLog();
        try
        {
            await GameHandler.StartGame(new VersionInfo());
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