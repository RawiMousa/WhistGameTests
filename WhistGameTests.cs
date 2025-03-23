using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WhistGameTests.Helpers;

namespace WhistGameTests
{
    public class WhistGameTest : IClassFixture<WebDriverFixture>
    {
        private readonly IWebDriver Player1;
        private readonly IWebDriver Player2;
        private readonly IWebDriver Player3;
        private readonly IWebDriver Player4;

        private string password = "blowmehackers";

        // Constructor receives the WebDriverFixture instance
        public WhistGameTest(WebDriverFixture fixture)
        {
            Player1 = fixture.ChromeDriver1;
            Player2 = fixture.ChromeDriver2;
            Player3 = fixture.ChromeDriver3;
            Player4 = fixture.ChromeDriver4;
            // Assume two screens of width 1920px, each with a height of 1080px
            int screenWidth = 1920; // Width of one screen
            int screenHeight = 1080; // Height of one screen
            // Each player's browser will take half the screen width, but full screen height
            int width = screenWidth; // Full screen width for each player (without dividing)
            int height = screenHeight;   // Full height for each player
            // Positioning Players on the First Screen (Screen 1)
            Player1.Manage().Window.Size = new System.Drawing.Size(width / 2, height);
            Player1.Manage().Window.Position = new System.Drawing.Point(0, 0);  // Player1: Left half of Screen 1
            Player2.Manage().Window.Size = new System.Drawing.Size(width / 2, height);
            Player2.Manage().Window.Position = new System.Drawing.Point(width / 2, 0);  // Player2: Right half of Screen 1
            // Positioning Players on the Second Screen (Screen 2)
            int secondScreenX = screenWidth; // Position for the second screen starts at 1920px

            Player3.Manage().Window.Size = new System.Drawing.Size(width / 2, height);
            Player3.Manage().Window.Position = new System.Drawing.Point(secondScreenX, 0);  // Player3: Left half of Screen 2

            Player4.Manage().Window.Size = new System.Drawing.Size(width / 2, height);
            Player4.Manage().Window.Position = new System.Drawing.Point(secondScreenX + (width / 2), 0);  // Player4: Right half of Screen 2
        }

        public void LoadBrowser(IWebDriver player)
        {
            player.Navigate().GoToUrl("http://localhost:5190");
            Assert.Contains("WhistGame", player.Title);
        }

        public void GoogleLoginFunc(IWebDriver player, string email, string password)
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

        public string CreateInviteSession(IWebDriver player)
        {
            var multiplayerButton = player.FindElement(By.Id("multiplayer"));
            multiplayerButton.Click();
            return player.Url;
        }

        public void GoToInviteSession(IWebDriver[] players, string url)
        {
            foreach (var player in players)
            {
                player.Navigate().GoToUrl(url);
            }
        }

        public void ClickReady(IWebDriver[] players)
        {
            foreach (var player in players)
            {
                HelperFunctions.WaitForButton(player, "ready", 2);
                var readyButton = player.FindElement(By.Id("ready"));
                readyButton.Click();
                Thread.Sleep(200); // This is to delay the clicks , because each click updates the DOM , and we would probably get a StaleElementReferenceException
            }
        }

        [Fact]
        public void CompleteFlow()
        {
            LoadBrowser(Player1);
            LoadBrowser(Player2);
            LoadBrowser(Player3);
            LoadBrowser(Player4);

            HelperFunctions.WaitForButton(Player1, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player2, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player3, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player4, "googlelogin", 5);

            GoogleLoginFunc(Player1, "rawiwhistgame@gmail.com", password); // This is the host (doesn't have to be him but for the sake of testing)
            HelperFunctions.WaitForButton(Player1, "multiplayer", 3);
            var inviteUrl = CreateInviteSession(Player1); // The host creates an invite session 

            GoogleLoginFunc(Player2, "rawi.mossa@gmail.com", password);
            GoogleLoginFunc(Player3, "sides.music@gmail.com", password);
            GoogleLoginFunc(Player4, "rawisamimousa@gmail.com", password);

            IWebDriver[] players = { Player2, Player3, Player4 };
            GoToInviteSession(players, inviteUrl); // All other players but the host are redirected to the invite url

            IWebDriver[] clickPlayers = { Player1, Player2, Player3, Player4 };
            ClickReady(clickPlayers); // Ready to play the game

            HelperFunctions.WaitForButton(Player1, "//span[text()='Start Game']", 5, "XPath");
            var startGame = Player1.FindElement(By.XPath("//span[text()='Start Game']"));
            startGame.Click();
        }

    }
}
