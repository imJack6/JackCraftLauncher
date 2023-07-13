using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.QuiltModels;

public class QuiltListModel
{
    [JsonPropertyName("separator")] public char Separator { get; set; }
    [JsonPropertyName("build")] public int Build { get; set; }
    [JsonPropertyName("maven")] public string Maven { get; set; }
    [JsonPropertyName("version")] public string Version { get; set; }
}