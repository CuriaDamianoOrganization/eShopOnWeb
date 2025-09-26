using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumUITests;

public abstract class BaseSeleniumTest : IDisposable
{
    protected IWebDriver Driver { get; private set; } = null!;
    protected WebDriverWait Wait { get; private set; } = null!;
    protected IConfiguration Configuration { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = null!;

    public BaseSeleniumTest()
    {
        SetupConfiguration();
        SetupWebDriver();
        BaseUrl = Configuration["BaseUrl"] ?? "https://localhost:5001";
    }

    private void SetupConfiguration()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private void SetupWebDriver()
    {
        var options = new ChromeOptions();
        
        // Configure Chrome options
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        
        // Run in headless mode for CI/CD environments
        if (Configuration["Selenium:Headless"] == "true" || 
            Environment.GetEnvironmentVariable("CI") == "true")
        {
            options.AddArgument("--headless");
        }

        Driver = new ChromeDriver(options);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
    }

    protected void NavigateToUrl(string relativeUrl = "")
    {
        var fullUrl = BaseUrl + relativeUrl;
        Driver.Navigate().GoToUrl(fullUrl);
    }

    protected IWebElement WaitForElement(By locator, int timeoutInSeconds = 15)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
        return wait.Until(driver => driver.FindElement(locator));
    }

    protected void WaitForElementToBeClickable(By locator, int timeoutInSeconds = 15)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(locator);
                return element.Displayed && element.Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
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

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}