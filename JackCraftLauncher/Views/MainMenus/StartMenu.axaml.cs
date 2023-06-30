using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Launch;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Class.Utils;
using Material.Dialog;
using Material.Dialog.Icons;
using Material.Styles.Controls;
using Material.Styles.Models;

namespace JackCraftLauncher.Views.MainMenus;

public partial class StartMenu : UserControl
{
    public static StartMenu? Instance { get; private set; }

    public StartMenu()
    {
        InitializeComponent();
        Instance = this;
        
        Task.Run(async () =>
        {
            while (true)
            {
                Dispatcher.UIThread.InvokeAsync(ListHandler.RefreshLocalGameList);
                await Task.Delay(1000);
            }
        });
    }

    private async void EditionSelectTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (EditionSelectTabControl != null)
            if (EditionSelectTabControl.SelectedIndex == 1)
            {
                if (PlatformUtils.GetOperatingSystem() != PlatformUtils.OperatingSystem.Windows)
                {
                    DialogHost.Show(new WarningTemplateModel(Localizer.Localizer.Instance["YourOperatingSystemNotSupported"],Localizer.Localizer.Instance["MinecraftBedrockEditionOnlySupportWindows"]), "MainDialogHost");
                    EditionSelectTabControl.SelectedIndex = 0;
                }
                else
                {
                    await GameHandler.CheckMCBedrockInstalled();
                }
            }
    }

    private void GoToMicrosoftStoreButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var psi = new ProcessStartInfo("ms-windows-store://pdp/?ProductId=9nblggh2jhxj")
        {
            UseShellExecute = true
        };

        Process.Start(psi);
        
        var helloSnackBar = new SnackbarModel(Localizer.Localizer.Instance["AttemptedStartMicrosoftStore"], TimeSpan.FromSeconds(3));
        SnackbarHost.Post(helloSnackBar, null, DispatcherPriority.Normal);
    }

    private async void RefreshMCBedrockInstalledButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await GameHandler.CheckMCBedrockInstalled();
    }
}