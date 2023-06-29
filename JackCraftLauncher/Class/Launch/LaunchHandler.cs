using System;
using System.Net;
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
            ClientId = "f7bbc3a7-8c12-4f20-9ac4-0237cbff25e1",
            TenentId = "consumers",
            Scopes = new[] { "XboxLive.signin", "offline_access", "openid", "profile", "email" }
        });
        return core;
    }
}