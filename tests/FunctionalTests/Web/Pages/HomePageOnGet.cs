using Microsoft.eShopWeb.FunctionalTests.Web;
using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.WebRazorPages;

[Collection("Sequential")]
public class HomePageOnGet : IClassFixture<TestApplication>
{
    public HomePageOnGet(TestApplication factory)
    {
        Client = factory.CreateClient();
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task ReturnsHomePageWithProductListing()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(".NET Bot Black Sweatshirt", stringResponse);
    }

    [Fact]
    public async Task IncludesThemeControlsBeforeRenderingStyles()
    {
        var response = await Client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("DAMIANO's BEST eSHOP", html);
        Assert.Contains("data-theme-toggle", html);
        Assert.Contains("css/theme.css", html);
        Assert.True(html.IndexOf("js/theme.js", StringComparison.Ordinal) <
                    html.IndexOf("css/app.css", StringComparison.Ordinal));
    }
}
