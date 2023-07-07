using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Launch.AuthenticatorHandler;
using JackCraftLauncher.Class.Models.ErrorModels;
using JackCraftLauncher.Class.Models.LoginModels;
using JackCraftLauncher.Class.Utils;
using JackCraftLauncher.Views.MainMenus;
using JackCraftLauncher.Views.MyWindow;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ProjBobcat.Class.Model;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.Interface;

namespace JackCraftLauncher.Class.ConfigHandler;

public class DefaultConfigHandler
{
    private static readonly string ConfigFilePath = GlobalVariable.Config.MainConfigPath;

    #region 加载配置文件

    public static void LoadSettingsConfig()
    {
        LoadLauncherConfig();
        LoadDownloadConfig();
        LoadGameConfig();
        LoadLoginConfig();
    }

    private static void LoadLauncherConfig()
    {
        LoadThemeConfig();
    }

    private static void LoadDownloadConfig()
    {
        LoadDownloadSourceConfig();
        LoadDownloadMaxDegreeOfParallelismCountConfig();
        LoadDownloadPartsCountConfig();
        LoadDownloadRetryCountConfig();
    }

    private static void LoadGameConfig()
    {
        LoadStartJavaConfig();
        LoadGameGcTypeConfig();
        LoadGameResolutionConfig();
    }

    #region 启动器配置

    private static void LoadThemeConfig()
    {
        var theme = (BaseThemeMode)GetConfig(DefaultConfigConstants.LauncherSettingsNodes.ThemeNode);
        Application.Current!.LocateMaterialTheme<MaterialTheme>().BaseTheme = theme;
        switch (theme)
        {
            case BaseThemeMode.Inherit:
                SettingMenu.Instance.ThemeFollowSystemModeRadioButton.IsChecked = true;
                break;
            case BaseThemeMode.Light:
                SettingMenu.Instance.ThemeLightModeRadioButton.IsChecked = true;
                break;
            case BaseThemeMode.Dark:
                SettingMenu.Instance.ThemeDarkModeRadioButton.IsChecked = true;
                break;
        }
    }

    #endregion

    #region 下载配置

    private static void LoadDownloadSourceConfig()
    {
        var downloadSource = (DownloadSourceHandler.DownloadSourceEnum)GetConfig(
            DefaultConfigConstants.DownloadSettingsNodes.DownloadSourceNode);
        GlobalVariable.Config.DownloadSourceEnum = downloadSource;
        SettingMenu.Instance.DownloadSourceSelectComboBox.SelectedIndex = (int)downloadSource;
    }

    private static void LoadDownloadMaxDegreeOfParallelismCountConfig()
    {
        var parallelismCount =
            (int)GetConfig(DefaultConfigConstants.DownloadSettingsNodes.MaxDegreeOfParallelismCountNode);
        GlobalVariable.Config.DownloadMaxDegreeOfParallelismCount = parallelismCount;
        SettingMenu.Instance.DownloadMaxDegreeOfParallelismCountSlider.Value = parallelismCount;
    }

    private static void LoadDownloadPartsCountConfig()
    {
        var downloadPartsCount = (int)GetConfig(DefaultConfigConstants.DownloadSettingsNodes.PartsCountNode);
        GlobalVariable.Config.DownloadPartsCount = downloadPartsCount;
        SettingMenu.Instance.DownloadSegmentsForLargeFileSlider.Value = downloadPartsCount;
    }

    private static void LoadDownloadRetryCountConfig()
    {
        var downloadRetryCount = (int)GetConfig(DefaultConfigConstants.DownloadSettingsNodes.RetryCountNode);
        GlobalVariable.Config.DownloadRetryCount = downloadRetryCount;
        SettingMenu.Instance.DownloadTotalRetrySlider.Value = downloadRetryCount;
    }

    #endregion

    #region 游戏配置

