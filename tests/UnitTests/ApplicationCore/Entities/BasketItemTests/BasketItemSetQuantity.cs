using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketItemTests;

public class BasketItemSetQuantity
{
    private readonly int _testCatalogItemId = 123;
    private readonly decimal _testUnitPrice = 1.23m;
    private readonly int _initialQuantity = 2;

    private BasketItem CreateBasketItem()
    {
        return new BasketItem(_testCatalogItemId, _initialQuantity, _testUnitPrice);
    }

    [Fact]
    public void SetsQuantityToGivenValue()
    {
        var item = CreateBasketItem();

        item.SetQuantity(5);

        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void CanSetQuantityToZero()
    {
        var item = CreateBasketItem();

        item.SetQuantity(0);

        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public void ThrowsGivenNegativeQuantity()
    {
        var item = CreateBasketItem();

        Assert.Throws<ArgumentOutOfRangeException>(() => item.SetQuantity(-1));
    }
}
