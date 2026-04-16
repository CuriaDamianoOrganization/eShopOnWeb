using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketTests;

public class BasketSetNewBuyerId
{
    private readonly string _originalBuyerId = "original-buyer-id";

    [Fact]
    public void SetsBuyerIdToNewValue()
    {
        var basket = new Basket(_originalBuyerId);
        var newBuyerId = "new-buyer-id";

        basket.SetNewBuyerId(newBuyerId);

        Assert.Equal(newBuyerId, basket.BuyerId);
    }

    [Fact]
    public void CanSetBuyerIdToEmptyString()
    {
        var basket = new Basket(_originalBuyerId);

        basket.SetNewBuyerId(string.Empty);

        Assert.Equal(string.Empty, basket.BuyerId);
    }
}
