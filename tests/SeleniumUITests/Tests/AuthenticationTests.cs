using Microsoft.eShopWeb.SeleniumUITests.PageObjects;

namespace Microsoft.eShopWeb.SeleniumUITests.Tests;

public class AuthenticationTests : BaseSeleniumTest
{
    private readonly string _testEmail = "demouser@microsoft.com";
    private readonly string _testPassword = "Pass@word1";

    [Fact]
    public void LoginPage_ShouldLoad_Successfully()
    {
        // Arrange
        var loginPage = new LoginPage(Driver, Wait);

        // Act
        loginPage.NavigateTo(BaseUrl);

        // Assert
        Assert.True(loginPage.IsLoaded(), "Login page should load successfully");
    }

    [Fact]
    public void LoginPage_WithValidCredentials_ShouldLoginSuccessfully()
    {
        // Arrange
        var loginPage = new LoginPage(Driver, Wait);
        var homePage = new HomePage(Driver, Wait);

        // Act
        loginPage.NavigateTo(BaseUrl);
        loginPage.LoginUser(_testEmail, _testPassword);

        // Wait for redirect
        Thread.Sleep(3000);

        // Assert
        Assert.True(loginPage.IsLoginSuccessful(), "Login should be successful");
        
        // Navigate back to home to verify login state
        NavigateToUrl();
        Assert.True(homePage.IsUserLoggedIn(), "User should be logged in on home page");
    }

    [Fact]
    public void LoginPage_WithInvalidCredentials_ShouldShowError()
    {
        // Arrange
        var loginPage = new LoginPage(Driver, Wait);

        // Act
        loginPage.NavigateTo(BaseUrl);
        loginPage.LoginUser("invalid@email.com", "wrongpassword");

        // Wait for validation
        Thread.Sleep(2000);

        // Assert
        Assert.False(loginPage.IsLoginSuccessful(), "Login should fail with invalid credentials");
        Assert.Contains("/Account/Login", Driver.Url);
    }

    [Fact]
    public void LoginPage_WithEmptyFields_ShouldStayOnLoginPage()
    {
        // Arrange
        var loginPage = new LoginPage(Driver, Wait);

        // Act
        loginPage.NavigateTo(BaseUrl);
        loginPage.ClickLoginButton(); // Try to login without entering credentials

        // Wait a moment
        Thread.Sleep(1000);

        // Assert
        Assert.Contains("/Account/Login", Driver.Url);
        Assert.False(loginPage.IsLoginSuccessful(), "Should not login with empty fields");
    }

    [Fact]
    public void Navigation_FromHomeToLogin_ShouldWork()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);
        var loginPage = new LoginPage(Driver, Wait);

        // Act
        NavigateToUrl();
        homePage.ClickLoginLink();

        // Assert
        Assert.True(loginPage.IsLoaded(), "Should navigate to login page");
        Assert.Contains("/Account/Login", Driver.Url);
    }

    [Fact]
    public void LoginFlow_WithRememberMe_ShouldWork()
    {
        // Arrange
        var loginPage = new LoginPage(Driver, Wait);

        // Act
        loginPage.NavigateTo(BaseUrl);
        loginPage.EnterEmail(_testEmail);
        loginPage.EnterPassword(_testPassword);
        loginPage.SetRememberMe(true);
        loginPage.ClickLoginButton();

        // Wait for redirect
        Thread.Sleep(3000);

        // Assert
        Assert.True(loginPage.IsLoginSuccessful(), "Login with remember me should be successful");
    }
}