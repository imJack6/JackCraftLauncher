using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.QuiltModels;

public class QuiltSupportMcModel
{
    [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
    [JsonPropertyName("stable")] public bool Stable { get; set; }
}