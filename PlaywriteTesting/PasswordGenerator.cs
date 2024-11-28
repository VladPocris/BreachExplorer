using Microsoft.Playwright;

namespace PlaywriteTesting
{
    [TestClass]
    public class PasswordGenerator: PageTest
    {
        [TestMethod]
        public async Task PasswordGeneratorIsFunctional()
        {
            await Page.GotoAsync("https://gcstorageacc2.z9.web.core.windows.net/");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Test Slider Value Change And Tooltip
            var slider = Page.Locator("input#customRange3");
            await slider.EvaluateAsync("element => element.value = '32'");
            await slider.DispatchEventAsync("input");
            var tooltip = Page.Locator("span.tooltip");
            var tooltipText = await tooltip.TextContentAsync();
            Assert.AreEqual("32", tooltipText);
            await slider.EvaluateAsync("element => element.value = '16'");

            //Test Include Number Checkbox
            var checkbox1 = Page.Locator("input#inlineCheckbox1");
            await checkbox1.SetCheckedAsync(true);
            Assert.IsTrue(await checkbox1.IsCheckedAsync());
            await checkbox1.SetCheckedAsync(false);

            //Test Include Special Characters Checkbox
            var checkbox2 = Page.Locator("input#inlineCheckbox2");
            await checkbox2.SetCheckedAsync(true);
            Assert.IsTrue(await checkbox2.IsCheckedAsync());
            await checkbox2.SetCheckedAsync(false);

            var buttonGenerate = Page.Locator("button.btn.btn-primary:has-text('Generate')");
            var inputPassword = Page.Locator("input.form-control.mb-2#passwordInput");
            //Test If Password Uses Slider Value
            await slider.EvaluateAsync("element => element.value = '32'");
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
            var passwordValue = await inputPassword.InputValueAsync();
            Assert.AreEqual(32, passwordValue.Length);
            await slider.EvaluateAsync("element => element.value = '16'");

            //Test If Password Uses Number Checkbox Value
            await checkbox1.SetCheckedAsync(true);
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Any(char.IsDigit));
            await checkbox1.SetCheckedAsync(false);

            //Test If Password Uses Special Characters Checkbox Value
            await checkbox2.SetCheckedAsync(true);
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Any(ch => !char.IsLetterOrDigit(ch)));

            //Test If Password Uses Special Characters and Number Checkbox Value
            await checkbox1.SetCheckedAsync(true);
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Any(char.IsDigit) && passwordValue.Any(ch => !char.IsLetterOrDigit(ch)));

            //Test If Password Uses Special Characters, Number Checkbox Value and Slider Value
            await slider.EvaluateAsync("element => element.value = '32'");
            await buttonGenerate.ClickAsync();
            await Page.WaitForTimeoutAsync(1500);
            passwordValue = await inputPassword.InputValueAsync();
            Assert.IsTrue(passwordValue.Length == 32 && passwordValue.Any(char.IsDigit) && passwordValue.Any(ch => !char.IsLetterOrDigit(ch)));

        }
    }
}
