using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class NavigationBar : PageTest
    {
        [TestMethod]
        public async Task NavigationBarLinksAreFunctional()
        {
            //I start from /breached because I want to see if it actually changes the link after the click is performed
            await Page.GotoAsync("http://localhost:5241/breached");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Test Home Navigation button
            var homeButton = Page.Locator("li.nav-item a:has-text('Home')");
            Assert.IsTrue(await homeButton.IsVisibleAsync(), "Home link is not visible");
            await homeButton.ClickAsync();
            Assert.IsTrue(await homeButton.IsVisibleAsync(), "Home link is not visible");
            Assert.AreEqual("http://localhost:5241/", Page.Url, "Home link navigation failed");

            //Test Was I Breached? Navigation button
            var wasibreachedButton = Page.Locator("li.nav-item a:has-text('Was I Breached?')");
            Assert.IsTrue(await wasibreachedButton.IsVisibleAsync(), "Was I Breached link is not visible");
            await wasibreachedButton.ClickAsync();
            Assert.IsTrue(await wasibreachedButton.IsVisibleAsync(), "Was I Breached is not visible");
            Assert.AreEqual("http://localhost:5241/breached", Page.Url, "Was I Breached link navigation failed");

            await wasibreachedButton.ClickAsync();
            //Test Logo svg
            var logoButton = Page.Locator("header .image-hover.zoom-in-out-element");
            Assert.IsTrue(await logoButton.IsVisibleAsync(), "Logo is not visible");
            await logoButton.ClickAsync();
            Assert.IsTrue(await logoButton.IsVisibleAsync(), "Logo is not visible");
            Assert.AreEqual("http://localhost:5241/", Page.Url, "Logo navigation failed");

            await wasibreachedButton.ClickAsync();
            //Test website name on navigation bar
            var titleButton = Page.Locator("span.fs-4.fw-bold.text-light");
            Assert.IsTrue(await titleButton.IsVisibleAsync(), "Title is not visible");
            await titleButton.ClickAsync();
            Assert.IsTrue(await titleButton.IsVisibleAsync(), "Title is not visible");
            Assert.AreEqual("http://localhost:5241/", Page.Url, "Title navigation failed");
        }
    }
}
