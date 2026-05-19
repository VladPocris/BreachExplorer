using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BreachExplorer
{
    public interface IEnvLoader
    {
        Task LoadAsync();
        string? Get(string key);
    }

    public class EnvLoader : IEnvLoader
    {
        private readonly HttpClient _httpClient;
        private readonly IWebAssemblyHostEnvironment _hostEnvironment;
        private Dictionary<string, string> _env = new();

        public EnvLoader(HttpClient httpClient, IWebAssemblyHostEnvironment hostEnvironment)
        {
            _httpClient = httpClient;
            _hostEnvironment = hostEnvironment;
        }

        public async Task LoadAsync()
        {
            try
            {
                var envUri = new Uri(new Uri(_hostEnvironment.BaseAddress), "env.json");
                var response = await _httpClient.GetAsync(envUri);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"env.json not loaded ({(int)response.StatusCode}) from {envUri}");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var envData = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                if (envData != null)
                {
                    _env = envData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading env.json: {ex.Message}");
            }
        }

        public string? Get(string key)
        {
            _env.TryGetValue(key, out var value);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
