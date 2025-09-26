using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumUITests.PageObjects;

public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Locators
    private readonly By _emailField = By.Id("Input_Email");
    private readonly By _passwordField = By.Id("Input_Password");
    private readonly By _loginButton = By.CssSelector("input[type='submit'][value='Log in']");
    private readonly By _rememberMeCheckbox = By.Id("Input_RememberMe");
    private readonly By _validationSummary = By.CssSelector(".validation-summary-errors");
    private readonly By _pageTitle = By.TagName("h2");

    public LoginPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void NavigateTo(string baseUrl)
    {
        _driver.Navigate().GoToUrl($"{baseUrl}/Identity/Account/Login");
    }

    public bool IsLoaded()
    {
        try
        {
            _wait.Until(driver => driver.FindElement(_emailField));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public void EnterEmail(string email)
    {
        var emailField = _wait.Until(driver =>
        {
            var element = driver.FindElement(_emailField);
            return element.Displayed ? element : null;
        });
        emailField.Clear();
        emailField.SendKeys(email);
    }

    public void EnterPassword(string password)
    {
        var passwordField = _driver.FindElement(_passwordField);
        passwordField.Clear();
        passwordField.SendKeys(password);
    }

    public void SetRememberMe(bool remember)
    {
        var checkbox = _driver.FindElement(_rememberMeCheckbox);
        if (checkbox.Selected != remember)
        {
            checkbox.Click();
        }
    }

    public void ClickLoginButton()
    {
        var loginButton = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_loginButton);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        loginButton.Click();
    }

    public void LoginUser(string email, string password, bool rememberMe = false)
    {
        EnterEmail(email);
        EnterPassword(password);
        SetRememberMe(rememberMe);
        ClickLoginButton();
    }

    public bool HasValidationErrors()
    {
        try
        {
            return _driver.FindElement(_validationSummary).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public string GetValidationErrorText()
    {
        try
        {
            var validationDiv = _driver.FindElement(_validationSummary);
            return validationDiv.Text;
        }
        catch (NoSuchElementException)
        {
            return string.Empty;
        }
    }

    public bool IsLoginSuccessful()
    {
        try
        {
            // Wait for redirect after successful login
            _wait.Until(driver => !driver.Url.Contains("/Account/Login"));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}