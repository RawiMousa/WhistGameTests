using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using WhistGameTests.Helpers;

namespace WhistGameTests.LoginAndPrepareGame
{
    internal class LoginAndPrepare
    {
        public static void GoogleLoginFunc(IWebDriver player, string email, string password)
        {
            var googleButton = player.FindElement(By.Id("googlelogin"));
            googleButton.Click();
            WebDriverWait wait = new WebDriverWait(player, TimeSpan.FromSeconds(3));

            try
            {
                // Try to find the email input field — if it's visible, you're not logged in yet
                var emailInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[@type='email']")));
                emailInput.SendKeys(email);

                var nextButton1 = player.FindElement(By.XPath("//span[text()='Next']"));
                nextButton1.Click();

                var passwordInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[@type='password']")));
                passwordInput.SendKeys(password);

                var nextButton2 = player.FindElement(By.XPath("//span[text()='Next']"));
                nextButton2.Click();

                var continueButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.XPath("//span[text()='Continue']")));
                continueButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Login skipped — already signed in.");
            }
        }

        public static string CreateInviteSession(IWebDriver player)
        {
            var multiplayerButton = player.FindElement(By.Id("multiplayer"));
            multiplayerButton.Click();
            return player.Url;
        }

        public static void GoToInviteSession(IWebDriver[] players, string url)
        {
            foreach (var player in players)
            {
                player.Navigate().GoToUrl(url);
            }
        }

        public static void ClickReady(IWebDriver[] players)
        {
            foreach (var player in players)
            {
                HelperFunctions.WaitForButton(player, "ready", 2);
                var readyButton = player.FindElement(By.Id("ready"));
                readyButton.Click();
                Thread.Sleep(200); // This is to delay the clicks , because each click updates the DOM , and we would probably get a StaleElementReferenceException
            }
        }
    }

}
