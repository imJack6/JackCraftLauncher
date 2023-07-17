using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.LiteLoaderModels;

public class LiteLoaderMcVersionModel
{
    [JsonPropertyName("_id")] public string Id { get; set; }
    [JsonPropertyName("mcversion")] public string McVersion { get; set; }
    [JsonPropertyName("build")] public Build Build { get; set; }
    [JsonPropertyName("hash")] public string Hash { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("version")] public string Version { get; set; }
    [JsonPropertyName("__v")] public int V { get; set; }
}

public class Build
{
    [JsonPropertyName("tweakClass")] public string TweakClass { get; set; }
    [JsonPropertyName("libraries")] public Libraries[] Libraries { get; set; }
    [JsonPropertyName("stream")] public string Stream { get; set; }
    [JsonPropertyName("file")] public string File { get; set; }
    [JsonPropertyName("version")] public string Version { get; set; }
    [JsonPropertyName("build")] public string _Build { get; set; }
    [JsonPropertyName("md5")] public string Md5 { get; set; }
    [JsonPropertyName("timestamp")] public string TimeStamp { get; set; }

    [JsonPropertyName("lastSuccessfulBuild")]
    public int LastSuccessfulBuild { get; set; }
}

public class Libraries
{
    [JsonPropertyName("name")] public string Name { get; set; }
}