using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API_Proxy.Services;

namespace API_Proxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreachesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BreachesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPasswordGeneratorService _passwordGeneratorService;
        private readonly IBreachCheckService _breachCheckService;

        public BreachesController(
            HttpClient httpClient,
            ILogger<BreachesController> logger,
            IConfiguration configuration,
            IPasswordGeneratorService passwordGeneratorService,
            IBreachCheckService breachCheckService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _passwordGeneratorService = passwordGeneratorService;
            _breachCheckService = breachCheckService;
        }

        private string GetHibpApiKey() =>
            _configuration["KEY_BREACH_API"]
            ?? _configuration["ApiKeys:HibpApiKey"]
            ?? string.Empty;

        private async Task<IActionResult> ForwardBreachApiRequest(string relativePath)
        {
            string breachApiUrl = _configuration["URL_BREACH_API"] ?? "https://haveibeenpwned.com/api/v3";
            string breachApiKey = GetHibpApiKey();

            if (string.IsNullOrEmpty(breachApiKey))
            {
                _logger.LogError("Breach API key is missing.");
                return StatusCode(500, new { message = "Internal server error: API key not configured." });
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("hibp-api-key", breachApiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BreachExplorer/1.0");

            string uri = $"{breachApiUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

            _logger.LogInformation("Making request to Breach API: {Uri}", uri);
            _logger.LogDebug("Using API key: {ApiKeyMasked}", breachApiKey.Length > 0 ? $"{breachApiKey.Substring(0, 4)}...{breachApiKey.Substring(breachApiKey.Length - 4)}" : "None");

            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            _logger.LogError("Request failed with status code {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpGet("latestbreach")]
        public Task<IActionResult> GetLatestBreach()
        {
            return ForwardBreachApiRequest("latestbreach");
        }

        [HttpGet("breaches")]
        public Task<IActionResult> GetBreaches()
        {
            return ForwardBreachApiRequest("breaches");
        }

        [HttpGet("breachedaccount/{email}")]
        public async Task<IActionResult> GetBreachedAccount(string email)
        {
            try
            {
                _logger.LogInformation("Request received for email: {Email}", email);

                string breachApiUrl = _configuration["URL_BREACH_API"] ?? "https://haveibeenpwned.com/api/v3";
                string breachApiKey = GetHibpApiKey();

                if (string.IsNullOrEmpty(breachApiKey))
                {
                    _logger.LogError("Breach API key is missing.");
                    return StatusCode(500, new { message = "Internal server error: API key not configured." });
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("hibp-api-key", breachApiKey);
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourApp/1.0");

                string encodedEmail = Uri.EscapeDataString(email);
                string uri = $"{breachApiUrl}/breachedaccount/{encodedEmail}";

                _logger.LogInformation("Making request to Breach API: {Uri}", uri);

                var response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Response received successfully for email: {Email}", email);
                    return Content(content, "application/json");
                }
                else
                {
                    _logger.LogError("Request failed with status code {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Error occurred while processing request: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check/{email}")]
        public async Task<IActionResult> CheckEmailBreaches(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return BadRequest(new { message = "A valid email address is required." });
            }

            try
            {
                _logger.LogInformation("LeakCheck + HIBP check for {Email}", email);
                var result = await _breachCheckService.CheckEmailAsync(email.Trim(), cancellationToken);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Breach check failed for {Email}", email);
                return StatusCode(502, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during breach check for {Email}", email);
                return StatusCode(500, new { message = "Failed to check breaches. Please try again." });
            }
        }

        [HttpGet("breach/{name}")]
        public Task<IActionResult> GetBreachDetails(string name)
        {
            return ForwardBreachApiRequest($"breach/{Uri.EscapeDataString(name)}");
        }

        [HttpGet("generate-password")]
        public async Task<IActionResult> GeneratePassword([FromQuery] int length = 16, [FromQuery] bool includeNumbers = true, [FromQuery] bool includeSpecialChars = true)
        {
            try
            {
                _logger.LogInformation("Password generation requested: length={Length}, numbers={Numbers}, special={Special}", length, includeNumbers, includeSpecialChars);

                if (length < 1 || length > 64)
                {
                    return BadRequest(new { error = "Password length must be between 1 and 64 characters." });
                }

                string jsonResponse = await _passwordGeneratorService.GeneratePasswordAsync(length, includeNumbers, includeSpecialChars);
                _logger.LogInformation("Password generated successfully with length {Length}", length);

                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating password: {Message}", ex.Message);
                return StatusCode(500, new { error = "Failed to generate password." });
            }
        }
    }
}