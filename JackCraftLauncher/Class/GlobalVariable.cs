using JackCraftLauncher.Class.Launch;

namespace JackCraftLauncher.Class;

public class GlobalVariable
{
    public static DownloadSourceHandler.DownloadSourceEnum DownloadSourceEnum { get; set; } =
        DownloadSourceHandler.DownloadSourceEnum.BMCL; // 下载源

    public static int DownloadParallelismCount { get; set; } = 8; // 下载并行数
    public static int DownloadThreadCount { get; set; } = 8; // 下载线程数
    public static int DownloadRetryCount { get; set; } = 3; // 下载重试次数
}