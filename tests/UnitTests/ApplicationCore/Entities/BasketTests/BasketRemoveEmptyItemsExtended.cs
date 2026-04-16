using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketTests;

public class BasketRemoveEmptyItemsExtended
{
    private readonly string _buyerId = "Test buyerId";
    private readonly int _testCatalogItemId = 123;
    private readonly decimal _testUnitPrice = 1.23m;

    [Fact]
    public void RetainsNonEmptyItems()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(_testCatalogItemId, _testUnitPrice, 5);
        basket.AddItem(_testCatalogItemId + 1, _testUnitPrice, 3);

        basket.RemoveEmptyItems();

        Assert.Equal(2, basket.Items.Count);
    }

    [Fact]
    public void RemovesOnlyEmptyItemsKeepingNonEmpty()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(_testCatalogItemId, _testUnitPrice, 0);
        basket.AddItem(_testCatalogItemId + 1, _testUnitPrice, 5);
        basket.AddItem(_testCatalogItemId + 2, _testUnitPrice, 0);

        basket.RemoveEmptyItems();

        Assert.Equal(1, basket.Items.Count);
        Assert.All(basket.Items, item => Assert.True(item.Quantity > 0));
    }

    [Fact]
    public void DoesNothingOnEmptyBasket()
    {
        var basket = new Basket(_buyerId);

        basket.RemoveEmptyItems();

        Assert.Empty(basket.Items);
    }
}
