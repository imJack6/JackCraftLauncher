using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DialogHostAvalonia;
using JackCraftLauncher.Class.ConfigHandler;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Models.FabricModels;
using JackCraftLauncher.Class.Models.ForgeModels;
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Class.Models.MinecraftVersionManifest;
using JackCraftLauncher.Class.Models.OptifineModels;
using JackCraftLauncher.Class.Models.QuiltModels;
using JackCraftLauncher.Class.Utils;
using JackCraftLauncher.Views.MainMenus;
using Material.Styles.Assists;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Class.Launch;

public class ListHandler
{
    public static async Task RefreshLocalJavaList(bool fullSearch = false)
    {
        DialogHost.Show(SettingMenu.Instance.Resources["SearchingForJavaLoadingView"]!, "MainDialogHost");
        ComboBoxAssist.SetLabel(SettingMenu.Instance.StartJavaSelectComboBox,
            Localizer.Localizer.Instance["SearchingForJava"]);
        SettingMenu.Instance.StartJavaSelectComboBox.PlaceholderText = Localizer.Localizer.Instance["SearchingForJava"];
        SettingMenu.Instance.RefreshLocalJavaComboBoxButton.IsEnabled = false;
        var task = Task.Run(async () =>
        {
            var javaList = SystemInfoHelper.FindJava(fullSearch);
            GlobalVariable.LocalJavaList = await javaList.ToListAsync();
        });
        await task;
        SettingMenu.Instance.StartJavaSelectComboBox.ItemsSource = GlobalVariable.LocalJavaList;
        await Task.Delay(1);
        DialogHostUtils.Close();
        ComboBoxAssist.SetLabel(SettingMenu.Instance.StartJavaSelectComboBox,
            Localizer.Localizer.Instance["SelectJava"]);
        SettingMenu.Instance.StartJavaSelectComboBox.PlaceholderText = Localizer.Localizer.Instance["SelectJava"];
        SettingMenu.Instance.RefreshLocalJavaComboBoxButton.IsEnabled = true;
        DefaultConfigHandler.SetConfig(DefaultConfigConstants.GlobalGameSettingsNodes.JavaPathListNode,
            GlobalVariable.LocalJavaList);
    }

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

