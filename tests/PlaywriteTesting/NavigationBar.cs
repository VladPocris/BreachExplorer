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

            var homeButton = Page.Locator("li.nav-item a.nav-pill:has-text('Home')");
            Assert.IsTrue(await homeButton.IsVisibleAsync(), "Home link is not visible");
            await homeButton.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Home link navigation failed");

            var wasIBreachedButton = Page.Locator("li.nav-item a.nav-pill:has-text('Was I Breached?')");
            Assert.IsTrue(await wasIBreachedButton.IsVisibleAsync(), "Was I Breached link is not visible");
            await wasIBreachedButton.ClickAsync();
            Assert.IsTrue(Page.Url.Contains("/Breached", StringComparison.OrdinalIgnoreCase), "Was I Breached link navigation failed");

            var brandLink = Page.Locator("a.navbar-brand.nav-brand");
            Assert.IsTrue(await brandLink.IsVisibleAsync(), "Brand link is not visible");
            await brandLink.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Brand link navigation failed");

            await wasIBreachedButton.ClickAsync();
            var titleText = Page.Locator("span.nav-brand-text");
            Assert.IsTrue(await titleText.IsVisibleAsync(), "Brand title is not visible");
            await titleText.ClickAsync();
            Assert.AreEqual(NormalizeUrl(BaseUrl), NormalizeUrl(Page.Url), "Brand title navigation failed");
        }
    }
}
