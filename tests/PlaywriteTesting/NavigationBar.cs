using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class NavigationBar : PageTest
    {
        private static string BaseUrl =>
            Environment.GetEnvironmentVariable("BLAZOR_URL") ?? "http://localhost:5241";

        private static string NormalizeUrl(string url) => url.TrimEnd('/');

        [TestMethod]
        public async Task NavigationBarLinksAreFunctional()
        {
            await Page.GotoAsync($"{BaseUrl}/Breached");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var homeButton = Page.Locator("li.nav-item a:has-text('Home')");
            Assert.IsTrue(await homeButton.IsVisibleAsync(), "Home link is not visible");
            await homeButton.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Home link navigation failed");

            var wasIBreachedButton = Page.Locator("li.nav-item a:has-text('Was I Breached?')");
            Assert.IsTrue(await wasIBreachedButton.IsVisibleAsync(), "Was I Breached link is not visible");
            await wasIBreachedButton.ClickAsync();
            Assert.IsTrue(Page.Url.Contains("/Breached", StringComparison.OrdinalIgnoreCase), "Was I Breached link navigation failed");

            var logoButton = Page.Locator("header .image-hover.zoom-in-out-element");
            Assert.IsTrue(await logoButton.IsVisibleAsync(), "Logo is not visible");
            await logoButton.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Logo navigation failed");

            await wasIBreachedButton.ClickAsync();
            var titleButton = Page.Locator("span.fs-4.fw-bold.text-light");
            Assert.IsTrue(await titleButton.IsVisibleAsync(), "Title is not visible");
            await titleButton.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Title navigation failed");
        }
    }
}
