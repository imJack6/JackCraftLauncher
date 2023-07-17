using System.IO;

namespace JackCraftLauncher.Class.Launch;

public static class DownloadSourceHandler
{
    public enum DownloadSourceEnum
    {
        MCBBS,
        BMCL,
        Official
    }

    public enum DownloadTargetEnum
    {
        VersionInfoV1,
        VersionInfoV2,
        MinecraftJar,
        MinecraftJson,
        MinecraftLibraries,
        MinecraftAssets,
        MinecraftAssetsIndex,
        ForgeSupportMcList,
        ForgeMcList,
        Forge,
        ForgeOldMaven,
        ForgeMaven,
        FabricMeta,
        FabricMaven,
        OptifineMcList,
        OptifineJarDownload,
        QuiltSupportMcList,
        QuiltLoaderList,
        LiteLoaderMcList
    }

    public static string GetDownloadSource(DownloadTargetEnum target, DownloadSourceEnum? source,
        string? minecraftVersion = "1.0")
    {
        source ??= GlobalVariable.Config.DownloadSourceEnum;

        var baseUrl = source switch
        {
            DownloadSourceEnum.MCBBS => target switch
            {
                DownloadTargetEnum.QuiltSupportMcList or DownloadTargetEnum.QuiltLoaderList =>
                    "https://meta.quiltmc.org",
                _ => "https://download.mcbbs.net"
            },
            DownloadSourceEnum.BMCL => target switch
            {
                DownloadTargetEnum.QuiltSupportMcList or DownloadTargetEnum.QuiltLoaderList =>
                    "https://meta.quiltmc.org",
                _ => "https://bmclapi2.bangbang93.com"
            },
            DownloadSourceEnum.Official => target switch
            {
                DownloadTargetEnum.MinecraftJar or DownloadTargetEnum.MinecraftJson => "https://download.mcbbs.net",
                DownloadTargetEnum.MinecraftLibraries => "https://libraries.minecraft.net",
                DownloadTargetEnum.MinecraftAssets => "http://resources.download.minecraft.net",
                DownloadTargetEnum.MinecraftAssetsIndex => "https://launchermeta.mojang.com",
                DownloadTargetEnum.ForgeSupportMcList or DownloadTargetEnum.ForgeMcList => "https://download.mcbbs.net",
                DownloadTargetEnum.Forge or DownloadTargetEnum.ForgeOldMaven => "https://files.minecraftforge.net",
                DownloadTargetEnum.ForgeMaven => "https://maven.minecraftforge.net",
                DownloadTargetEnum.FabricMeta => "https://meta.fabricmc.net",
                DownloadTargetEnum.FabricMaven => "https://maven.fabricmc.net",
                DownloadTargetEnum.OptifineMcList => "https://download.mcbbs.net",
                DownloadTargetEnum.OptifineJarDownload => "https://download.mcbbs.net",
                DownloadTargetEnum.QuiltSupportMcList or DownloadTargetEnum.QuiltLoaderList =>
                    "https://meta.quiltmc.org",
                DownloadTargetEnum.LiteLoaderMcList => "https://download.mcbbs.net",
                _ => "http://launchermeta.mojang.com"
            },
            _ => throw new InvalidDataException($"Selected mirror field {source} does not exist.")
        };
        return target switch
        {
            DownloadTargetEnum.VersionInfoV1 => $"{baseUrl}/mc/game/version_manifest.json",
            DownloadTargetEnum.VersionInfoV2 => $"{baseUrl}/mc/game/version_manifest_v2.json",
            DownloadTargetEnum.MinecraftJar => $"{baseUrl}/version/{minecraftVersion}/client",
            DownloadTargetEnum.MinecraftJson => $"{baseUrl}/version/{minecraftVersion}/json",
            DownloadTargetEnum.MinecraftLibraries when source == DownloadSourceEnum.Official => $"{baseUrl}/",
            DownloadTargetEnum.MinecraftLibraries => $"{baseUrl}/maven/",
            DownloadTargetEnum.MinecraftAssets when source == DownloadSourceEnum.Official => $"{baseUrl}/",
            DownloadTargetEnum.MinecraftAssets => $"{baseUrl}/assets/",
            DownloadTargetEnum.MinecraftAssetsIndex => $"{baseUrl}/",
            DownloadTargetEnum.ForgeSupportMcList => $"{baseUrl}/forge/minecraft",
            DownloadTargetEnum.ForgeMcList => $"{baseUrl}/forge/minecraft/{minecraftVersion}",
            DownloadTargetEnum.Forge or DownloadTargetEnum.ForgeOldMaven => $"{baseUrl}/maven/",
            DownloadTargetEnum.ForgeMaven when source != DownloadSourceEnum.Official => $"{baseUrl}/maven/",
            DownloadTargetEnum.ForgeMaven => $"{baseUrl}/",
            DownloadTargetEnum.FabricMeta when source != DownloadSourceEnum.Official => $"{baseUrl}/fabric-meta/",
            DownloadTargetEnum.FabricMeta => $"{baseUrl}/",
            DownloadTargetEnum.FabricMaven when source != DownloadSourceEnum.Official => $"{baseUrl}/maven/",
            DownloadTargetEnum.FabricMaven => $"{baseUrl}/",
            DownloadTargetEnum.OptifineMcList => $"{baseUrl}/optifine/{minecraftVersion}",
            DownloadTargetEnum.OptifineJarDownload => $"{baseUrl}/maven/com/optifine/{minecraftVersion}",
            DownloadTargetEnum.QuiltSupportMcList => $"{baseUrl}/v3/versions/game",
            DownloadTargetEnum.QuiltLoaderList => $"{baseUrl}/v3/versions/loader",
            DownloadTargetEnum.LiteLoaderMcList => $"{baseUrl}/liteloader/list?mcversion={minecraftVersion}",
            _ => throw new InvalidDataException($"Selected target field {target} does not exist.")
        };
    }

    public static string PistonMetaUrlHandle(DownloadSourceEnum? source, string url)
    {
        var HandleString = url;
        var baseUrl = "";
        source ??= GlobalVariable.Config.DownloadSourceEnum;
        switch (source)
        {
            case DownloadSourceEnum.BMCL:
                baseUrl = "https://bmclapi2.bangbang93.com";
                break;
            case DownloadSourceEnum.MCBBS:
                baseUrl = "https://download.mcbbs.net";
                break;
            case DownloadSourceEnum.Official:
                baseUrl = "https://piston-meta.mojang.com";
                break;
            default:
                throw new InvalidDataException($"Selected mirror field {source} does not found");
        }

        HandleString = HandleString.Replace("https://piston-meta.mojang.com", baseUrl);
        return HandleString;
    }
}