using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class Breached : PageTest
    {
        [TestMethod]
        public async Task BreachedTestIfReturnsInformation()
        {

            //Account with found breaches
            await Page.GotoAsync("https://gcstorageacc2.z9.web.core.windows.net/breached");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var emailInput = Page.Locator("input.form-control");
            var buttonBreached = Page.Locator("button#searchPwnage");
            await emailInput.FillAsync("vpocris@gmail.com");
            await buttonBreached.ClickAsync();
            await Page.WaitForTimeoutAsync(5000);
            var responseElement = Page.Locator("h2.neon.typing-container.text-center.fs-1 > span.style-purple-2.text-danger");
            var responseText = await responseElement.InnerTextAsync();
            Assert.IsTrue(responseText.Contains("Oh no!"), "Response did not contain the expected text.");

            //Account with no breaches
            await Page.GotoAsync("https://gcstorageacc2.z9.web.core.windows.net/breached");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await emailInput.FillAsync("vlapocris@gmail.com");
            await buttonBreached.ClickAsync();
            await Page.WaitForTimeoutAsync(5000);
            responseElement = Page.Locator("h2.neon.typing-container.text-center.fs-1 > span.style-purple-2.text-success");
            responseText = await responseElement.InnerTextAsync();
            Assert.IsTrue(responseText.Contains("Congrats!!"), "Response did not contain the expected text.");
        }
    }
}
