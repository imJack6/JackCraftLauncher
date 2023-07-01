using System.IO;
using System.Reflection;
using Avalonia;
using JackCraftLauncher.Class.Models.ErrorModels;
using JackCraftLauncher.Class.Utils;
using JackCraftLauncher.Views.MainMenus;
using JackCraftLauncher.Views.MyWindow;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Newtonsoft.Json.Linq;

namespace JackCraftLauncher.Class.ConfigHandler;

public class DefaultConfigHandler
{
    private static readonly string ConfigFilePath = GlobalVariable.MainConfigPath;

    #region 加载配置文件

    public static void LoadSettingsConfig()
    {
        LoadLauncherConfig();
    }

    private static void LoadLauncherConfig()
    {
        LoadThemeConfig();
    }

    #region 启动器配置

    private static void LoadThemeConfig()
    {
        var theme = (BaseThemeMode)GetConfig(GlobalConstants.ConfigThemeNode);
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
            return JsonConvert.DeserializeObject<Config>(json)!;
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

        var json = JsonConvert.SerializeObject(config);
        File.WriteAllText(ConfigFilePath, JObject.Parse(json).ToString());
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
                var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"], type.Name, propertyNames[i]);
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
                var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"], type.Name, propertyNames[i]);
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
            var PropertyNotFoundInClass = string.Format(Localizer.Localizer.Instance["PropertyNotFoundInClass"], type.Name, propertyNames[propertyNames.Length - 1]);
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
    }

    public class LauncherSettings
    {
        public BaseThemeMode ThemeMode { get; set; } = BaseThemeMode.Inherit;
    }

    #endregion
}