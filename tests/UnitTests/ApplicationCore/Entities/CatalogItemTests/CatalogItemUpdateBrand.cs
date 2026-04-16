using System;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemTests;

public class CatalogItemUpdateBrand
{
    private CatalogItem CreateCatalogItem()
    {
        return new CatalogItem(1, 1, "desc", "name", 10m, "uri");
    }

    [Fact]
    public void UpdatesCatalogBrandId()
    {
        var item = CreateCatalogItem();

        item.UpdateBrand(5);

        Assert.Equal(5, item.CatalogBrandId);
    }

    [Fact]
    public void ThrowsWhenBrandIdIsZero()
    {
        var item = CreateCatalogItem();

        Assert.Throws<ArgumentException>(() => item.UpdateBrand(0));
    }
}
