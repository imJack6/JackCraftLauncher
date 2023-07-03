using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.MinecraftVersionManifest;

public class MinecraftLatestModel
{
    [JsonPropertyName("release")] public string Release { get; set; }
    [JsonPropertyName("snapshot")] public string Snapshot { get; set; }
}