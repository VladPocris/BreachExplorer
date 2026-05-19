using System.Text.Json;
using System.Text.RegularExpressions;
using API_Proxy.Models;

namespace API_Proxy.Services;

public interface IBreachCheckService
{
    Task<EmailBreachCheckResult> CheckEmailAsync(string email, CancellationToken cancellationToken = default);
}

public class BreachCheckService : IBreachCheckService
{
    private const int MaxSourcesToEnrich = 120;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BreachCheckService> _logger;

    public BreachCheckService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BreachCheckService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<EmailBreachCheckResult> CheckEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var leakCheckSources = await FetchLeakCheckSourcesAsync(email, cancellationToken);
        var uniqueSources = leakCheckSources
            .GroupBy(s => s.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Take(MaxSourcesToEnrich)
            .ToList();

        var enriched = new List<EnrichedBreach>();
        foreach (var source in uniqueSources)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var hibp = await TryFetchHibpBreachAsync(source.Name, cancellationToken);
            enriched.Add(MapToEnrichedBreach(source, hibp));
            await Task.Delay(150, cancellationToken);
        }

        return new EmailBreachCheckResult
        {
            TotalFound = leakCheckSources.Count,
            Breaches = enriched
        };
    }

    private async Task<List<LeakCheckSource>> FetchLeakCheckSourcesAsync(string email, CancellationToken cancellationToken)
    {
        var baseUrl = _configuration["URL_LEAKCHECK_API"] ?? "https://leakcheck.io/api/";
        var uri = $"{baseUrl.TrimEnd('/')}/public?check={Uri.EscapeDataString(email)}";

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Agent", "BreachExplorer/1.0");

        var response = await client.GetAsync(uri, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("LeakCheck returned {StatusCode}: {Body}", response.StatusCode, content);
            throw new HttpRequestException($"LeakCheck API error ({(int)response.StatusCode}).");
        }

        var leakCheck = JsonSerializer.Deserialize<LeakCheckResponse>(content, JsonOptions);
        if (leakCheck is null || !leakCheck.Success)
        {
            return new List<LeakCheckSource>();
        }

        return leakCheck.Sources ?? new List<LeakCheckSource>();
    }

    private async Task<HibpBreach?> TryFetchHibpBreachAsync(string sourceName, CancellationToken cancellationToken)
    {
        var breachApiUrl = _configuration["URL_BREACH_API"] ?? "https://haveibeenpwned.com/api/v3";
        var breachApiKey = _configuration["KEY_BREACH_API"] ?? string.Empty;

        if (string.IsNullOrEmpty(breachApiKey))
        {
            return null;
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("hibp-api-key", breachApiKey);
        client.DefaultRequestHeaders.Add("User-Agent", "BreachExplorer/1.0");

        foreach (var candidate in GetHibpNameCandidates(sourceName))
        {
            var encoded = Uri.EscapeDataString(candidate);
            var uri = $"{breachApiUrl.TrimEnd('/')}/breach/{encoded}";

            try
            {
                var response = await client.GetAsync(uri, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("HIBP lookup failed for {Candidate}: {Status}", candidate, response.StatusCode);
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<HibpBreach>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "HIBP lookup error for {Candidate}", candidate);
            }
        }

        return null;
    }

    internal static IEnumerable<string> GetHibpNameCandidates(string leakCheckName)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var candidate in EnumerateCandidates(leakCheckName))
        {
            if (seen.Add(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static IEnumerable<string> EnumerateCandidates(string leakCheckName)
    {
        yield return leakCheckName;

        var withoutTld = Regex.Replace(
            leakCheckName,
            @"\.(com|net|org|io|ru|me|tv|biz|info|co\.uk|xyz|app|dev|vn|kz|ma|bg|au|in|il|br|jp|kr|ua|su)$",
            "",
            RegexOptions.IgnoreCase);

        if (!string.Equals(withoutTld, leakCheckName, StringComparison.OrdinalIgnoreCase))
        {
            yield return withoutTld;
        }

        var noSpaces = leakCheckName.Replace(" ", "", StringComparison.Ordinal);
        if (!string.Equals(noSpaces, leakCheckName, StringComparison.Ordinal))
        {
            yield return noSpaces;
        }

        var noParens = Regex.Replace(leakCheckName, @"\s*\([^)]*\)\s*", "").Trim();
        if (!string.IsNullOrEmpty(noParens) && !string.Equals(noParens, leakCheckName, StringComparison.OrdinalIgnoreCase))
        {
            yield return noParens;
            var noParensNoTld = Regex.Replace(
                noParens,
                @"\.(com|net|org|io|ru|me|tv|biz|info|co\.uk)$",
                "",
                RegexOptions.IgnoreCase);
            if (!string.Equals(noParensNoTld, noParens, StringComparison.OrdinalIgnoreCase))
            {
                yield return noParensNoTld;
            }
        }

        var alphanumeric = Regex.Replace(leakCheckName, @"[^a-zA-Z0-9]", "");
        if (!string.IsNullOrEmpty(alphanumeric) && alphanumeric.Length >= 3)
        {
            yield return alphanumeric;
        }
    }

    private static EnrichedBreach MapToEnrichedBreach(LeakCheckSource source, HibpBreach? hibp)
    {
        if (hibp is null)
        {
            return new EnrichedBreach
            {
                SourceName = source.Name,
                LeakCheckDate = string.IsNullOrWhiteSpace(source.Date) ? null : source.Date,
                IsEnriched = false,
                Name = source.Name,
                BreachDate = string.IsNullOrWhiteSpace(source.Date) ? null : source.Date
            };
        }

        return new EnrichedBreach
        {
            SourceName = source.Name,
            LeakCheckDate = string.IsNullOrWhiteSpace(source.Date) ? null : source.Date,
            IsEnriched = true,
            Name = hibp.Name,
            Title = hibp.Title,
            Domain = hibp.Domain,
            BreachDate = hibp.BreachDate ?? source.Date,
            PwnCount = hibp.PwnCount,
            Description = hibp.Description,
            LogoPath = hibp.LogoPath,
            DataClasses = hibp.DataClasses,
            IsVerified = hibp.IsVerified
        };
    }
}
