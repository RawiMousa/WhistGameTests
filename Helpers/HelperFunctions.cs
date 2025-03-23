using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WhistGameTests.Helpers
{
    internal class HelperFunctions
    {
        public static By WaitForButton(IWebDriver player, string button, int time = 3 , string type = "id")
        {
            By            locator = type == "id" ? By.Id(button) : By.XPath(button);
            WebDriverWait wait    = new WebDriverWait(player, TimeSpan.FromSeconds(time));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
            return locator;
        }


        public static void ClickButton(IWebDriver player, string button, int time = 3, string type = "id")
        {
            var locator = WaitForButton(player, button, time , type);
            var readyButton = player.FindElement(locator);
            readyButton.Click();
        }
    }
}
