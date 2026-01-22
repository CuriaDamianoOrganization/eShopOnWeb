using Microsoft.eShopWeb.FunctionalTests.Web;
using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.WebRazorPages;

[Collection("Sequential")]
public class ThemeTest : IClassFixture<TestApplication>
{
    public ThemeTest(TestApplication factory)
    {
        Client = factory.CreateClient();
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task HomePageContainsThemeToggleButton()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that theme toggle button exists
        Assert.Contains("theme-toggle", stringResponse);
        Assert.Contains("🎨", stringResponse);
    }

    [Fact]
    public async Task HomePageContainsThemeCss()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that theme CSS file is included
        Assert.Contains("theme.css", stringResponse);
    }

    [Fact]
    public async Task HomePageContainsThemeJavaScript()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that site.js is included (contains theme logic)
        Assert.Contains("site.js", stringResponse);
    }

    [Fact]
    public async Task ThemeCssFileIsAccessible()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/css/theme.css");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("data-theme", stringResponse);
        Assert.Contains("--color-background-brighter", stringResponse);
    }

    [Fact]
    public async Task SiteJsContainsThemeLogic()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/js/site.js");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("getTheme", stringResponse);
        Assert.Contains("setTheme", stringResponse);
        Assert.Contains("data-theme", stringResponse);
    }
}