        if (!string.IsNullOrEmpty(result))
        {
            var minecraftVersionManifestModel = JsonSerializer.Deserialize<MinecraftVersionManifestModel>(result)!;
            GlobalVariable.MinecraftDownload.MinecraftVersionManifestModel = minecraftVersionManifestModel;

            GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion =
                minecraftVersionManifestModel.LatestModel!.Release;
            GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion =
                minecraftVersionManifestModel.LatestModel.Snapshot;
            foreach (var mc in minecraftVersionManifestModel.VersionsModel)
            {
                // 添加到数组
                GlobalVariable.MinecraftDownload.MinecraftIdList.Add(mc.ID!);
                GlobalVariable.MinecraftDownload.MinecraftTypeList.Add(mc.Type);
                GlobalVariable.MinecraftDownload.MinecraftUrlList.Add(mc.Url!);
                GlobalVariable.MinecraftDownload.MinecraftTimeList.Add(mc.Time);
                GlobalVariable.MinecraftDownload.MinecraftReleaseTimeList.Add(mc.ReleaseTime);

                if (mc.Type is "release")
                {
                    GlobalVariable.MinecraftDownload.MinecraftReleaseList.Add(mc.ID!);
                    var releaseTime = string.Format(Localizer.Localizer.Instance["OfficialVersionAndReleaseDate"],
                        mc.ReleaseTime);
                    GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList.Add(
                        new DefaultDownloadList(mc.ID!, releaseTime, VersionType.Official));
                }
                else if (mc.Type is "snapshot")
                {
                    GlobalVariable.MinecraftDownload.MinecraftSnapshotList.Add(mc.ID!);
                    var releaseTime = string.Format(Localizer.Localizer.Instance["BetaVersionAndReleaseDate"],
                        mc.ReleaseTime);
                    GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList.Add(
                        new DefaultDownloadList(mc.ID!, releaseTime, VersionType.Beta));
                }
                else if (mc.Type is "old_alpha" or "old_beta")
                {
                    GlobalVariable.MinecraftDownload.MinecraftOldList.Add(mc.ID!);
                    var releaseTime = string.Format(Localizer.Localizer.Instance["OldVersionAndReleaseDate"],
                        mc.ReleaseTime);
                    GlobalVariable.MinecraftDownload.OldVersionDownloadList.Add(
                        new DefaultDownloadList(mc.ID!, releaseTime, VersionType.Old));
                }
            }

            DownloadMenu.Instance.ReleaseVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList;
            DownloadMenu.Instance.SnapshotVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList;
            DownloadMenu.Instance.OldVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.OldVersionDownloadList;

            DownloadMenu.Instance.ReleaseVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.ReleaseVersionDownloadList;
            DownloadMenu.Instance.SnapshotVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.SnapshotVersionDownloadList;
            DownloadMenu.Instance.OldVersionListBox.ItemsSource =
                GlobalVariable.MinecraftDownload.OldVersionDownloadList;

            if (GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion ==
                GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion)
                DownloadMenu.Instance.LatestSnapshotVersionButton.IsVisible = false;
            DownloadMenu.Instance.LatestReleaseVersionTextBlock.Text =
                GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion;
            DownloadMenu.Instance.LatestSnapshotVersionTextBlock.Text =
                GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion;

            var latestReleaseVersionIndex =
                GlobalVariable.MinecraftDownload.MinecraftIdList.IndexOf(GlobalVariable.MinecraftDownload
                    .LatestMinecraftReleaseVersion);
            DownloadMenu.Instance.LatestReleaseTimeTextBlock.Text =
                string.Format(Localizer.Localizer.Instance["OfficialVersionAndReleaseDate"],
                    GlobalVariable.MinecraftDownload.MinecraftReleaseTimeList[latestReleaseVersionIndex]);
            var latestSnapshotVersionIndex =
                GlobalVariable.MinecraftDownload.MinecraftIdList.IndexOf(GlobalVariable.MinecraftDownload
                    .LatestMinecraftSnapshotVersion);
            DownloadMenu.Instance.LatestSnapshotTimeTextBlock.Text =
                string.Format(Localizer.Localizer.Instance["BetaVersionAndReleaseDate"],
                    GlobalVariable.MinecraftDownload.MinecraftReleaseTimeList[latestSnapshotVersionIndex]);
        }

        #endregion

        #region 解析版本列表 旧的

        /*
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
                                                                                 */

