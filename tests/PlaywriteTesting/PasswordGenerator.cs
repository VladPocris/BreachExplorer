using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class PasswordGenerator : PageTest
    {
        private static string BaseUrl =>
            Environment.GetEnvironmentVariable("BLAZOR_URL") ?? "http://localhost:5241";

        [TestMethod]
        public async Task PasswordGeneratorIsFunctional()
        {
            await Page.GotoAsync($"{BaseUrl}/");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var slider = Page.Locator("input.form-range");
            await slider.EvaluateAsync("element => element.value = '32'");
            await slider.DispatchEventAsync("input");

            var checkboxNumbers = Page.Locator("#numbers");
            await checkboxNumbers.SetCheckedAsync(true);
            Assert.IsTrue(await checkboxNumbers.IsCheckedAsync());
            await checkboxNumbers.SetCheckedAsync(false);

            var checkboxSpecial = Page.Locator("#special");
            await checkboxSpecial.SetCheckedAsync(true);
            Assert.IsTrue(await checkboxSpecial.IsCheckedAsync());
            await checkboxSpecial.SetCheckedAsync(false);

            var buttonGenerate = Page.Locator("button:has-text('Generate')");
            var inputPassword = Page.Locator("#passwordInput");

            await slider.EvaluateAsync("element => element.value = '32'");
            await slider.DispatchEventAsync("input");
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);
            var passwordValue = await inputPassword.InputValueAsync();
            Assert.AreEqual(32, passwordValue.Length);

            await checkboxNumbers.SetCheckedAsync(true);
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Any(char.IsDigit));

            await checkboxSpecial.SetCheckedAsync(true);
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Any(ch => !char.IsLetterOrDigit(ch)));
        }
    }
}
