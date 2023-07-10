using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DialogHostAvalonia;
using JackCraftLauncher.Class;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Models.InstallModels;
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjBobcat.DefaultComponent.Installer.ForgeInstaller;
using ProjBobcat.Interface;

namespace JackCraftLauncher.Views.MainMenus;

public partial class DownloadMenu : UserControl
{
    private static CancellationTokenSource _downloadListLoadingCancellationTokenSource = new();

    private static CancellationToken _downloadListLoadingCancellationToken =
        _downloadListLoadingCancellationTokenSource.Token;

    public DownloadMenu()
    {
        InitializeComponent();
        Instance = this;
    }

    public static DownloadMenu Instance { get; private set; } = null!;

    #region 选择要安装的附加内容界面

    private void BackToSelectVersionButton_Click(object sender, RoutedEventArgs e)
    {
        InstallMinecraftVersionTextBlock.Text = string.Empty;
        DownloadSaveVersionNameTextBox.Text = string.Empty;
        SelectDownloadMinecraftVersionStackPanel.IsVisible = true;
        SelectDownloadAttachmentsStackPanel.IsVisible = false;

        #region Forge

        ForgeExpander.IsExpanded = false;
        ForgeCancelSelectButton.IsVisible = false;
        ForgeListBox.SelectedIndex = -1;

        #endregion
    }

