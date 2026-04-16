using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemOrderedTests;

public class CatalogItemOrderedConstructor
{
    private readonly int _testCatalogItemId = 1;
    private readonly string _testProductName = "Test Product";
    private readonly string _testPictureUri = "http://test.com/image.jpg";

    [Fact]
    public void SetsProperties()
    {
        var item = new CatalogItemOrdered(_testCatalogItemId, _testProductName, _testPictureUri);

        Assert.Equal(_testCatalogItemId, item.CatalogItemId);
        Assert.Equal(_testProductName, item.ProductName);
        Assert.Equal(_testPictureUri, item.PictureUri);
    }

    [Fact]
    public void ThrowsWhenCatalogItemIdIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CatalogItemOrdered(0, _testProductName, _testPictureUri));
    }

    [Fact]
    public void ThrowsWhenProductNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CatalogItemOrdered(_testCatalogItemId, null!, _testPictureUri));
    }

    [Fact]
    public void ThrowsWhenProductNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new CatalogItemOrdered(_testCatalogItemId, string.Empty, _testPictureUri));
    }

    [Fact]
    public void ThrowsWhenPictureUriIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CatalogItemOrdered(_testCatalogItemId, _testProductName, null!));
    }

    [Fact]
    public void ThrowsWhenPictureUriIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new CatalogItemOrdered(_testCatalogItemId, _testProductName, string.Empty));
    }
}
