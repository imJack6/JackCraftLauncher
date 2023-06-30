using System.Collections.Generic;
using JackCraftLauncher.Class.Launch;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Class;

public class GlobalVariable
{
    public static List<VersionInfo> LocalGameList { get; set; } = null!;
    public static DownloadSourceHandler.DownloadSourceEnum DownloadSourceEnum { get; set; } =
        DownloadSourceHandler.DownloadSourceEnum.BMCL; // 下载源
    public static int DownloadParallelismCount { get; set; } = 8; // 下载并行数
    public static int DownloadThreadCount { get; set; } = 8; // 下载线程数
    public static int DownloadRetryCount { get; set; } = 4; // 下载重试次数
}