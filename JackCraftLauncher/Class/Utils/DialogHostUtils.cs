using DialogHostAvalonia;

namespace JackCraftLauncher.Class.Utils;

public class DialogHostUtils
{
    public static void Show(object content, string? dialogIdentifier = "MainDialogHost")
    {
        DialogHost.Show(content, dialogIdentifier);
    }

    public static void Close(string? dialogIdentifier = "MainDialogHost", string? parameter = null)
    {
        if (DialogHost.IsDialogOpen(dialogIdentifier))
            DialogHost.Close(dialogIdentifier, parameter);
    }
}