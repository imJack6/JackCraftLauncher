using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JackCraftLauncher.Class;
using JackCraftLauncher.Class.ConfigHandler;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace JackCraftLauncher.Views.MainMenus;

public partial class SettingMenu : UserControl
{
    public SettingMenu()
    {
        InitializeComponent();
        Instance = this;
    }

    public static SettingMenu Instance { get; private set; } = null!;

    private void ThemeModeRadioButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var checkedButton = (Button)sender!;
        var materialTheme = Application.Current!.LocateMaterialTheme<MaterialTheme>();
        switch (checkedButton.Name)
        {
            case "ThemeFollowSystemModeRadioButton":
                materialTheme.BaseTheme = BaseThemeMode.Inherit;
                break;
            case "ThemeLightModeRadioButton":
                materialTheme.BaseTheme = BaseThemeMode.Light;
                break;
            case "ThemeDarkModeRadioButton":
                materialTheme.BaseTheme = BaseThemeMode.Dark;
                break;
        }

        DefaultConfigHandler.SetConfig(GlobalConstants.ConfigThemeNode,
            materialTheme.BaseTheme);
    }
}