    private void StartInstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        StartInstall();
    }

    #region Forge

    private void ForgeListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ForgeListBox.SelectedIndex != -1)
        {
            var downloadList = (ForgeDownloadList)ForgeListBox.SelectedItem!;
            RemoveDownloadSelectModel(DownloadAttachmentsType.Forge);
            AddDownloadSelectModel(
                new DownloadSelectModel
                {
                    InstallAttachmentsType = DownloadAttachmentsType.Forge,
                    Version = downloadList.Version
                });
            ForgeExpander.IsExpanded = false;
            ForgeCancelSelectButton.IsVisible = true;
            ForgeSelectVersionTextBlock.Text = downloadList.Version;
        }
    }

    private void ForgeCancelSelectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RemoveDownloadSelectModel(DownloadAttachmentsType.Forge);
        ForgeExpander.IsExpanded = false;
        ForgeCancelSelectButton.IsVisible = false;
        ForgeListBox.SelectedIndex = -1;
        ForgeSelectVersionTextBlock.Text = Localizer.Localizer.Instance["NotSelected"];
    }

    #endregion

    private static void AddDownloadSelectModel(DownloadSelectModel model)
    {
        if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                m.InstallAttachmentsType == model.InstallAttachmentsType))
            GlobalVariable.DownloadSelectAttachmentsModels.Remove(
                GlobalVariable.DownloadSelectAttachmentsModels.First(m =>
                    m.InstallAttachmentsType == model.InstallAttachmentsType));
        GlobalVariable.DownloadSelectAttachmentsModels.Add(model);
        Instance.InstallAttachmentsTextBlock.Text = string.Join(", ",
            GlobalVariable.DownloadSelectAttachmentsModels.Select(m => $"{m.InstallAttachmentsType} {m.Version}"));
        Instance.InstallAttachmentsTextBlock.IsVisible = true;
    }

    private static void RemoveDownloadSelectModel(DownloadAttachmentsType type)
    {
        if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m => m.InstallAttachmentsType == type))
            foreach (var model in GlobalVariable.DownloadSelectAttachmentsModels
                         .Where(m => m.InstallAttachmentsType == type).ToList())
                GlobalVariable.DownloadSelectAttachmentsModels.Remove(model);
        Instance.InstallAttachmentsTextBlock.Text = string.Join(", ",
            GlobalVariable.DownloadSelectAttachmentsModels.Select(m => $"{m.InstallAttachmentsType} {m.Version}"));
        if (GlobalVariable.DownloadSelectAttachmentsModels.Count == 0)
            Instance.InstallAttachmentsTextBlock.IsVisible = false;
    }

    #endregion

    #region 选择要安装的Minecraft界面

    public static async void RefreshLocalMinecraftDownloadList()
    {
        DialogHost.Show(Instance.Resources["LoadingAndCancelView"]!, "MainDialogHost");
        _downloadListLoadingCancellationTokenSource.Cancel();
        _downloadListLoadingCancellationTokenSource = new CancellationTokenSource();
        _downloadListLoadingCancellationToken = _downloadListLoadingCancellationTokenSource.Token;
        await Task.Delay(1000);
        await Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ListHandler.RefreshLocalMinecraftDownloadList();
                DialogHostUtils.Close();
            });
        }, _downloadListLoadingCancellationToken);
    }

    private void LoadingAndCancelViewCancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CancelLoadingDownloadList();
        DialogHostUtils.Close();
    }

    private static void CancelLoadingDownloadList()
    {
        _downloadListLoadingCancellationTokenSource.Cancel();
        _downloadListLoadingCancellationTokenSource = new CancellationTokenSource();
        _downloadListLoadingCancellationToken = _downloadListLoadingCancellationTokenSource.Token;
        MainWindow.Instance!.MenuTabControl.SelectedIndex = 1;
        MainWindow.Instance.StartRadioButton.IsChecked = true;
    }

    private void GoToSelectDownloadAttachmentsStackPanel(string mcVersion, VersionType versionType)
    {
        switch (versionType)
        {
            case VersionType.Official:
                SelectDownloadAttachmentsImage.Source =
                    new Bitmap(AssetLoader.Open(new Uri("avares://JackCraftLauncher/Assets/Icons/Grass_Block.png")));
                break;
            case VersionType.Beta:
                SelectDownloadAttachmentsImage.Source = new Bitmap(
                    AssetLoader.Open(new Uri("avares://JackCraftLauncher/Assets/Icons/Impulse_Command_Block.gif")));
                break;
            case VersionType.Old:
                SelectDownloadAttachmentsImage.Source = new Bitmap(
                    AssetLoader.Open(new Uri("avares://JackCraftLauncher/Assets/Icons/Badlands_Grass_Block.png")));
                break;
        }

        GlobalVariable.DownloadSelectAttachmentsModels = new ObservableCollection<DownloadSelectModel>();
        InstallAttachmentsTextBlock.IsVisible = false;
        ListHandler.RefreshLocalForgeDownloadList(mcVersion);

        InstallMinecraftVersionTextBlock.Text = mcVersion;
        DownloadSaveVersionNameTextBox.Text = mcVersion;
        SelectDownloadMinecraftVersionStackPanel.IsVisible = false;
        SelectDownloadAttachmentsStackPanel.IsVisible = true;
    }

    private void DownloadSaveVersionNameTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DownloadSaveVersionNameTextBox.Text!.Length > 0)
        {
            PleaseTypeVersionNameTextBlock.IsVisible = false;
            var folderPath =
                $"{App.Core.RootPath}/versions/{DownloadSaveVersionNameTextBox.Text}";
            if (Directory.Exists(folderPath))
            {
                if (string.IsNullOrWhiteSpace(DownloadSaveVersionNameTextBox.Text))
                {
                    PleaseTypeVersionNameTextBlock.IsVisible = true;
                    FolderAlreadyExistsTextBlock.IsVisible = false;
                    FolderNameInvalidCharacterTextBlock.IsVisible = false;
                    StartInstallButton.IsEnabled = false;
                }
                else
                {
                    FolderAlreadyExistsTextBlock.IsVisible = true;
                    FolderNameInvalidCharacterTextBlock.IsVisible = false;
                    StartInstallButton.IsEnabled = false;
                }
            }
            else
            {
                if (DownloadSaveVersionNameTextBox.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    PleaseTypeVersionNameTextBlock.IsVisible = false;
                    FolderAlreadyExistsTextBlock.IsVisible = false;
                    FolderNameInvalidCharacterTextBlock.IsVisible = true;
                    StartInstallButton.IsEnabled = false;
                }
                else
                {
                    PleaseTypeVersionNameTextBlock.IsVisible = false;
                    FolderAlreadyExistsTextBlock.IsVisible = false;
                    FolderNameInvalidCharacterTextBlock.IsVisible = false;
                    StartInstallButton.IsEnabled = true;
                }
            }
        }
        else
        {
            FolderAlreadyExistsTextBlock.IsVisible = false;
            PleaseTypeVersionNameTextBlock.IsVisible = true;
            FolderNameInvalidCharacterTextBlock.IsVisible = false;
            StartInstallButton.IsEnabled = false;
        }
    }

    private void VersionListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender!;
        if (listBox.SelectedIndex != -1)
        {
            var defaultDownloadList = (DefaultDownloadList)listBox.SelectedItem!;
            var selectVersion = defaultDownloadList.Version;
            GoToSelectDownloadAttachmentsStackPanel(selectVersion, defaultDownloadList.versionType);
            // InstallVersionName = selectVersion;
            listBox.SelectedIndex = -1;
        }
    }

    private void LatestReleaseVersionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GoToSelectDownloadAttachmentsStackPanel(GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion,
            VersionType.Official);
    }

    private void LatestSnapshotVersionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GoToSelectDownloadAttachmentsStackPanel(GlobalVariable.MinecraftDownload.LatestMinecraftSnapshotVersion,
            VersionType.Beta);
    }

    #endregion

    #region 开始安装界面

    private async void StartInstall()
    {
        MainWindow.Instance!.LoginRadioButton.IsEnabled = false;
        MainWindow.Instance.StartRadioButton.IsEnabled = false;
        MainWindow.Instance.DownloadRadioButton.IsEnabled = false;
        MainWindow.Instance.SettingRadioButton.IsEnabled = false;
        MainWindow.Instance.StartGameButton.IsEnabled = false;
        SelectDownloadMinecraftVersionStackPanel.IsVisible = false;
        SelectDownloadAttachmentsStackPanel.IsVisible = false;
        StartInstallGrid.IsVisible = true;
        InstallProgressBar.IsIndeterminate = false;
        InstallProgressBar.Value = 0;
        ClearInstallLog();
        try
        {
            #region 初始化

            var installMinecraftVersion = InstallMinecraftVersionTextBlock.Text!;
            var saveVersionName = DownloadSaveVersionNameTextBox.Text!;
            AddInstallLog(
                $"[{Localizer.Localizer.Instance["Initialize"]}] {string.Format(Localizer.Localizer.Instance["InstallingMinecraftWithVersionAndName"], installMinecraftVersion, saveVersionName)}");
            AddInstallLog(
                $"[{Localizer.Localizer.Instance["Initialize"]}] {string.Format(Localizer.Localizer.Instance["CreateFolder"])}");
            var folderPath =
                $"{App.Core.RootPath}/versions/{DownloadSaveVersionNameTextBox.Text}";
            DirectoryUtils.CreateDirectory(folderPath);
            var downloadSettings = new DownloadSettings
            {
                DownloadParts = GlobalVariable.Config.DownloadPartsCount,
                RetryCount = GlobalVariable.Config.DownloadRetryCount,
                CheckFile = true,
                Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds
            };
            InstallProgressBar.Value = 10;

            #endregion

            #region 下载Json

            AddInstallLog(
                $"[{Localizer.Localizer.Instance["Download"]}] - [{Localizer.Localizer.Instance["Start"]}] JSON - {DownloadSaveVersionNameTextBox.Text}.json");
            var idToFind = installMinecraftVersion; // 要查找的 id
            var versionModel =
                GlobalVariable.MinecraftDownload.MinecraftVersionManifestModel.VersionsModel?.FirstOrDefault(x =>
                    x.ID == idToFind);
            var minecraftJsonUrl =
                DownloadSourceHandler.PistonMetaUrlHandle(GlobalVariable.Config.DownloadSourceEnum, versionModel!.Url!);
            var downloadFile = new DownloadFile
            {
                DownloadUri = minecraftJsonUrl,
                FileName = $"{saveVersionName}.json",
                DownloadPath = folderPath,
                RetryCount = GlobalVariable.Config.DownloadRetryCount
            };
            await DownloadHelper.AdvancedDownloadFile(downloadFile, downloadSettings);
            AddInstallLog(
                $"[{Localizer.Localizer.Instance["Download"]}] - [{Localizer.Localizer.Instance["Completed"]}] JSON - {DownloadSaveVersionNameTextBox.Text}.json - {Localizer.Localizer.Instance["DownloadCompleted"]}");
            InstallProgressBar.Value = 20;

            #endregion

            #region 处理第三方加载器

            if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                    m.InstallAttachmentsType == DownloadAttachmentsType.Optifine))
                // Optifine install
                InstallProgressBar.Value = 35;
            if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                    m.InstallAttachmentsType == DownloadAttachmentsType.Forge))
            {
                // Forge install
                InstallProgressBar.Value = 50;
                AddInstallLog("初始化Forge安装");
                var forgeVersion = GlobalVariable.DownloadSelectAttachmentsModels
                    .FirstOrDefault(m => m.InstallAttachmentsType == DownloadAttachmentsType.Forge)?.Version!;
                var forgeFileName = $"forge-{installMinecraftVersion}-{forgeVersion}-installer.jar";
                var forgeUrl = $"{DownloadSourceHandler
                    .GetDownloadSource(DownloadSourceHandler.DownloadTargetEnum.ForgeMaven, null)}net/minecraftforge/forge/{installMinecraftVersion}-{forgeVersion}/{forgeFileName}";
                var forgeInstallFilePath = $"./JCL/cache/{forgeFileName}";
                var forgeTempVersionName = $"{saveVersionName}-temp";
                var forgeMcFolderPath = $"{App.Core.RootPath}/versions/{forgeTempVersionName}";
                DirectoryUtils.CreateDirectory(forgeMcFolderPath);
                var forgeInstallerDownloadFile = new DownloadFile
                {
                    DownloadUri = forgeUrl,
                    FileName = forgeFileName,
                    DownloadPath = "./JCL/cache",
                    RetryCount = GlobalVariable.Config.DownloadRetryCount
                };
                InstallProgressBar.Value = 60;
                AddInstallLog("开始下载Forge安装器");
                await DownloadHelper.AdvancedDownloadFile(forgeInstallerDownloadFile, downloadSettings);
                InstallProgressBar.Value = 70;
                AddInstallLog("下载Forge安装器完成");
                var isLegacy = ForgeInstallerFactory.IsLegacyForgeInstaller(forgeInstallFilePath, forgeVersion);
                IForgeInstaller forgeInstaller = isLegacy
                    ? new LegacyForgeInstaller
                    {
                        ForgeExecutablePath = forgeInstallFilePath,
                        RootPath = App.Core.RootPath,
                        CustomId = forgeTempVersionName,
                        ForgeVersion = forgeVersion,
                        InheritsFrom = installMinecraftVersion
                    }
                    : new HighVersionForgeInstaller
                    {
                        ForgeExecutablePath = forgeInstallFilePath,
                        JavaExecutablePath = GlobalVariable.Config.GameStartJavaPath,
                        RootPath = App.Core.RootPath,
                        CustomId = forgeTempVersionName,
                        VersionLocator = App.Core.VersionLocator,
                        DownloadUrlRoot =
                            DownloadSourceHandler.GetDownloadSource(
                                DownloadSourceHandler.DownloadTargetEnum.ForgeOldMaven,
                                null),
                        MineCraftVersion = installMinecraftVersion,
                        MineCraftVersionId = installMinecraftVersion,
                        InheritsFrom = installMinecraftVersion
                    };
                ((InstallerBase)forgeInstaller).StageChangedEventDelegate += (_, args) =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        InstallProgressBar2.IsVisible = true;
                        InstallProgressBar2.Value = args.Progress * 100;
                        AddInstallLog($"[Forge] - [{args.Progress * 100:0.00}] - {args.CurrentStage}");
                        if (Math.Abs(args.Progress - 1) < 0.01)
                            InstallProgressBar2.IsVisible = false;
                    });
                };
                InstallProgressBar.Value = 80;
                AddInstallLog("开始安装Forge");
                await forgeInstaller.InstallForgeTaskAsync();
                InstallProgressBar.Value = 90;

                AddInstallLog("Forge安装完成");
            }
            else if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                         m.InstallAttachmentsType == DownloadAttachmentsType.Fabric))
            {
                // Fabric install
            }
            else if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                         m.InstallAttachmentsType == DownloadAttachmentsType.Quilt))
            {
                // Quilt install
            }
            else if (GlobalVariable.DownloadSelectAttachmentsModels.Any(m =>
                         m.InstallAttachmentsType == DownloadAttachmentsType.LiteLoader))
            {
                // LiteLoader install
            }

            #endregion

            #region 安装完成

            ListHandler.RefreshLocalGameList();
            AddInstallLog(
                $"[{Localizer.Localizer.Instance["Install"]}] - [{Localizer.Localizer.Instance["Completed"]}] {string.Format(Localizer.Localizer.Instance["GameInstallCompleted"], installMinecraftVersion, saveVersionName)}");
            InstallProgressBar.Value = 100;
            var timeBack = 3;
            for (var i = timeBack; i > 0; i--)
            {
                AddInstallLog(string.Format(Localizer.Localizer.Instance["ReturnInSeconds"], i));
                await Task.Delay(1000);
            }

            #endregion
        }
        finally
        {
            MainWindow.Instance!.LoginRadioButton.IsEnabled = true;
            MainWindow.Instance.StartRadioButton.IsEnabled = true;
            MainWindow.Instance.DownloadRadioButton.IsEnabled = true;
            MainWindow.Instance.SettingRadioButton.IsEnabled = true;
            MainWindow.Instance.StartGameButton.IsEnabled = true;
            SelectDownloadMinecraftVersionStackPanel.IsVisible = true;
            SelectDownloadAttachmentsStackPanel.IsVisible = false;
            StartInstallGrid.IsVisible = false;
            InstallProgressBar.IsIndeterminate = true;
        }
    }

    public void ClearInstallLog()
    {
        InstallLogListBox.Items.Clear();
    }

    public void AddInstallLog(string log)
    {
        InstallLogListBox.Items.Add(log);
        if (InstallLogListBox.Scroll != null)
        {
            var listboxScroll = (ScrollViewer)InstallLogListBox.Scroll;
            listboxScroll.ScrollToEnd();
        }
    }

    private void InstallLogListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        InstallLogListBox.SelectedIndex = -1;
    }

    #endregion
}