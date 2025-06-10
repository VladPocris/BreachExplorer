using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API_Proxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreachesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BreachesController> _logger;

        public BreachesController(HttpClient httpClient, ILogger<BreachesController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("breachedaccount/{email}")]
        public async Task<IActionResult> GetBreachedAccount(string email)
        {
            try
            {
                _logger.LogInformation("Request received for email: {Email}", email);

                string hibpApiKey = "189c24e9baf0438094b05ff14e22585d"; // Use environment variable for security
                if (string.IsNullOrEmpty(hibpApiKey))
                {
                    _logger.LogError("HIBP API key is missing.");
                    return StatusCode(500, new { message = "Internal server error: API key not configured." });
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("hibp-api-key", hibpApiKey);
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourApp/1.0");

                string encodedEmail = Uri.EscapeDataString(email);
                string uri = $"https://haveibeenpwned.com/api/v3/breachedaccount/{encodedEmail}";

                _logger.LogInformation("Making request to HIBP API: {Uri}", uri);

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
    }
}