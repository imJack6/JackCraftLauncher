using Avalonia.Threading;
using DialogHostAvalonia;
using JackCraftLauncher.Class.Models;
using JackCraftLauncher.Views;
using Material.Styles.Controls;

namespace JackCraftLauncher.Class.Utils;

public class ClipboardUtils
{
    public static async void SetTextAsync(string text)
    {
        try
        {
            if (MainWindow.Instance!.Clipboard != null)
            {
                await MainWindow.Instance.Clipboard.SetTextAsync(text);
                SnackbarHost.Post(Localizer.Localizer.Instance["CopiedToClipboard"], null, DispatcherPriority.Normal);
            }
            else
            {
                DialogHost.Show(
                    new WarningTemplateModel(Localizer.Localizer.Instance["ClipboardError"],
                        $"{Localizer.Localizer.Instance["CopyContentToClipboardError"]} - {Localizer.Localizer.Instance["ClipboardIsNull"]}"),
                    "MainDialogHost");
            }
        }
        catch (Exception ex)
        {
            DialogHost.Show(
                new WarningTemplateModel(Localizer.Localizer.Instance["ClipboardError"],
                    $"{Localizer.Localizer.Instance["CopyContentToClipboardError"]}:\n{ex}"), "MainDialogHost");
        }
    }
}