    private static void LoadStartJavaConfig()
    {
        var startJavaPathList =
            (List<string>)GetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.JavaPathListNode);
        var startJavaIndex = (int)GetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.SelectedJavaIndexNode);
        GlobalVariable.LocalJavaList = startJavaPathList;
        GlobalVariable.Config.GameStartJavaIndex = startJavaIndex;
        if (startJavaIndex > -1)
            GlobalVariable.Config.GameStartJavaPath = startJavaPathList[startJavaIndex];
        SettingMenu.Instance.StartJavaSelectComboBox.ItemsSource = startJavaPathList;
        SettingMenu.Instance.StartJavaSelectComboBox.SelectedIndex = startJavaIndex;
    }

    private static void LoadGameGcTypeConfig()
    {
        var gameGcType = (GcType)GetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.GcTypeNode);
        GlobalVariable.Config.GameGcType = gameGcType;
        SettingMenu.Instance.GcTypeSelectComboBox.SelectedIndex = (int)gameGcType;
    }

    private static void LoadGameResolutionConfig()
    {
        var resolutionWidth = (uint)GetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.ResolutionWidthNode);
        var resolutionHeight = (uint)GetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.ResolutionHeightNode);
        GlobalVariable.Config.GameResolutionWidth = resolutionWidth;
        GlobalVariable.Config.GameResolutionHeight = resolutionHeight;
        SettingMenu.Instance!.GameResolutionWidthTextBox.Text = resolutionWidth.ToString();
        SettingMenu.Instance!.GameResolutionHeightTextBox.Text = resolutionHeight.ToString();
    }

    #endregion

    #region 登录配置

    private static async void LoadLoginConfig()
    {
        //string loginAs = Localizer.Localizer.Instance["LoginAs"];
        var username = (string)GetConfig(DefaultConfigConstants.LoginInformationNodes.UsernameNode);
        IAuthenticator authenticator = null!;
        switch (GetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode))
        {
            case LoginType.None:
                break;
            case LoginType.Offline:
                //loginAs = string.Format(loginAs, Localizer.Localizer.Instance["OfflineLogin"]);
                await AccountAuthenticatorHandler.Login(new OfflineAuthenticator
                {
                    Username = username,
                    LauncherAccountParser = new DefaultLauncherAccountParser(App.Core.RootPath!, App.Core.ClientToken)
                }, username);
                break;
            case LoginType.Microsoft:
                //loginAs = string.Format(loginAs, Localizer.Localizer.Instance["MicrosoftLogin"]);
                AuthenticatorVerify authVerify = new()
                {
                    XblRefreshToken = EncryptHandler.JcDecrypt((string)GetConfig(DefaultConfigConstants
                        .LoginInformationNodes.MicrosoftLoginRefreshTokenNode))
                };
                await AccountAuthenticatorHandler.Login(
                    new MicrosoftAuthenticator
                    {
                        CacheTokenProvider = authVerify.CacheTokenProviderAsync,
                        LauncherAccountParser = App.Core.VersionLocator.LauncherAccountParser
                    },
                    username);
                break;
            case LoginType.Yggdrasil:
                //loginAs = string.Format(loginAs, Localizer.Localizer.Instance["ThirdPartyLogin"]);
                break;
        }

        /*if ((LoginType)GetConfig(DefaultConfigConstants.LoginInformationNodes.LoginModeNode) != LoginType.None)
        {
            LoginMenu.Instance.UserNameTextBlock.Text = (string?)GetConfig(DefaultConfigConstants.LoginInformationNodes.UsernameNode);
            LoginMenu.Instance.LoginAsTextBlock.Text = loginAs;
            GlobalVariable.AccountAuthenticator = authenticator;
            LoginMenu.Instance.LoginTabControl.IsVisible = false;
            LoginMenu.Instance.LoginInGrid.IsVisible = true;
        }*/
    }

    #endregion

    #endregion

    #region 配置文件操作

    public static Config LoadConfig()
    {
        DirectoryUtils.CreateDirectory(Path.GetDirectoryName(ConfigFilePath)!);
        if (!File.Exists(ConfigFilePath)) return new Config();

        try
        {
            var json = File.ReadAllText(ConfigFilePath);
            if (string.IsNullOrWhiteSpace(json))
                SaveConfig(new Config());
            // Newtonsoft.Json
            //return JsonConvert.DeserializeObject<Config>(json)!;
            return JsonSerializer.Deserialize<Config>(json)!;
        }
        catch (Exception ex)
        {
            MyErrorWindow.CreateErrorWindow(
                ErrorType.InternalError,
                Localizer.Localizer.Instance["LoadFail"],
                Localizer.Localizer.Instance["LoadConfigFailErrorMessage1"],
                Localizer.Localizer.Instance["LoadConfigFailRepairMeasures1"],
                ex);
            return new Config();
        }
    }

    public static void SaveConfig(Config config)
    {
        if (!File.Exists(ConfigFilePath))
        {
            var fileStream = File.Create(ConfigFilePath);
            fileStream.Close();
        }

        // Newtonsoft.Json
        //var json = JsonConvert.SerializeObject(config);
        //File.WriteAllText(ConfigFilePath, JObject.Parse(json).ToString());
        var json = JsonSerializer.Serialize(config);
        File.WriteAllText(ConfigFilePath, JsonDocument.Parse(json).RootElement.GetRawText());
    }

    public static object GetConfig(string propertyName)
    {
        var config = LoadConfig();
        var propertyNames = propertyName.Split('.');
        object obj = config;
        var type = typeof(Config);
        for (var i = 0; i < propertyNames.Length; i++)
        {
            var property = type.GetProperty(propertyNames[i]);
            if (property == null)
            {
                var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"],
                    type.Name, propertyNames[i]);
                MyErrorWindow.CreateErrorWindow(
                    ErrorType.InternalError,
                    Localizer.Localizer.Instance["InternalError"],
                    $"{PropertyNotFoundInClass} - {Localizer.Localizer.Instance["PropertyNotFoundInClassErrorMessage1"]}",
                    Localizer.Localizer.Instance["PropertyNotFoundInClassRepairMeasures1"],
                    new ArgumentException($"{PropertyNotFoundInClass}"));
                return null!;
            }

            obj = GetConfigValue(obj, property);
            type = property.PropertyType;
        }

        SaveConfig(config);
        return obj;
    }

    public static void SetConfig(string propertyName, object value)
    {
        var config = LoadConfig();
        var propertyNames = propertyName.Split('.');
        object obj = config;
        var type = typeof(Config);
        for (var i = 0; i < propertyNames.Length - 1; i++)
        {
            var property = type.GetProperty(propertyNames[i]);
            if (property == null)
            {
                var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"],
                    type.Name, propertyNames[i]);
                MyErrorWindow.CreateErrorWindow(
                    ErrorType.InternalError,
                    Localizer.Localizer.Instance["InternalError"],
                    $"{PropertyNotFoundInClass} - {Localizer.Localizer.Instance["PropertyNotFoundInClassErrorMessage1"]}",
                    Localizer.Localizer.Instance["PropertyNotFoundInClassRepairMeasures1"],
                    new ArgumentException($"{PropertyNotFoundInClass}"));
                return;
            }

            obj = GetConfigValue(obj, property);
            type = property.PropertyType;
        }

        var lastProperty = type.GetProperty(propertyNames[propertyNames.Length - 1]);
        if (lastProperty == null)
        {
            var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"],
                type.Name, propertyNames[propertyNames.Length - 1]);
            MyErrorWindow.CreateErrorWindow(
                ErrorType.InternalError,
                Localizer.Localizer.Instance["InternalError"],
                $"{PropertyNotFoundInClass} - {Localizer.Localizer.Instance["PropertyNotFoundInClassErrorMessage1"]}",
                Localizer.Localizer.Instance["PropertyNotFoundInClassRepairMeasures1"],
                new ArgumentException($"{PropertyNotFoundInClass}"));
            return;
        }

        lastProperty.SetValue(obj, value);
        SaveConfig(config);
    }

    private static object GetConfigValue(object obj, PropertyInfo property)
    {
        var value = property.GetValue(obj);
        if (value == null)
        {
            value = Activator.CreateInstance(property.PropertyType);
            property.SetValue(obj, value);
        }

        return value!;
    }

    #endregion

    #region 配置文件模型

    public class Config
    {
        public LauncherSettings LauncherSettings { get; set; } = new();
        public DownloadSettings DownloadSettings { get; set; } = new();
        public GlobalGameSettings GlobalGameSettings { get; set; } = new();
        public LoginInformation LoginInformation { get; set; } = new();
    }

    public class LauncherSettings
    {
        public BaseThemeMode ThemeMode { get; set; } = BaseThemeMode.Inherit;
    }

    public class DownloadSettings
    {
        public DownloadSourceHandler.DownloadSourceEnum DownloadSource { get; set; } =
            DownloadSourceHandler.DownloadSourceEnum.BMCL; // 下载源

        public int MaxDegreeOfParallelismCount { get; set; } = 8; // 最大并行下载数
        public int PartsCount { get; set; } = 8; // 分段下载的段数
        public int RetryCount { get; set; } = 4; // 重试次数
    }

    public class GlobalGameSettings
    {
        public int SelectedJavaIndex { get; set; } = -1;
        public List<string> JavaPathList { get; set; } = new();
        public GcType GcType { get; set; } = GcType.G1Gc;
        public uint ResolutionWidth { get; set; } = 854;
        public uint ResolutionHeight { get; set; } = 480;
    }

    public class LoginInformation
    {
        public LoginType LoginMode { get; set; } = LoginType.None;
        public string Username { get; set; } = string.Empty;
        public MicrosoftLogin MicrosoftLogin { get; set; } = new();
        public YggdrasilLogin YggdrasilLogin { get; set; } = new();
    }

    public class MicrosoftLogin
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class YggdrasilLogin
    {
        public string AuthServer { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    #endregion
}