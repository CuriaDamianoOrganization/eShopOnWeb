using Microsoft.eShopWeb.ApplicationCore.Services;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.UriComposerTests;

public class ComposePicUri
{
    [Fact]
    public void ReplacesPlaceholderWithCatalogBaseUrl()
    {
        var settings = new CatalogSettings { CatalogBaseUrl = "https://mycdn.example.com" };
        var uriComposer = new UriComposer(settings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/images/product1.png");

        Assert.Equal("https://mycdn.example.com/images/product1.png", result);
    }

    [Fact]
    public void ReturnsOriginalStringIfNoPlaceholderFound()
    {
        var settings = new CatalogSettings { CatalogBaseUrl = "https://mycdn.example.com" };
        var uriComposer = new UriComposer(settings);

        var result = uriComposer.ComposePicUri("https://other.example.com/images/product1.png");

        Assert.Equal("https://other.example.com/images/product1.png", result);
    }

    [Fact]
    public void HandlesEmptyCatalogBaseUrl()
    {
        var settings = new CatalogSettings { CatalogBaseUrl = "" };
        var uriComposer = new UriComposer(settings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/images/product1.png");

        Assert.Equal("/images/product1.png", result);
    }
}