        #endregion
    }

    public static async Task RefreshLocalForgeDownloadList(string mcVersion)
    {
        #region 初始化

        DownloadMenu.Instance.ForgeExpander.IsEnabled = false;
        DownloadMenu.Instance.ForgeSelectVersionTextBlock.Text = Localizer.Localizer.Instance["Loading"];
        GlobalVariable.ForgeDownload.ForgeListModel = null!;
        GlobalVariable.ForgeDownload.ForgeDownloadList.Clear();

        #endregion

        #region 获取列表

        #region 获取支持版本列表

        var forgeSupportResult = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.ForgeSupportMcList, null).GetStringAsync();
        var forgeSupportList = JsonSerializer.Deserialize<string[]>(forgeSupportResult);

        #endregion

        #region 判断是否支持

        if (!forgeSupportList!.Contains(mcVersion))
        {
            DownloadMenu.Instance.ForgeSelectVersionTextBlock.Text = Localizer.Localizer.Instance["UnsupportedVersion"];
            DownloadMenu.Instance.ForgeExpander.IsEnabled = false;
            return;
        }

        #endregion

        #region 获取对应版本Forge列表

        var forgeResult = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.ForgeMcList, null, mcVersion).GetStringAsync();
        var forgeList = JsonSerializer.Deserialize<ForgeListModel[]>(forgeResult);
        forgeList = forgeList!.OrderByDescending(model => model.Modified).ToArray(); // 按时间排序
        GlobalVariable.ForgeDownload.ForgeListModel = forgeList;

        #endregion

        #endregion

        #region 更新到UI

        GlobalVariable.ForgeDownload.ForgeDownloadList = new ObservableCollection<ForgeDownloadList>();
        foreach (var forge in forgeList)
            GlobalVariable.ForgeDownload.ForgeDownloadList.Add(new ForgeDownloadList(forge.Version!,
                string.Format(Localizer.Localizer.Instance["ReleaseDate"], forge.Modified)));
        DownloadMenu.Instance.ForgeListBox.ItemsSource = GlobalVariable.ForgeDownload.ForgeDownloadList;

        #endregion

        #region 结束

        DownloadMenu.Instance.ForgeExpander.IsEnabled = true;
        DownloadMenu.Instance.ForgeSelectVersionTextBlock.Text = Localizer.Localizer.Instance["NotSelected"];

        #endregion
    }

    public static async Task RefreshLocalFabricDownloadList(string mcVersion)
    {
        #region 初始化

        DownloadMenu.Instance.FabricExpander.IsEnabled = false;
        DownloadMenu.Instance.FabricSelectVersionTextBlock.Text = Localizer.Localizer.Instance["Loading"];
        GlobalVariable.FabricDownload.FabricListModel = null!;
        GlobalVariable.FabricDownload.FabricDownloadList.Clear();

        #endregion

        #region 获取列表

        #region 获取对应版本列表

        var fabricUrl = $"{DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.FabricMeta, null)}v2/versions/loader/{mcVersion}";
        var fabricResult = await fabricUrl.AllowAnyHttpStatus().GetStringAsync();

        #endregion

        #region 判断是否支持

        if (!IsVersionSupported(fabricResult))
        {
            DownloadMenu.Instance.FabricSelectVersionTextBlock.Text =
                Localizer.Localizer.Instance["UnsupportedVersion"];
            DownloadMenu.Instance.FabricExpander.IsEnabled = false;
            return;
        }

        #endregion

        #region 添加到数组

        var fabricList = JsonSerializer.Deserialize<FabricListModel[]>(fabricResult)!;
        GlobalVariable.FabricDownload.FabricListModel = fabricList;

        #endregion

        #endregion

        #region 更新到UI

        GlobalVariable.FabricDownload.FabricDownloadList = new ObservableCollection<FabricDownloadList>();
        foreach (var fabric in fabricList)
            GlobalVariable.FabricDownload.FabricDownloadList.Add(new FabricDownloadList(fabric.Loader.Version,
                fabric.Loader.Stable
                    ? Localizer.Localizer.Instance["OfficialVersion"]
                    : Localizer.Localizer.Instance["BetaVersion"]));
        DownloadMenu.Instance.FabricListBox.ItemsSource = GlobalVariable.FabricDownload.FabricDownloadList;

        #endregion

        #region 结束

        DownloadMenu.Instance.FabricExpander.IsEnabled = true;
        DownloadMenu.Instance.FabricSelectVersionTextBlock.Text = Localizer.Localizer.Instance["NotSelected"];

        #endregion
    }

    public static async Task RefreshLocalOptifineDownloadList(string mcVersion)
    {
        #region 初始化

        DownloadMenu.Instance.OptifineExpander.IsEnabled = false;
        DownloadMenu.Instance.OptifineSelectVersionTextBlock.Text = Localizer.Localizer.Instance["Loading"];
        GlobalVariable.OptifineDownload.OptifineListModel = null!;
        GlobalVariable.OptifineDownload.OptifineDownloadList.Clear();

        #endregion

        #region 获取列表

        #region 获取对应版本列表

        var optifineUrl = $"{DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.OptifineMcList, null, mcVersion)}";
        var optifineResult = await optifineUrl.AllowAnyHttpStatus().GetStringAsync();

        #endregion

        #region 判断是否支持

        if (!IsVersionSupported(optifineResult))
        {
            DownloadMenu.Instance.OptifineSelectVersionTextBlock.Text =
                Localizer.Localizer.Instance["UnsupportedVersion"];
            DownloadMenu.Instance.OptifineExpander.IsEnabled = false;
            return;
        }

        #endregion

        #region 添加到数组

        var optifineList = JsonSerializer.Deserialize<OptifineListModel[]>(optifineResult)!;
        GlobalVariable.OptifineDownload.OptifineListModel = optifineList;

        #endregion

        #endregion

        #region 更新到UI

        GlobalVariable.OptifineDownload.OptifineDownloadList = new ObservableCollection<OptifineDownloadList>();
        foreach (var optifine in optifineList)
        {
            var versionType = !optifine.Patch.Contains("pre")
                ? Localizer.Localizer.Instance["OfficialVersion"]
                : Localizer.Localizer.Instance["BetaVersion"];
            GlobalVariable.OptifineDownload.OptifineDownloadList.Add(new OptifineDownloadList(
                $"{optifine.Type} {optifine.Patch}",
                $"{versionType} - {string.Format(Localizer.Localizer.Instance["RecommendedForgeVersion"], optifine.Forge)}"));
        }

        DownloadMenu.Instance.OptifineListBox.ItemsSource = GlobalVariable.OptifineDownload.OptifineDownloadList;

        #endregion

        #region 结束

        DownloadMenu.Instance.OptifineExpander.IsEnabled = true;
        DownloadMenu.Instance.OptifineSelectVersionTextBlock.Text = Localizer.Localizer.Instance["NotSelected"];

        #endregion
    }

    public static async Task RefreshLocalQuiltDownloadList(string mcVersion)
    {
        #region 初始化

        DownloadMenu.Instance.QuiltExpander.IsEnabled = false;
        DownloadMenu.Instance.QuiltSelectVersionTextBlock.Text = Localizer.Localizer.Instance["Loading"];
        GlobalVariable.QuiltDownload.QuiltListModel = null!;
        GlobalVariable.QuiltDownload.QuiltDownloadList.Clear();

        #endregion

        #region 获取列表

        #region 获取支持版本列表

        var quiltSupportResult = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.QuiltSupportMcList, null).GetStringAsync();
        var quiltSupportList = JsonSerializer.Deserialize<QuiltSupportMcModel[]>(quiltSupportResult);

        #endregion

        #region 判断是否支持

        if (!quiltSupportList.Any(model => model.Version == mcVersion))
        {
            DownloadMenu.Instance.QuiltSelectVersionTextBlock.Text = Localizer.Localizer.Instance["UnsupportedVersion"];
            DownloadMenu.Instance.QuiltExpander.IsEnabled = false;
            return;
        }

        #endregion

        #region 获取Quilt加载器列表

        var quiltResult = await DownloadSourceHandler
            .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.QuiltLoaderList, null, mcVersion)
            .GetStringAsync();
        var quiltList = JsonSerializer.Deserialize<QuiltListModel[]>(quiltResult)!;
        GlobalVariable.QuiltDownload.QuiltListModel = quiltList;

        #endregion

        #endregion

        #region 更新到UI

        GlobalVariable.QuiltDownload.QuiltDownloadList = new ObservableCollection<QuiltDownloadList>();
        foreach (var quilt in quiltList)
            GlobalVariable.QuiltDownload.QuiltDownloadList.Add(new QuiltDownloadList(quilt.Version,
                !quilt.Version.Contains("beta")
                    ? Localizer.Localizer.Instance["OfficialVersion"]
                    : Localizer.Localizer.Instance["BetaVersion"]));
        DownloadMenu.Instance.QuiltListBox.ItemsSource = GlobalVariable.QuiltDownload.QuiltDownloadList;

        #endregion

        #region 结束

        DownloadMenu.Instance.QuiltExpander.IsEnabled = true;
        DownloadMenu.Instance.QuiltSelectVersionTextBlock.Text = Localizer.Localizer.Instance["NotSelected"];

        #endregion
    }

    private static bool IsVersionSupported(string result)
    {
        try
        {
            var document = JsonDocument.Parse(result);
            if (!(document.RootElement.ValueKind == JsonValueKind.Array && document.RootElement.GetArrayLength() > 0))
                return false;
        }
        catch (JsonException)
        {
            return false;
        }

        return true;
    }
}