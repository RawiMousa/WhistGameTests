using OpenQA.Selenium;
using WhistGameTests.Helpers;

namespace WhistGameTests.GameMethods
{
    internal class Game
    {
        public static void Deal(IWebDriver player) => HelperFunctions.ClickButton(player, "//div[@class='deal']", 5, "XPath");

        public static int     PlayedCards      { get; set; } = 0;
        public static string? CurrentTrickSuit { get; set; } = "";

        public static void PlayCard(IWebDriver[] players)
        {
            if (PlayedCards < 4)
            {
                foreach (var player in players)
                {
                    if (HelperFunctions.TryFindElement(player, "//span[@class='turn']", 6, "XPath") != null)
                    {
                        if (PlayedCards == 0)
                        {
                            var card = HelperFunctions.ClickButtonJS(player, "//*[@chair='bottom']//div[@class='card']", 10, "XPath");
                            CurrentTrickSuit = card?.GetAttribute("suit");
                        }
                        else
                        {
                            IWebElement? card = HelperFunctions.TryFindElement(player,
                                $"//*[@chair='bottom']//div[@class='card'][@suit='{CurrentTrickSuit}']", 7, "XPath");

                            if (card == null)
                            {
                                card = HelperFunctions.TryFindElement(player,
                                    "//*[@chair='bottom']//div[@class='card']", 7, "XPath");
                            }
                            HelperFunctions.ClickFoundButtonJS(player, card);

                        }

                    }
                }
                PlayedCards++;
            }
            else
            {
                PlayedCards      = 0;
                CurrentTrickSuit = "";
            }

            Console.WriteLine("Played cards = " + PlayedCards);
        }

        public static void PlayATrick(IWebDriver[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                PlayCard(players);
            }
        }

        public static void PlayRound(IWebDriver[] players)
        {
            for (int i = 0; i < 13; i++)
            {
                PlayATrick(players);
            }
        }
    }
}
