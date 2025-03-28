using OpenQA.Selenium;
using WhistGameTests.Helpers;
using WhistGameTests.LoginAndPrepareGame;
using WhistGameTests.GameMethods;

namespace WhistGameTests
{
    [Collection("FixtureCollection")]
    public class WhistGameTests
    {
        private readonly IWebDriver Player1;
        private readonly IWebDriver Player2;
        private readonly IWebDriver Player3;
        private readonly IWebDriver Player4;

        private readonly WebDriverFixture _fixture;

        private string password = "blowmehackers";

        // Constructor receives the WebDriverFixture instance
        public WhistGameTests(WebDriverFixture fixture)
        {
            Player1 = fixture.ChromeDriver1;
            Player2 = fixture.ChromeDriver2;
            Player3 = fixture.ChromeDriver3;
            Player4 = fixture.ChromeDriver4;
            _fixture = fixture;
            HelperFunctions.ArrangeBrowsers([Player1, Player2, Player3, Player4]);
        }


        [Fact]
        public void CompleteFlow()
        {
            HelperFunctions.LoadBrowser(Player1);
            HelperFunctions.LoadBrowser(Player2);
            HelperFunctions.LoadBrowser(Player3);
            HelperFunctions.LoadBrowser(Player4);

            HelperFunctions.WaitForButton(Player1, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player2, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player3, "googlelogin", 5);
            HelperFunctions.WaitForButton(Player4, "googlelogin", 5);
            
            LoginAndPrepare.GoogleLoginFunc(Player1, "rawiwhistgame@gmail.com", password); // This is the host (doesn't have to be him but for the sake of testing)

            HelperFunctions.WaitForButton(Player1, "multiplayer", 3);
            var inviteUrl = LoginAndPrepare.CreateInviteSession(Player1); // The host creates an invite session 

            LoginAndPrepare.GoogleLoginFunc(Player2, "rawi.mossa@gmail.com", password);
            LoginAndPrepare.GoogleLoginFunc(Player3, "sides.music@gmail.com", password);
            LoginAndPrepare.GoogleLoginFunc(Player4, "rawisamimousa@gmail.com", password);

            IWebDriver[] players = { Player2, Player3, Player4 };
            LoginAndPrepare.GoToInviteSession(players, inviteUrl); // All other players but the host are redirected to the invite url

            IWebDriver[] clickPlayers = { Player1, Player2, Player3, Player4 };
            LoginAndPrepare.ClickReady(clickPlayers); // Ready to play the game
            HelperFunctions.ClickButton(Player1, "//span[text()='Start Game']", 5, "XPath");

            _fixture.GameReady = true;
            Game.Deal(Player1);
            Game.PlayRound([Player1, Player2, Player3, Player4]);



        }

    }
}
