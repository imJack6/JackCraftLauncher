using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models.FabricModels;
using JackCraftLauncher.Class.Models.ForgeModels;
using JackCraftLauncher.Class.Models.InstallModels;
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Class.Models.LiteLoaderModels;
using JackCraftLauncher.Class.Models.MinecraftVersionManifest;
using JackCraftLauncher.Class.Models.OptifineModels;
using JackCraftLauncher.Class.Models.QuiltModels;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.Class.Model;
using ProjBobcat.Interface;

namespace JackCraftLauncher.Class;

public class GlobalVariable
{
    public static List<VersionInfo> LocalGameList { get; set; } = null!;
    public static List<string> LocalJavaList { get; set; } = new();
    public static IAuthenticator AccountAuthenticator { get; set; } = null!;

    #region 配置

    public class Config
    {
        public static string MainConfigPath { get; } =
            $"{PlatformUtils.GetSystemUserDirectory()}{Path.DirectorySeparatorChar}JackCraft{Path.DirectorySeparatorChar}Launcher{Path.DirectorySeparatorChar}Desktop{Path.DirectorySeparatorChar}config.json";

        #region 下载

        public static DownloadSourceHandler.DownloadSourceEnum DownloadSourceEnum { get; set; } =
            DownloadSourceHandler.DownloadSourceEnum.BMCL; // 下载源

        public static int DownloadMaxDegreeOfParallelismCount { get; set; } = 8; // 下载并行数
        public static int DownloadPartsCount { get; set; } = 8; // 下载分段数
        public static int DownloadRetryCount { get; set; } = 4; // 下载重试次数

        #endregion

        #region 游戏

        public static int GameStartJavaIndex { get; set; } = -1; // 游戏启动 Java 索引
        public static string GameStartJavaPath { get; set; } = ""; // 游戏启动 Java 路径
        public static GcType GameGcType { get; set; } = GcType.G1Gc; // 游戏 GC 类型
        public static uint GameResolutionWidth { get; set; } = 854; // 游戏分辨率宽度
        public static uint GameResolutionHeight { get; set; } = 480; // 游戏分辨率高度

        #endregion
    }

    #endregion

    #region 登录

    public class MicrosoftLogin // 微软登录
    {
        public static string RefreshToken { get; set; } = string.Empty; // 刷新用 Token
    }

    public class YggdrasilLogin // Yggdrasil 登录
    {
        public static string AuthServer { get; set; } = string.Empty; // 验证服务器
        public static string UserNameOrEmail { get; set; } = string.Empty; // 用户名或邮箱
        public static string Password { get; set; } = string.Empty; // 密码
    }

    #endregion

    #region Download

    public static ObservableCollection<DownloadSelectModel> DownloadSelectAttachmentsModels { get; set; } = new();

    public class MinecraftDownload
    {
        public static MinecraftVersionManifestModel MinecraftVersionManifestModel { get; set; } =
            null!; // Minecraft 版本清单

        public static ObservableCollection<DefaultDownloadList> ReleaseVersionDownloadList { get; set; } =
            new(); // 正式版下载列表

        public static ObservableCollection<DefaultDownloadList> SnapshotVersionDownloadList { get; set; } =
            new(); // 测试版下载列表

        public static ObservableCollection<DefaultDownloadList> OldVersionDownloadList { get; set; } = new(); // 远古版下载列表

        public static string LatestMinecraftReleaseVersion { get; set; } = string.Empty; // 最新正式版
        public static string LatestMinecraftSnapshotVersion { get; set; } = string.Empty; // 最新快照版

        public static List<string> MinecraftIdList { get; set; } = new(); // Minecraft ID 版本列表
        public static List<string> MinecraftTypeList { get; set; } = new(); // Minecraft Type 类型列表
        public static List<string> MinecraftUrlList { get; set; } = new(); // Minecraft Url 地址列表
        public static List<DateTime> MinecraftTimeList { get; set; } = new(); // Minecraft 时间列表
        public static List<DateTime> MinecraftReleaseTimeList { get; set; } = new(); // Minecraft 发布时间列表

        public static List<string> MinecraftReleaseList { get; set; } = new(); // Minecraft 正式版列表
        public static List<string> MinecraftSnapshotList { get; set; } = new(); // Minecraft 快照列表
        public static List<string> MinecraftOldList { get; set; } = new(); // Minecraft 远古列表
    }

    public class ForgeDownload
    {
        public static ObservableCollection<ForgeDownloadList> ForgeDownloadList { get; set; } = new(); // Forge下载列表
        public static ForgeListModel[] ForgeListModel { get; set; } = null!; // Forge 版本清单
    }

    public class FabricDownload
    {
        public static ObservableCollection<FabricDownloadList> FabricDownloadList { get; set; } = new(); // Fabric下载列表
        public static FabricListModel[] FabricListModel { get; set; } = null!; // Fabric 版本清单
    }

    public class OptifineDownload
    {
        public static ObservableCollection<OptifineDownloadList> OptifineDownloadList { get; set; } =
            new(); // Optifine下载列表

        public static OptifineListModel[] OptifineListModel { get; set; } = null!; // Optifine 版本清单
    }

    public class QuiltDownload
    {
        public static ObservableCollection<QuiltDownloadList> QuiltDownloadList { get; set; } = new(); // Quilt下载列表
        public static QuiltListModel[] QuiltListModel { get; set; } = null!; // Quilt 版本清单
    }

    public class LiteLoaderDownload
    {
        public static ObservableCollection<LiteLoaderDownloadList> LiteLoaderDownloadList { get; set; } =
            new(); // LiteLoader下载列表

        public static LiteLoaderMcVersionModel LiteLoaderMcVersionModel { get; set; } = null!; // LiteLoader 版本
    }

    #endregion
}