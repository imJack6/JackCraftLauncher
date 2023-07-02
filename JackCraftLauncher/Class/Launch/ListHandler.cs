using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Views.MainMenus;
using Newtonsoft.Json.Linq;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Class.Launch;

public class ListHandler
{
    public static void RefreshLocalGameList()
    {
        try
        {
            var gameList = GetLocalGameList();
            if (StartMenu.Instance != null)
            {
                if (gameList == null)
                {
                    GlobalVariable.LocalGameList = new List<VersionInfo>();
                    StartMenu.Instance.LocalGameListBox.ItemsSource =
                        GlobalVariable.LocalGameList.Select(x => x.Name ?? x.Id).ToList();
                }
                else
                {
                    if (GlobalVariable.LocalGameList != gameList)
                    {
                        GlobalVariable.LocalGameList = gameList;
                        StartMenu.Instance.LocalGameListBox.ItemsSource =
                            GlobalVariable.LocalGameList.Select(x => x.Name ?? x.Id).ToList();
                    }
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private static List<VersionInfo> GetLocalGameList()
    {
        List<VersionInfo> gameList = null!;
        try
        {
            gameList = App.Core.VersionLocator.GetAllGames().ToList();
        }
        catch (Exception)
        {
            // ignored
        }

        return gameList;
    }

    public static async Task RefreshLocalMinecraftDownloadList()
    {
        #region 清空旧数组

        GlobalVariable.MinecraftDownload.MinecraftReleaseList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftSnapshotList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftOldList.Clear();

        GlobalVariable.MinecraftDownload.MinecraftIdList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftTypeList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftUrlList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftTimeList.Clear();
        GlobalVariable.MinecraftDownload.MinecraftReleaseTimeList.Clear();

        GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList.Clear();
        GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList.Clear();
        GlobalVariable.MinecraftDownload.OldVersionDownloadList.Clear();

        #endregion

        #region 获取版本列表

        var result = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.VersionInfoV1, null).GetStringAsync();

        #endregion

        #region 解析版本列表

        var jObject = JObject.Parse(result);
        GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion = jObject["latest"]!["release"]!.ToString();
        GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion = jObject["latest"]!["snapshot"]!.ToString();
        var allMinecraftList = JArray.Parse(jObject["versions"]!.ToString());
        foreach (var mc in allMinecraftList)
        {
            var MinecraftList = JObject.Parse(mc.ToString());
            var MinecraftId = MinecraftList["id"].ToString();
            var MinecraftType = MinecraftList["type"].ToString();
            var MinecraftUrl = MinecraftList["url"].ToString();
            var MinecraftTime = MinecraftList["time"].ToString();
            var MinecraftReleaseTime = MinecraftList["releaseTime"].ToString();
            // 4.添加到数组
            GlobalVariable.MinecraftDownload.MinecraftIdList.Add(MinecraftId);
            GlobalVariable.MinecraftDownload.MinecraftTypeList.Add(MinecraftType);
            GlobalVariable.MinecraftDownload.MinecraftUrlList.Add(MinecraftUrl);
            GlobalVariable.MinecraftDownload.MinecraftTimeList.Add(MinecraftTime);
            GlobalVariable.MinecraftDownload.MinecraftReleaseTimeList.Add(MinecraftReleaseTime);

            if (MinecraftType == "release")
            {
                GlobalVariable.MinecraftDownload.MinecraftReleaseList.Add(MinecraftId);
                var ReleaseTime = string.Format(Localizer.Localizer.Instance["OfficialVersionAndReleaseDate"],
                    MinecraftReleaseTime);
                GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList.Add(
                    new DefaultDownloadList(MinecraftId, ReleaseTime, VersionType.Official));
            }
            else if (MinecraftType == "snapshot")
            {
                GlobalVariable.MinecraftDownload.MinecraftSnapshotList.Add(MinecraftId);
                var ReleaseTime = string.Format(Localizer.Localizer.Instance["BetaVersionAndReleaseDate"],
                    MinecraftReleaseTime);
                GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList.Add(
                    new DefaultDownloadList(MinecraftId, ReleaseTime, VersionType.Beta));
            }
            else if (MinecraftType is "old_alpha" or "old_beta")
            {
                GlobalVariable.MinecraftDownload.MinecraftOldList.Add(MinecraftId);
                var ReleaseTime = string.Format(Localizer.Localizer.Instance["OldVersionAndReleaseDate"],
                    MinecraftReleaseTime);
                GlobalVariable.MinecraftDownload.OldVersionDownloadList.Add(
                    new DefaultDownloadList(MinecraftId, ReleaseTime, VersionType.Old));
            }
        }

        DownloadMenu.Instance.ReleaseVersionListBox.ItemsSource =
            GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList;
        DownloadMenu.Instance.SnapshotVersionListBox.ItemsSource =
            GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList;
        DownloadMenu.Instance.OldVersionListBox.ItemsSource = GlobalVariable.MinecraftDownload.OldVersionDownloadList;

        if (GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion ==
            GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion)
            DownloadMenu.Instance.LatestSnapshotVersionButton.IsVisible = false;
        DownloadMenu.Instance.LatestReleaseVersionTextBlock.Text =
            GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion;
        DownloadMenu.Instance.LatestSnapshotVersionTextBlock.Text =
            GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion;

        var LatestReleaseVersionIndex =
            GlobalVariable.MinecraftDownload.MinecraftIdList.IndexOf(GlobalVariable.MinecraftDownload
                .LatestMinecraftReleaseVersion);
        if (LatestReleaseVersionIndex != -1)
            DownloadMenu.Instance.LatestReleaseVersionTextBlock.Text += "\n正式版  发布日期: " +
                                                                        GlobalVariable.MinecraftDownload
                                                                            .MinecraftReleaseTimeList[
                                                                                LatestReleaseVersionIndex];

        var LatestSnapshotVersionIndex =
            GlobalVariable.MinecraftDownload.MinecraftIdList.IndexOf(GlobalVariable.MinecraftDownload
                .LatestMinecraftSnapshotVersion);
        if (LatestSnapshotVersionIndex != -1)
            DownloadMenu.Instance.LatestSnapshotVersionTextBlock.Text += "\n测试版  发布日期: " +
                                                                         GlobalVariable.MinecraftDownload
                                                                             .MinecraftReleaseTimeList[
                                                                                 LatestSnapshotVersionIndex];

        #endregion
    }
}