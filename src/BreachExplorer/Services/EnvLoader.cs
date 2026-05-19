using System.Text.Json;

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
        private Dictionary<string, string> _env = new();

        public EnvLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LoadAsync()
        {
            try
            {
                // Load from env.json in wwwroot
                var response = await _httpClient.GetAsync("env.json");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var envData = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                    if (envData != null)
                    {
                        _env = envData;
                    }
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
            return value;
        }
    }
}

