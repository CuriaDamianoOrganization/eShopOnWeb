using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketTests;

public class BasketItemBehavior
{
    [Fact]
    public void ConstructorSetsValues()
    {
        var item = new BasketItem(1, 2, 3.50m);

        Assert.Equal(1, item.CatalogItemId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(3.50m, item.UnitPrice);
    }

    [Fact]
    public void ConstructorAllowsZeroQuantity()
    {
        var item = new BasketItem(1, 0, 3.50m);

        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public void ConstructorRejectsNegativeQuantity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BasketItem(1, -1, 3.50m));
    }

    [Fact]
    public void AddQuantityAddsPositiveQuantity()
    {
        var item = new BasketItem(1, 2, 3.50m);

        item.AddQuantity(3);

        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AddQuantityAllowsZero()
    {
        var item = new BasketItem(1, 2, 3.50m);

        item.AddQuantity(0);

        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void AddQuantityRejectsNegativeQuantity()
    {
        var item = new BasketItem(1, 2, 3.50m);

        Assert.Throws<ArgumentOutOfRangeException>(() => item.AddQuantity(-1));
    }

    [Fact]
    public void SetQuantitySetsNonNegativeQuantity()
    {
        var item = new BasketItem(1, 2, 3.50m);

        item.SetQuantity(0);

        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public void SetQuantityRejectsNegativeQuantity()
    {
        var item = new BasketItem(1, 2, 3.50m);

        Assert.Throws<ArgumentOutOfRangeException>(() => item.SetQuantity(-1));
    }
}