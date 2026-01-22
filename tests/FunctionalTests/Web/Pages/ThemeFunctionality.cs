using Microsoft.eShopWeb.FunctionalTests.Web;
using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.WebRazorPages;

[Collection("Sequential")]
public class ThemeFunctionality : IClassFixture<TestApplication>
{
    public ThemeFunctionality(TestApplication factory)
    {
        Client = factory.CreateClient();
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task HomePageIncludesThemeSystemFiles()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that theme CSS is loaded
        Assert.Contains("theme-variables.css", stringResponse);
        
        // Assert - Check that theme toggle CSS is loaded
        Assert.Contains("theme-toggle.css", stringResponse);
        
        // Assert - Check that theme.js is loaded
        Assert.Contains("theme.js", stringResponse);
    }

    [Fact]
    public async Task HomePageIncludesThemeToggleButton()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that theme toggle button exists
        Assert.Contains("class=\"theme-toggle\"", stringResponse);
        Assert.Contains("theme-icon", stringResponse);
        Assert.Contains("theme-label", stringResponse);
    }

    [Fact]
    public async Task ThemeToggleButtonHasCorrectAttributes()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert - Check that theme toggle button has correct onclick handler
        Assert.Contains("window.themeManager.cycle()", stringResponse);
        
        // Assert - Check that theme toggle has aria-label for accessibility
        Assert.Contains("aria-label=\"Toggle theme\"", stringResponse);
    }

    [Fact]
    public async Task ThemeCssVariablesFileIsAccessible()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/css/theme-variables.css");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that CSS variables are defined
        Assert.Contains("--color-brand", content);
        Assert.Contains("--color-background-primary", content);
        Assert.Contains("--color-text-primary", content);
        Assert.Contains("[data-theme=\"dark\"]", content);
    }

    [Fact]
    public async Task ThemeToggleCssFileIsAccessible()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/css/theme-toggle.css");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that theme toggle styles are defined
        Assert.Contains(".theme-toggle", content);
        Assert.Contains(".theme-icon", content);
        Assert.Contains(".theme-label", content);
    }

    [Fact]
    public async Task ThemeJavaScriptFileIsAccessible()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/js/theme.js");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that theme JavaScript includes the theme manager
        Assert.Contains("themeManager", content);
        Assert.Contains("THEME_LIGHT", content);
        Assert.Contains("THEME_DARK", content);
        Assert.Contains("THEME_AUTO", content);
        Assert.Contains("prefers-color-scheme", content);
        Assert.Contains("localStorage", content);
    }
}
