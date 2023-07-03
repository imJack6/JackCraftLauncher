using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.MinecraftVersionManifest;

public class MinecraftVersionsModel
{
    [JsonPropertyName("id")] public string? ID { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("time")] public DateTime Time { get; set; }
    [JsonPropertyName("releaseTime")] public DateTime ReleaseTime { get; set; }
}