using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.MinecraftDownloadModel;

public class MinecraftClientDownloadModel
{
    [JsonPropertyName("downloads")] public Downloads Downloads { get; set; }
}

public class Downloads
{
    [JsonPropertyName("client")] public Client Client { get; set; }
}

public class Client
{
    [JsonPropertyName("sha1")] public string Sha1 { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
}