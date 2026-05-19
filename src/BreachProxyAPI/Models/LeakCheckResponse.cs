using System.Text.Json.Serialization;

namespace API_Proxy.Models;

public class LeakCheckResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("found")]
    public int Found { get; set; }

    [JsonPropertyName("sources")]
    public List<LeakCheckSource>? Sources { get; set; }
}

public class LeakCheckSource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string? Date { get; set; }
}
