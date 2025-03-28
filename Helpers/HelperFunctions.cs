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
            Console.WriteLine($"Displayed: {readyButton.Displayed}, Enabled: {readyButton.Enabled}, Size: {readyButton.Size}");
            readyButton.Click();
        }

        public static IWebElement? ClickButtonJS(IWebDriver player, string button, int time = 3, string type = "id")
        {
            var readyButton = TryFindElement(player, button, time, type);
            if (readyButton != null) ((IJavaScriptExecutor)player).ExecuteScript("arguments[0].click();", readyButton);
            return readyButton;
        }

        public static void ClickFoundButtonJS(IWebDriver player, IWebElement? button)
        {
            if (button != null) ((IJavaScriptExecutor)player).ExecuteScript("arguments[0].click();", button);
        }

        public static IWebElement? TryFindElement(IWebDriver driver, string button, int timeoutSeconds = 3 , string type = "id")
        {
            try
            {
                By locator = type == "id" ? By.Id(button) : By.XPath(button);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(drv => drv.FindElement(locator));
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }


        public static void LoadBrowser(IWebDriver player)
        {
            player.Navigate().GoToUrl("http://localhost:5190");
            Assert.Contains("WhistGame", player.Title);
        }

        public static void ArrangeBrowsers(IWebDriver[] players) // Since it is a multiplayer whist game i'd have to split each screen to 2 players(2 browsers)
        {
            if (players.Length < 4) return;

            int screenWidth  = 1920;
            int screenHeight = 1080;

            int width  = screenWidth;
            int height = screenHeight;

            // Player1
            players[0].Manage().Window.Size     = new System.Drawing.Size(width / 2, height);
            players[0].Manage().Window.Position = new System.Drawing.Point(0, 0);
            // Player2
            players[1].Manage().Window.Size     = new System.Drawing.Size(width / 2, height);
            players[1].Manage().Window.Position = new System.Drawing.Point(width / 2, 0);

            int secondScreenX = screenWidth;
            // Player3
            players[2].Manage().Window.Size     = new System.Drawing.Size(width / 2, height);
            players[2].Manage().Window.Position = new System.Drawing.Point(secondScreenX, 0);
            // Player4
            players[3].Manage().Window.Size     = new System.Drawing.Size(width / 2, height);
            players[3].Manage().Window.Position = new System.Drawing.Point(secondScreenX + (width / 2), 0);
        }
    }
}
