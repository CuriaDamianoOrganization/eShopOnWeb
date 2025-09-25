using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumTests.PageObjects;

public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    // Page elements
    private By EmailInput => By.CssSelector("input[name='email'], input[name='Email']");
    private By PasswordInput => By.CssSelector("input[name='password'], input[name='Password']");
    private By LoginButton => By.CssSelector("button[type='submit'], input[value='Log in']");
    private By RememberMeCheckbox => By.CssSelector("input[name='RememberMe']");
    private By ErrorMessage => By.CssSelector(".text-danger, .alert-danger");
    private By LoginForm => By.CssSelector("form");

    // Navigation methods
    public void Navigate(string baseUrl)
    {
        _driver.Navigate().GoToUrl($"{baseUrl}/Identity/Account/Login");
    }

    // Verification methods
    public bool IsLoginFormVisible()
    {
        try
        {
            _wait.Until(driver => driver.FindElement(LoginForm));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool HasErrorMessage()
    {
        return _driver.FindElements(ErrorMessage).Count > 0;
    }

    public string GetErrorMessage()
    {
        var errorElements = _driver.FindElements(ErrorMessage);
        return errorElements.Count > 0 ? errorElements[0].Text : string.Empty;
    }

    // Interaction methods
    public void EnterEmail(string email)
    {
        var emailElement = _wait.Until(driver => driver.FindElement(EmailInput));
        emailElement.Clear();
        emailElement.SendKeys(email);
    }

    public void EnterPassword(string password)
    {
        var passwordElement = _wait.Until(driver => driver.FindElement(PasswordInput));
        passwordElement.Clear();
        passwordElement.SendKeys(password);
    }

    public void ClickRememberMe()
    {
        var rememberMeElement = _driver.FindElement(RememberMeCheckbox);
        if (!rememberMeElement.Selected)
        {
            rememberMeElement.Click();
        }
    }

    public void ClickLoginButton()
    {
        var loginButtonElement = _wait.Until(driver => driver.FindElement(LoginButton));
        loginButtonElement.Click();
    }

    public void Login(string email, string password)
    {
        EnterEmail(email);
        EnterPassword(password);
        ClickLoginButton();
    }

    public string GetCurrentUrl()
    {
        return _driver.Url;
    }

    public string GetPageTitle()
    {
        return _driver.Title;
    }
}