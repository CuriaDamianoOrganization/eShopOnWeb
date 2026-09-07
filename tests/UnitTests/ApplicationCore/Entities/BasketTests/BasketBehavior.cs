using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.BasketTests;

public class BasketBehavior
{
    [Fact]
    public void ConstructorSetsBuyerId()
    {
        var basket = new Basket("anonymous");

        Assert.Equal("anonymous", basket.BuyerId);
        Assert.Empty(basket.Items);
    }

    [Fact]
    public void SetNewBuyerIdChangesBuyerId()
    {
        var basket = new Basket("anonymous");

        basket.SetNewBuyerId("buyer@example.com");

        Assert.Equal("buyer@example.com", basket.BuyerId);
    }

    [Theory]
    [InlineData("buyer@example.com")]
    [InlineData("first.last+tag@sub.example.co.uk")]
    public void IsValidEmailReturnsTrueForValidAddress(string email)
    {
        Assert.True(Basket.IsValidEmail(email));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("missing-at.example.com")]
    [InlineData("missing-domain@")]
    [InlineData("missing-dot@example")]
    [InlineData("white space@example.com")]
    public void IsValidEmailReturnsFalseForInvalidAddress(string? email)
    {
        Assert.False(Basket.IsValidEmail(email!));
    }
}