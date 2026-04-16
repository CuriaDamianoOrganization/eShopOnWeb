using System;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemTests;

public class CatalogItemUpdateType
{
    private CatalogItem CreateCatalogItem()
    {
        return new CatalogItem(1, 1, "desc", "name", 10m, "uri");
    }

    [Fact]
    public void UpdatesCatalogTypeId()
    {
        var item = CreateCatalogItem();

        item.UpdateType(5);

        Assert.Equal(5, item.CatalogTypeId);
    }

    [Fact]
    public void ThrowsWhenTypeIdIsZero()
    {
        var item = CreateCatalogItem();

        Assert.Throws<ArgumentException>(() => item.UpdateType(0));
    }
}
