using System.IO;
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
using JackCraftLauncher.Class.Models.ListTemplate;
using JackCraftLauncher.Class.Utils;

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

    public static void CancelLoadingDownloadList()
    {
        _downloadListLoadingCancellationTokenSource.Cancel();
        _downloadListLoadingCancellationTokenSource = new CancellationTokenSource();
        _downloadListLoadingCancellationToken = _downloadListLoadingCancellationTokenSource.Token;
        MainWindow.Instance!.MenuTabControl.SelectedIndex = 1;
        MainWindow.Instance.StartRadioButton.IsChecked = true;
    }

    public void GoToSelectDownloadAttachmentsStackPane(string mcVersion, VersionType versionType)
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

        InstallMinecraftVersionTextBlock.Text = mcVersion;
        DownloadSaveVersionNameTextBox.Text = mcVersion;
        SelectDownloadMinecraftVersionStackPanel.IsVisible = false;
        SelectDownloadAttachmentsStackPanel.IsVisible = true;
    }

    private void BackToSelectVersionButton_Click(object sender, RoutedEventArgs e)
    {
        InstallMinecraftVersionTextBlock.Text = string.Empty;
        DownloadSaveVersionNameTextBox.Text = string.Empty;
        SelectDownloadMinecraftVersionStackPanel.IsVisible = true;
        SelectDownloadAttachmentsStackPanel.IsVisible = false;
    }

    private void DownloadSaveVersionNameTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DownloadSaveVersionNameTextBox.Text!.Length > 0)
        {
            PleaseTypeVersionNameTextBlock.IsVisible = false;
            var folderPath =
                $"{App.Core.RootPath}{Path.DirectorySeparatorChar}versions{Path.DirectorySeparatorChar}{DownloadSaveVersionNameTextBox.Text}";
            if (Directory.Exists(folderPath))
            {
                if (string.IsNullOrWhiteSpace(DownloadSaveVersionNameTextBox.Text))
                {
                    PleaseTypeVersionNameTextBlock.IsVisible = true;
                    FolderAlreadyExistsTextBlock.IsVisible = false;
                    StartInstallButton.IsEnabled = false;
                }
                else
                {
                    FolderAlreadyExistsTextBlock.IsVisible = true;
                    StartInstallButton.IsEnabled = false;
                }
            }
            else
            {
                FolderAlreadyExistsTextBlock.IsVisible = false;
                StartInstallButton.IsEnabled = true;
            }
        }
        else
        {
            FolderAlreadyExistsTextBlock.IsVisible = false;
            PleaseTypeVersionNameTextBlock.IsVisible = true;
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
            GoToSelectDownloadAttachmentsStackPane(selectVersion, defaultDownloadList.versionType);
            // InstallVersionName = selectVersion;
            listBox.SelectedIndex = -1;
        }
    }

    private void LatestReleaseVersionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GoToSelectDownloadAttachmentsStackPane(GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion,
            VersionType.Official);
    }

    private void LatestSnapshotVersionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GoToSelectDownloadAttachmentsStackPane(GlobalVariable.MinecraftDownload.LatestMinecraftReleaseVersion,
            VersionType.Beta);
    }
}