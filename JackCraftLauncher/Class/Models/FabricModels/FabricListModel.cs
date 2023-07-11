using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.FabricModels;

public class FabricListModel
{
    [JsonPropertyName("loader")] public Loader Loader { get; set; }
    [JsonPropertyName("intermediary")] public Intermediary Intermediary { get; set; }
    [JsonPropertyName("launcherMeta")] public LauncherMeta LauncherMeta { get; set; }
}

public class Loader
{
    [JsonPropertyName("separator")] public string Separator { get; set; }
    [JsonPropertyName("build")] public int Build { get; set; }
    [JsonPropertyName("maven")] public string Maven { get; set; }
    [JsonPropertyName("version")] public string Version { get; set; }
    [JsonPropertyName("stable")] public bool Stable { get; set; }
}

public class Intermediary
{
    [JsonPropertyName("maven")] public string Maven { get; set; }
    [JsonPropertyName("version")] public string Version { get; set; }
    [JsonPropertyName("stable")] public bool Stable { get; set; }
}

public class LauncherMeta
{
    [JsonPropertyName("version")] public int Version { get; set; }
    [JsonPropertyName("libraries")] public Libraries Libraries { get; set; }
    [JsonPropertyName("mainClass")] public object MainClass { get; set; }
    [JsonPropertyName("arguments")] public Arguments Arguments { get; set; }
    [JsonPropertyName("launchwrapper")] public Launchwrapper LaunchWrapper { get; set; }
}

public class Libraries
{
    [JsonPropertyName("client")] public object[] Client { get; set; }
    [JsonPropertyName("common")] public Common[] Common { get; set; }
    [JsonPropertyName("server")] public Server[] Server { get; set; }
}

public class Common
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
}

public class Server
{
    [JsonPropertyName("_comment")] public string Comment { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
}

public class Arguments
{
    [JsonPropertyName("client")] public object[] Client { get; set; }
    [JsonPropertyName("common")] public object[] Common { get; set; }
    [JsonPropertyName("server")] public object[] Server { get; set; }
}

public class Launchwrapper
{
    [JsonPropertyName("tweakers")] public Tweakers Tweakers { get; set; }
}

public class Tweakers
{
    [JsonPropertyName("client")] public string[] Client { get; set; }
    [JsonPropertyName("common")] public object[] Common { get; set; }
    [JsonPropertyName("server")] public string[] Server { get; set; }
}