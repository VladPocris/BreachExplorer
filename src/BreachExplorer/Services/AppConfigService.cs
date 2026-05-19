using Microsoft.Extensions.Configuration;

namespace BreachExplorer
{
    public interface IAppConfigService
    {
        string GetBackendApiUrl();
    }

    public class AppConfigService : IAppConfigService
    {
        private readonly IEnvLoader _envLoader;
        private readonly IConfiguration _configuration;

        public AppConfigService(IEnvLoader envLoader, IConfiguration configuration)
        {
            _envLoader = envLoader;
            _configuration = configuration;
        }

        public string GetBackendApiUrl()
        {
            // env.json overrides appsettings (used by CI / local wwwroot/env.json)
            var fromEnvFile = _envLoader.Get("URL_BACKEND_API");
            if (!string.IsNullOrWhiteSpace(fromEnvFile))
                return fromEnvFile.Trim();

            var fromConfig = _configuration["URL_BACKEND_API"]
                ?? _configuration["ApiSettings:BackendApiUrl"];
            if (!string.IsNullOrWhiteSpace(fromConfig))
                return fromConfig.Trim();

            throw new InvalidOperationException(
                "Backend API URL not configured. Set URL_BACKEND_API in wwwroot/env.json or ApiSettings:BackendApiUrl in appsettings.json.");
        }
    }
}
