using System.Text.Json.Serialization;

namespace API_Proxy.Models;

public class HibpBreach
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("Domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("BreachDate")]
    public string? BreachDate { get; set; }

    [JsonPropertyName("PwnCount")]
    public int PwnCount { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    [JsonPropertyName("LogoPath")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("DataClasses")]
    public List<string>? DataClasses { get; set; }

    [JsonPropertyName("IsVerified")]
    public bool IsVerified { get; set; }
}
