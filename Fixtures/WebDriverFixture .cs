using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class WebDriverFixture : IDisposable
{
    public IWebDriver ChromeDriver1 { get; private set; }
    public IWebDriver ChromeDriver2 { get; private set; }
    public IWebDriver ChromeDriver3 { get; private set; }
    public IWebDriver ChromeDriver4 { get; private set; }
    public bool       GameReady     { get; set; }

    public WebDriverFixture()
    {
        ChromeDriver1 = CreateUndetectableChromeDriver("Player1");
        ChromeDriver2 = CreateUndetectableChromeDriver("Player2");
        ChromeDriver3 = CreateUndetectableChromeDriver("Player3");
        ChromeDriver4 = CreateUndetectableChromeDriver("Player4");
    }

    private IWebDriver CreateUndetectableChromeDriver(string playerName)
    {
        var options = new ChromeOptions();

        string profilePath = $@"C:\Selenium\ChromeProfiles\{playerName}";
        options.AddArgument($"--user-data-dir={profilePath}");
        options.AddArgument("--profile-directory=Default");


        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--disable-blink-features=AutomationControlled");

        var driver = new ChromeDriver(options);

        var script = @"
            Object.defineProperty(navigator, 'webdriver', {
                get: () => undefined
            });
        ";

        ((IJavaScriptExecutor)driver).ExecuteScript(
            "window.open('','_blank');"
        );
        driver.SwitchTo().Window(driver.WindowHandles[1]);
        ((IJavaScriptExecutor)driver).ExecuteScript(script);
        driver.Close();
        driver.SwitchTo().Window(driver.WindowHandles[0]);

        return driver;
    }

    public void Dispose()
    {
        ChromeDriver1?.Quit();
        ChromeDriver2?.Quit();
        ChromeDriver3?.Quit();
        ChromeDriver4?.Quit();
    }
}
