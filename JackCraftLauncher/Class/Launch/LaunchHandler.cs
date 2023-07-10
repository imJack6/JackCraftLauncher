using System.Net;
using JackCraftLauncher.Class.Utils;
using ProjBobcat.Class.Model.MicrosoftAuth;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.DefaultComponent.Launch.GameCore;
using ProjBobcat.DefaultComponent.Logging;

namespace JackCraftLauncher.Class.Launch;

public class LaunchHandler
{
    public static DefaultGameCore InitLaunch()
    {
        #region 创建文件夹

        DirectoryUtils.CreateDirectory("./JCL");
        DirectoryUtils.CreateDirectory("./JCL/.minecraft");
        DirectoryUtils.CreateDirectory("./JCL/cache");

        #endregion

        ServicePointManager.DefaultConnectionLimit = 1024;
        var clientToken = new Guid("11451419-1981-0114-5141-919810114514");
        var rootPath = "JCL/.minecraft/";
        var core = new DefaultGameCore
        {
            ClientToken = clientToken,
            RootPath = rootPath,
            VersionLocator = new DefaultVersionLocator(rootPath, clientToken)
            {
                LauncherProfileParser = new DefaultLauncherProfileParser(rootPath, clientToken),
                LauncherAccountParser = new DefaultLauncherAccountParser(rootPath, clientToken)
            },
            GameLogResolver = new DefaultGameLogResolver()
        };
        MicrosoftAuthenticator.Configure(new MicrosoftAuthenticatorAPISettings
        {
            ClientId = EncryptHandler.JcDecrypt("fFlRXiZKWFZZfgRDWU5cAUp9TFpTJRNMU0AtUkZbBQ4GEytV"),
            TenentId = "consumers",
            Scopes = new[] { "XboxLive.signin", "offline_access", "openid", "profile", "email" }
        });
        return core;
    }
}