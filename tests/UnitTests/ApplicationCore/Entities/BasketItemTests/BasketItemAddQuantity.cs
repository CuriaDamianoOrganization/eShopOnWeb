using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketItemTests;

public class BasketItemAddQuantity
{
    private readonly int _testCatalogItemId = 123;
    private readonly decimal _testUnitPrice = 1.23m;
    private readonly int _initialQuantity = 2;

    private BasketItem CreateBasketItem()
    {
        return new BasketItem(_testCatalogItemId, _initialQuantity, _testUnitPrice);
    }

    [Fact]
    public void IncreasesQuantityByGivenAmount()
    {
        var item = CreateBasketItem();

        item.AddQuantity(3);

        Assert.Equal(_initialQuantity + 3, item.Quantity);
    }

    [Fact]
    public void CanAddZeroQuantity()
    {
        var item = CreateBasketItem();

        item.AddQuantity(0);

        Assert.Equal(_initialQuantity, item.Quantity);
    }

    [Fact]
    public void ThrowsGivenNegativeQuantity()
    {
        var item = CreateBasketItem();

        Assert.Throws<ArgumentOutOfRangeException>(() => item.AddQuantity(-1));
    }
}
