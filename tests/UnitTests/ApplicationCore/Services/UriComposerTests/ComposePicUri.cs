using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.UriComposerTests;

public class ComposePicUri
{
    [Fact]
    public void ReturnsUriWithReplacedBaseUrl()
    {
        var catalogSettings = new CatalogSettings { CatalogBaseUrl = "https://example.com" };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/pics/1.png");

        Assert.Equal("https://example.com/pics/1.png", result);
    }

    [Fact]
    public void ReturnsOriginalUriWhenNoPlaceholderExists()
    {
        var catalogSettings = new CatalogSettings { CatalogBaseUrl = "https://example.com" };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("https://someother.com/pics/1.png");

        Assert.Equal("https://someother.com/pics/1.png", result);
    }

    [Fact]
    public void HandlesEmptyUriTemplate()
    {
        var catalogSettings = new CatalogSettings { CatalogBaseUrl = "https://example.com" };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("");

        Assert.Equal("", result);
    }

    [Fact]
    public void HandlesCatalogBaseUrlWithTrailingSlash()
    {
        var catalogSettings = new CatalogSettings { CatalogBaseUrl = "https://example.com/" };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/pics/1.png");

        Assert.Equal("https://example.com//pics/1.png", result);
    }

    [Fact]
    public void ReplacesMultiplePlaceholders()
    {
        var catalogSettings = new CatalogSettings { CatalogBaseUrl = "https://example.com" };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/pics/http://catalogbaseurltobereplaced/1.png");

        Assert.Equal("https://example.com/pics/https://example.com/1.png", result);
    }
}
