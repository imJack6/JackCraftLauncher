using System.Runtime.InteropServices;

namespace JackCraftLauncher.Class.Utils;

public class PlatformUtils
{
    public enum OperatingSystem
    {
        Windows,
        Linux,
        Macos
    }

    public static OperatingSystem GetOperatingSystem()
    {
#if WINDOWS
        return OperatingSystem.Windows;
#elif LINUX
        return OperatingSystem.Linux;
#elif OSX
        return OperatingSystem.Macos;
#endif
    }

    public static string GetSystemUserDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "/Users/" + Environment.UserName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "/home/" + Environment.UserName;
        throw new PlatformNotSupportedException();
    }
}