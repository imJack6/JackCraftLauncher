using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.OptifineModels;

public class OptifineListModel
{
    [JsonPropertyName("_id")] public string Id { get; set; }
    [JsonPropertyName("mcversion")] public string McVersion { get; set; }
    [JsonPropertyName("patch")] public string Patch { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("__v")] public int V { get; set; }
    [JsonPropertyName("filename")] public string Filename { get; set; }
    [JsonPropertyName("forge")] public string Forge { get; set; }
}