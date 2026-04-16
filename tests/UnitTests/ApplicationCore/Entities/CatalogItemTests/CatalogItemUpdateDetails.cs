using System;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;
using static Microsoft.eShopWeb.ApplicationCore.Entities.CatalogItem;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemTests;

public class CatalogItemUpdateDetails
{
    private CatalogItem CreateCatalogItem()
    {
        return new CatalogItem(1, 1, "desc", "name", 10m, "uri");
    }

    [Fact]
    public void UpdatesNameDescriptionAndPrice()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails("New Name", "New Description", 25m);

        item.UpdateDetails(details);

        Assert.Equal("New Name", item.Name);
        Assert.Equal("New Description", item.Description);
        Assert.Equal(25m, item.Price);
    }

    [Fact]
    public void ThrowsWhenNameIsNull()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails(null, "desc", 10m);

        Assert.Throws<ArgumentNullException>(() => item.UpdateDetails(details));
    }

    [Fact]
    public void ThrowsWhenNameIsEmpty()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails(string.Empty, "desc", 10m);

        Assert.Throws<ArgumentException>(() => item.UpdateDetails(details));
    }

    [Fact]
    public void ThrowsWhenDescriptionIsNull()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails("name", null, 10m);

        Assert.Throws<ArgumentNullException>(() => item.UpdateDetails(details));
    }

    [Fact]
    public void ThrowsWhenDescriptionIsEmpty()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails("name", string.Empty, 10m);

        Assert.Throws<ArgumentException>(() => item.UpdateDetails(details));
    }

    [Fact]
    public void ThrowsWhenPriceIsZero()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails("name", "desc", 0m);

        Assert.Throws<ArgumentException>(() => item.UpdateDetails(details));
    }

    [Fact]
    public void ThrowsWhenPriceIsNegative()
    {
        var item = CreateCatalogItem();
        var details = new CatalogItemDetails("name", "desc", -5m);

        Assert.Throws<ArgumentException>(() => item.UpdateDetails(details));
    }
}
