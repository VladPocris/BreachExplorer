using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class Breached : PageTest
    {
        private static string BaseUrl =>
            Environment.GetEnvironmentVariable("BLAZOR_URL") ?? "http://localhost:5241";

        [TestMethod]
        public async Task BreachedPageLoadsAndAcceptsEmailInput()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Inconclusive("Full breach lookup is skipped in CI (depends on live API and Render cold start).");
            }

            await Page.GotoAsync($"{BaseUrl}/Breached");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var emailInput = Page.Locator("input.breached-page__input");
            var checkButton = Page.Locator("button.breached-page__button");

            await emailInput.FillAsync("test@example.com");
            Assert.IsTrue(await checkButton.IsEnabledAsync());
            await checkButton.ClickAsync();

            await Page.WaitForTimeoutAsync(3000);
            Assert.IsTrue(await Page.Locator(".breached-page").IsVisibleAsync());
        }
    }
}
