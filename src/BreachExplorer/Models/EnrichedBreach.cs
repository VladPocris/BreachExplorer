namespace BreachExplorer.Models;

public class EnrichedBreach
{
    public string SourceName { get; set; } = string.Empty;
    public string? LeakCheckDate { get; set; }
    public bool IsEnriched { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Domain { get; set; }
    public string? BreachDate { get; set; }
    public int PwnCount { get; set; }
    public string? Description { get; set; }
    public string? LogoPath { get; set; }
    public List<string>? DataClasses { get; set; }
    public bool IsVerified { get; set; }

    public string DisplayName => Title ?? Name ?? SourceName;
}

public class EmailBreachCheckResult
{
    public int TotalFound { get; set; }
    public List<EnrichedBreach> Breaches { get; set; } = new();
}
