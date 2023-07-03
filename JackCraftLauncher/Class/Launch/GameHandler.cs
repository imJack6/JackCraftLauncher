using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Utils;
using JackCraftLauncher.Views;
using JackCraftLauncher.Views.MainMenus;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjBobcat.Class.Model.Mojang;
using ProjBobcat.DefaultComponent;
using ProjBobcat.DefaultComponent.Launch.GameCore;
using ProjBobcat.DefaultComponent.Logging;
using ProjBobcat.DefaultComponent.ResourceInfoResolver;
using ProjBobcat.Interface;
using ProjBobcat.Platforms.Windows;
using SystemInfoHelper = ProjBobcat.Platforms.Windows.SystemInfoHelper;

namespace JackCraftLauncher.Class.Launch;

public class GameHandler
{
    public static async Task StartGame(VersionInfo versionInfo)
    {
        var core = new DefaultGameCore
        {
            ClientToken = App.Core.ClientToken,
            RootPath = App.Core.RootPath,
            VersionLocator = App.Core.VersionLocator,
            GameLogResolver = new DefaultGameLogResolver()
        };
        var resourceCompletion = await GetResourceCompletion(versionInfo);

        var successI18N = Localizer.Localizer.Instance["Success"];
        var failI18N = Localizer.Localizer.Instance["Fail"];
        var launcherI18N = Localizer.Localizer.Instance["Launcher"];
        var completedI18N = Localizer.Localizer.Instance["Completed"];
        var resourceCompletionI18N = Localizer.Localizer.Instance["ResourceCompletion"];
        var retryI18N = Localizer.Localizer.Instance["Retry"];
        var errorI18N = Localizer.Localizer.Instance["Error"];


        var launchSettings = new LaunchSettings
        {
            FallBackGameArguments =
                new GameArguments // 游戏启动参数缺省值，适用于以该启动设置启动的所有游戏，对于具体的某个游戏，可以设置（见下）具体的启动参数，如果所设置的具体参数出现缺失，将使用这个补全
                {
                    GcType = GcType.G1Gc, // GC类型
                    JavaExecutable = @"C:\Program Files\Zulu\zulu-17\bin\java.exe", // Java路径
                    Resolution = new ResolutionModel // 游戏窗口分辨率
                    {
                        Height = 350, // 高度
                        Width = 800 // 宽度
                    },
                    MinMemory = 2048, // 最小内存
                    MaxMemory = 4196 // 最大内存
                },
            Version = versionInfo.Id!, // 需要启动的游戏ID，例如1.7.10或者1.15.2
            //VersionInsulation = false, // 版本隔离
            //GamePath = core.RootPath!, // 游戏根目录
            VersionInsulation = true, // 版本隔离
            GamePath = GamePathHelper.GetGamePath(versionInfo.Id!), // 游戏根目录
            GameResourcePath = core.RootPath!, // 资源根目录
            GameName = versionInfo.Name, // 游戏名称
            VersionLocator = core.VersionLocator, // 游戏定位器 
            LauncherName = "JackCraft Launcher",
            Authenticator = GlobalVariable.AccountAuthenticator // 账户验证器
        };
        core.LaunchLogEventDelegate += (sender, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog($"[{launcherI18N}] {args.Item}");
            });
        };
        /*core.GameLogEventDelegate += (sender, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (logWindow.IsVisible)
                    logWindow.AddLog($"[游戏] {args.RawContent}");
            });
        };
        core.GameExitEventDelegate += (sender, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (logWindow.IsVisible)
                    logWindow.AddLog("[启动器] 游戏已退出");
            });
        };*/
        resourceCompletion.DownloadFileCompletedEvent += (sender, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (sender is not DownloadFile file) return;
                var isSuccess = args.Success == null
                    ? string.Empty
                    : $"[{(args.Success.Value ? successI18N : failI18N)}]";
                var retry = file.RetryCount == 0
                    ? string.Empty
                    : $"<{retryI18N} - {file.RetryCount}>";
                var fileName = file.FileType switch
                {
                    ResourceType.Asset => file.FileName.AsSpan()[..10],
                    ResourceType.LibraryOrNative => file.FileName,
                    _ => file.FileName
                };
                var pD =
                    //$"[{file.FileType} 已完成] - {retry}{isSuccess} {fileName.ToString()} ({resourceCompletion.TotalDownloaded} / {resourceCompletion.NeedToDownload}) [{args.AverageSpeed:F} Kb / s]";
                    $"[{file.FileType} {completedI18N}] - {retry}{isSuccess} - {fileName.ToString()} - ({resourceCompletion.TotalDownloaded} / {resourceCompletion.NeedToDownload})";
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog($"[{resourceCompletionI18N}] {pD}");
            });
        };
        await resourceCompletion.CheckAndDownloadTaskAsync().ConfigureAwait(false);
        var result = await core.LaunchTaskAsync(launchSettings).ConfigureAwait(true); // 返回游戏启动结果，以及异常信息（如果存在）
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (result.Error != null)
            {
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.Error}");
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.ErrorMessage}");
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.Cause}");
            }
            /*if (logWindow.IsVisible)
                {
                    logWindow.LaunchResult = result;
                    logWindow.Title = $"日志显示 (PID:{result.GameProcess!.Id})";
                    logWindow.TitleTextBlock.Text = $"日志显示 (PID:{result.GameProcess!.Id})";
                }*/
        });
    }

    public static async Task StartBedrockGame()
    {
        var errorI18N = Localizer.Localizer.Instance["Error"];
        var launcherI18N = Localizer.Localizer.Instance["Launcher"];
        var uwpCore = new DefaultMineCraftUWPCore();
        uwpCore.LaunchLogEventDelegate += (sender, eventArgs) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog($"[{launcherI18N}] {eventArgs.Item}");
            });
        };
        var result = uwpCore.Launch();

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (result.Error != null)
            {
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.Error}");
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.ErrorMessage}");
                MainWindow.Instance?.StartGameMenuItem.AddStartGameLog(
                    $"[{launcherI18N}] [{errorI18N}] {result.Error.Cause}");
            }
            /*if (logWindow.IsVisible)
                {
                    logWindow.LaunchResult = result;
                    logWindow.Title = $"日志显示 (PID:{result.GameProcess!.Id})";
                    logWindow.TitleTextBlock.Text = $"日志显示 (PID:{result.GameProcess!.Id})";
                }*/
        });
    }

    public static async Task<DefaultResourceCompleter> GetResourceCompletion(VersionInfo versionInfo)
    {
        var versionManifest = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.VersionInfoV1, null).GetStringAsync();
        // Newtonsoft.Json
        //var manifest = JsonConvert.DeserializeObject<VersionManifest>(versionManifest);
        var manifest = JsonSerializer.Deserialize<VersionManifest>(versionManifest);
        var versions = manifest!.Versions;
        var basePath = App.Core.RootPath!;
        // Assets 解析器
        var resolverAsset = new AssetInfoResolver
        {
            AssetIndexUriRoot = DownloadSourceHandler
                .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.MinecraftAssetsIndex, null),
            AssetUriRoot = DownloadSourceHandler
                .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.MinecraftAssets, null),
            BasePath = basePath,
            VersionInfo = versionInfo,
            CheckLocalFiles = true,
            Versions = versions // 在上一步获取到的 Versions 数组
        };
        // log4j 日志格式化组件解析器
        var resolverLogging = new GameLoggingInfoResolver
        {
            BasePath = basePath,
            VersionInfo = versionInfo,
            CheckLocalFiles = true
        };
        // Libraries 解析器
        var resolverLibrary = new LibraryInfoResolver
        {
            BasePath = basePath,
            ForgeUriRoot = "https://files.minecraftforge.net/maven/",
            ForgeMavenUriRoot = "https://maven.minecraftforge.net/",
            ForgeMavenOldUriRoot = "https://files.minecraftforge.net/maven/",
            FabricMavenUriRoot = "https://maven.fabricmc.net/",
            LibraryUriRoot = DownloadSourceHandler
                .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.MinecraftLibraries, null),
            VersionInfo = versionInfo,
            CheckLocalFiles = true
        };
        // 版本信息解析器
        var resolverVersion = new VersionInfoResolver
        {
            BasePath = basePath,
            VersionInfo = versionInfo,
            CheckLocalFiles = true
        };
        // 资源补全器
        var completer = new DefaultResourceCompleter
        {
            MaxDegreeOfParallelism = GlobalVariable.DownloadParallelismCount,
            ResourceInfoResolvers = new List<IResourceInfoResolver>
            {
                resolverAsset,
                resolverLogging,
                resolverLibrary,
                resolverVersion
            },
            TotalRetry = GlobalVariable.DownloadThreadCount,
            CheckFile = true,
            DownloadParts = GlobalVariable.DownloadRetryCount
        };

        return completer;
    }

    public static async Task CheckMCBedrockInstalled()
    {
        DialogHost.Show(StartMenu.Instance!.Resources["LoadingView"]!, "MainDialogHost");
        await Task.Delay(1000);
        await Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (SystemInfoHelper.IsMinecraftUWPInstalled())
                {
                    StartMenu.Instance.NotFoundMinecraftBedrockEditionTextBlock.IsVisible = false;
                    StartMenu.Instance.FoundMinecraftBedrockEditionCanStartTextBlock.IsVisible = true;
                    StartMenu.Instance.GoToMicrosoftStoreButton.IsVisible = false;
                }
                else
                {
                    StartMenu.Instance.NotFoundMinecraftBedrockEditionTextBlock.IsVisible = true;
                    StartMenu.Instance.FoundMinecraftBedrockEditionCanStartTextBlock.IsVisible = false;
                    StartMenu.Instance.GoToMicrosoftStoreButton.IsVisible = true;
                }
            });
        });
        DialogHostUtils.Close();
    }
}