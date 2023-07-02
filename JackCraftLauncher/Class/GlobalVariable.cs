using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.Class.Model;
using ProjBobcat.Interface;

namespace JackCraftLauncher.Class;

public class GlobalVariable
{
    public static string MainConfigPath { get; } =
        $"{PlatformUtils.GetSystemUserDirectory()}{Path.DirectorySeparatorChar}JackCraft{Path.DirectorySeparatorChar}Launcher{Path.DirectorySeparatorChar}Desktop{Path.DirectorySeparatorChar}config.json";

    public static List<VersionInfo> LocalGameList { get; set; } = null!;
    public static IAuthenticator AccountAuthenticator { get; set; } = null!;

    public static DownloadSourceHandler.DownloadSourceEnum DownloadSourceEnum { get; set; } =
        DownloadSourceHandler.DownloadSourceEnum.BMCL; // 下载源

    public static int DownloadParallelismCount { get; set; } = 8; // 下载并行数
    public static int DownloadThreadCount { get; set; } = 8; // 下载线程数
    public static int DownloadRetryCount { get; set; } = 4; // 下载重试次数

    #region Minecraft Download

    public class MinecraftDownload
    {
        public static ObservableCollection<DefaultDownloadList> ReleaseVersionDownloadList = new(); // 正式版下载列表
        public static ObservableCollection<DefaultDownloadList> SnapshotVersionDownloadList = new(); // 测试版下载列表
        public static ObservableCollection<DefaultDownloadList> OldVersionDownloadList = new(); // 远古版下载列表

        public static string LatestMinecraftReleaseVersion = string.Empty; // 最新正式版
        public static string LatestMinecraftSnapshotVersion = string.Empty; // 最新快照版

        public static List<string> MinecraftIdList = new(); // Minecraft ID 版本列表
        public static List<string> MinecraftTypeList = new(); // Minecraft Type 类型列表
        public static List<string> MinecraftUrlList = new(); // Minecraft Url 地址列表
        public static List<string> MinecraftTimeList = new(); // Minecraft 时间列表
        public static List<string> MinecraftReleaseTimeList = new(); // Minecraft 发布时间列表

        public static List<string> MinecraftReleaseList = new(); // Minecraft 正式版列表
        public static List<string> MinecraftSnapshotList = new(); // Minecraft 快照列表
        public static List<string> MinecraftOldList = new(); // Minecraft 远古列表
    }

    #endregion

    #region 微软登录

    public class MicrosoftLogin // 微软登录
    {
        public static DateTime CurrentAuthTime { get; set; } = new(); // 当前的验证时间
        public static int ExpiresIn { get; set; } = 0; // Token 失效时间 (秒)
        public static string RefreshToken { get; set; } = string.Empty; // 刷新用 Token
    }

    #endregion
}