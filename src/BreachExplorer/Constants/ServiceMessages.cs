namespace BreachExplorer.Constants;

public static class ServiceMessages
{
    public const string BackendUnavailableTitle = "API is starting up";

    public const string BackendUnavailableLead =
        "Please wait about 1 minute, then reload this page or press Retry.";

    public const string BackendUnavailableDetail =
        "This app uses a backend hosted on Render.com's free tier. After a period of inactivity, the server spins down and usually needs about one minute to start again when you make a new request.";
}
