using System.Collections.Generic;
using System.IO;
using JackCraftLauncher.Class.Launch;
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

    #region 微软登录

    public class MicrosoftLogin // 微软登录
    {
        public static DateTime CurrentAuthTime { get; set; } = new(); // 当前的验证时间
        public static int ExpiresIn { get; set; } = 0; // Token 失效时间 (秒)
        public static string RefreshToken { get; set; } = string.Empty; // 刷新用 Token
    }

    #endregion
}