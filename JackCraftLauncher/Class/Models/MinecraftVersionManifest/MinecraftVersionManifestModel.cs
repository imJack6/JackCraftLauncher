using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.MinecraftVersionManifest;

public class MinecraftVersionManifestModel
{
    [JsonPropertyName("latest")] public MinecraftLatestModel? LatestModel { get; set; }
    [JsonPropertyName("versions")] public MinecraftVersionsModel[]? VersionsModel { get; set; }
}