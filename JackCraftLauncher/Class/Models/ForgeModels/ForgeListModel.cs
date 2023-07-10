using System.Text.Json.Serialization;

namespace JackCraftLauncher.Class.Models.ForgeModels;

public class ForgeListModel
{
    [JsonPropertyName("_id")] public string? Id { get; set; }
    [JsonPropertyName("__v")] public int V { get; set; }
    [JsonPropertyName("build")] public int Build { get; set; }
    [JsonPropertyName("files")] public Files[]? Files { get; set; }
    [JsonPropertyName("mcversion")] public string? McVersion { get; set; }
    [JsonPropertyName("modified")] public DateTime Modified { get; set; }
    [JsonPropertyName("version")] public string? Version { get; set; }
}

public class Files
{
    [JsonPropertyName("format")] public string? Format { get; set; }
    [JsonPropertyName("category")] public string? Category { get; set; }
    [JsonPropertyName("hash")] public string? Hash { get; set; }
}