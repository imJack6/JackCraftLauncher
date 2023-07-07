namespace JackCraftLauncher.Class.ConfigHandler;

public static class DefaultConfigConstants
{
    public class GlobalGameSettingsNodes
    {
        public const string SelectedJavaIndexNode = "GlobalGameSettings.SelectedJavaIndex";
        public const string JavaPathListNode = "GlobalGameSettings.JavaPathList";
        public const string GcTypeNode = "GlobalGameSettings.GcType";
        public const string ResolutionWidthNode = "GlobalGameSettings.ResolutionWidth";
        public const string ResolutionHeightNode = "GlobalGameSettings.ResolutionHeight";
    }

    public class DownloadSettingsNodes
    {
        public const string DownloadSourceNode = "DownloadSettings.DownloadSource";
        public const string MaxDegreeOfParallelismCountNode = "DownloadSettings.MaxDegreeOfParallelismCount";
        public const string PartsCountNode = "DownloadSettings.PartsCount";
        public const string RetryCountNode = "DownloadSettings.RetryCount";
    }

    public class LauncherSettingsNodes
    {
        public const string ThemeNode = "LauncherSettings.ThemeMode";
    }

    public class LoginInformationNodes
    {
        public const string LoginModeNode = "LoginInformation.LoginMode";
        public const string UsernameNode = "LoginInformation.Username";
        public const string MicrosoftLoginRefreshTokenNode = "LoginInformation.MicrosoftLogin.RefreshToken";
        public const string YggdrasilLoginAuthServerNode = "LoginInformation.YggdrasilLogin.AuthServer";
        public const string YggdrasilLoginEmailNode = "LoginInformation.YggdrasilLogin.Email";
        public const string YggdrasilLoginPasswordNode = "LoginInformation.YggdrasilLogin.Password";
    }
}