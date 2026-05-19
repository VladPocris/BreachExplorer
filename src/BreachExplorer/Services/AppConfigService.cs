namespace BreachExplorer
{
    public interface IAppConfigService
    {
        string GetBackendApiUrl();
    }

    public class AppConfigService : IAppConfigService
    {
        private readonly IEnvLoader _envLoader;

        public AppConfigService(IEnvLoader envLoader)
        {
            _envLoader = envLoader;
        }

        public string GetBackendApiUrl()
        {
            var url = _envLoader.Get("URL_BACKEND_API");
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("URL_BACKEND_API not set in .env file");
            return url;
        }
    }
}



