using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.UriComposerTests;

public class ComposePicUri
{
    [Fact]
    public void ReplacesBaseUrlPlaceholderWithCatalogBaseUrl()
    {
        var catalogSettings = new CatalogSettings
        {
            CatalogBaseUrl = "http://myserver.com"
        };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/images/products/item1.png");

        Assert.Equal("http://myserver.com/images/products/item1.png", result);
    }

    [Fact]
    public void DoesNotModifyUriWithoutPlaceholder()
    {
        var catalogSettings = new CatalogSettings
        {
            CatalogBaseUrl = "http://myserver.com"
        };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://otherserver.com/images/products/item1.png");

        Assert.Equal("http://otherserver.com/images/products/item1.png", result);
    }

    [Fact]
    public void HandlesEmptyString()
    {
        var catalogSettings = new CatalogSettings
        {
            CatalogBaseUrl = "http://myserver.com"
        };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("");

        Assert.Equal("", result);
    }

    [Fact]
    public void ReplacesMultipleOccurrencesOfPlaceholder()
    {
        var catalogSettings = new CatalogSettings
        {
            CatalogBaseUrl = "http://myserver.com"
        };
        var uriComposer = new UriComposer(catalogSettings);

        var result = uriComposer.ComposePicUri("http://catalogbaseurltobereplaced/http://catalogbaseurltobereplaced/image.png");

        Assert.Equal("http://myserver.com/http://myserver.com/image.png", result);
    }
}
