using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumTests.Infrastructure;

public class SeleniumTestBase : IDisposable
{
    protected IWebDriver Driver { get; private set; }
    protected WebDriverWait Wait { get; private set; }

    public SeleniumTestBase()
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments(
            "--headless",              // Run in headless mode for CI/CD
            "--no-sandbox",            // Required for running in containers
            "--disable-dev-shm-usage", // Overcome limited resource problems
            "--disable-gpu",           // Disable GPU hardware acceleration
            "--window-size=1920,1080"  // Set window size
        );

        Driver = new ChromeDriver(chromeOptions);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
    }

    protected void NavigateTo(string url)
    {
        Driver.Navigate().GoToUrl(url);
    }

    protected IWebElement FindElement(By locator)
    {
        return Wait.Until(driver => driver.FindElement(locator));
    }

    protected IWebElement FindElement(By locator, int timeoutInSeconds)
    {
        var customWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
        return customWait.Until(driver => driver.FindElement(locator));
    }

    protected bool IsElementPresent(By locator)
    {
        try
        {
            Driver.FindElement(locator);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    protected void WaitForElementToBeVisible(By locator)
    {
        Wait.Until(driver => driver.FindElement(locator).Displayed);
    }

    protected void WaitForElementToBeClickable(By locator)
    {
        Wait.Until(driver => driver.FindElement(locator).Enabled);
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}