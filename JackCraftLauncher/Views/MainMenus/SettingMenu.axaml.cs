using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using JackCraftLauncher.Class;
using JackCraftLauncher.Class.ConfigHandler;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models.SettingModels;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Views.MainMenus;

public partial class SettingMenu : UserControl
{
    public SettingMenu()
    {
        InitializeComponent();
        Instance = this;
    }

    public static SettingMenu Instance { get; private set; } = null!;

    #region 个性化

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

        DefaultConfigHandler.SetConfig(DefaultConfigConstants.LauncherSettingsNodes.ThemeNode,
            materialTheme.BaseTheme);
    }

    #endregion

    #region 游戏

    private async void RefreshLocalJavaComboBoxFullSearch_OnClick(object? sender, RoutedEventArgs e)
    {
        await ListHandler.RefreshLocalJavaList(true);
    }

    private async void RefreshLocalJavaComboBoxNormalSearch_OnClick(object? sender, RoutedEventArgs e)
    {
        await ListHandler.RefreshLocalJavaList();
    }

    private void GcTypeSelectComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (GcTypeSelectComboBox != null)
        {
            GlobalVariable.Config.GameGcType = (GcType)GcTypeSelectComboBox.SelectedIndex;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.GcTypeNode,
                GlobalVariable.Config.GameGcType);
        }
    }

    private void ResolutionDigitsOnly_OnTextChanging(object? sender, TextChangingEventArgs e)
    {
        var textBox = (sender as TextBox)!;
        textBox.Text = Regex.Replace(textBox.Text!, "[^0-9]", "");
        if (textBox.Text.Equals(""))
            textBox.Text = "1";
        switch (textBox.Name)
        {
            case "GameResolutionWidthTextBox":
                GlobalVariable.Config.GameResolutionWidth = Convert.ToUInt32(textBox.Text);
                DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.ResolutionWidthNode,
                    GlobalVariable.Config.GameResolutionWidth);
                break;
            case "GameResolutionHeightTextBox":
                GlobalVariable.Config.GameResolutionHeight = Convert.ToUInt32(textBox.Text);
                DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.ResolutionHeightNode,
                    GlobalVariable.Config.GameResolutionHeight);
                break;
        }
    }

    private void StartMemorySlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (StartMemorySlider != null)
        {
            GlobalVariable.Config.StartMemory = (uint)(StartMemorySlider.Value * 1024);
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.StartMemoryNode,
                GlobalVariable.Config.StartMemory);
        }
    }

    private void AutoConfigStartMemoryRadioButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AutoConfigStartMemoryRadioButton != null)
        {
            GameHandler.RefreshStartMemory();
            GlobalVariable.Config.StartMemoryType = StartMemoryType.Auto;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.StartMemoryTypeNode,
                GlobalVariable.Config.StartMemoryType);
        }
    }

    private void CustomStartMemoryRadioButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CustomStartMemoryRadioButton != null)
        {
            GameHandler.RefreshStartMemory();
            GlobalVariable.Config.StartMemoryType = StartMemoryType.Custom;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.StartMemoryTypeNode,
                GlobalVariable.Config.StartMemoryType);
        }
    }

    #endregion

    #region 下载

    private void StartJavaSelectComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (StartJavaSelectComboBox != null)
        {
            GlobalVariable.Config.GameStartJavaIndex = StartJavaSelectComboBox.SelectedIndex;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.SelectedJavaIndexNode,
                GlobalVariable.Config.GameStartJavaIndex);
        }
    }

    private void DownloadSourceSelectComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DownloadSourceSelectComboBox != null && DownloadSourceSelectComboBox.SelectedIndex >= 0)
        {
            GlobalVariable.Config.DownloadSourceEnum =
                (DownloadSourceHandler.DownloadSourceEnum)DownloadSourceSelectComboBox.SelectedIndex;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.DownloadSettingsNodes.DownloadSourceNode,
                GlobalVariable.Config.DownloadSourceEnum);
        }
    }

    private void DownloadMaxDegreeOfParallelismCountSlider_OnPointerCaptureLost(object? sender,
        PointerCaptureLostEventArgs e)
    {
        if (DownloadMaxDegreeOfParallelismCountSlider != null)
        {
            GlobalVariable.Config.DownloadMaxDegreeOfParallelismCount =
                (int)DownloadMaxDegreeOfParallelismCountSlider.Value;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.DownloadSettingsNodes.MaxDegreeOfParallelismCountNode,
                GlobalVariable.Config.DownloadMaxDegreeOfParallelismCount);
        }
    }

    private void DownloadSegmentsForLargeFileSlider_OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (DownloadSegmentsForLargeFileSlider != null)
        {
            GlobalVariable.Config.DownloadPartsCount = (int)DownloadSegmentsForLargeFileSlider.Value;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.DownloadSettingsNodes.PartsCountNode,
                GlobalVariable.Config.DownloadPartsCount);
        }
    }

    private void DownloadTotalRetrySlider_OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (DownloadTotalRetrySlider != null)
        {
            GlobalVariable.Config.DownloadRetryCount = (int)DownloadTotalRetrySlider.Value;
            DefaultConfigHandler.SetConfig(DefaultConfigConstants.DownloadSettingsNodes.RetryCountNode,
                GlobalVariable.Config.DownloadRetryCount);
        }
    }

    #endregion
}