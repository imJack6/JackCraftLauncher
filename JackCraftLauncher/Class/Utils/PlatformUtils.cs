using System;

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
}