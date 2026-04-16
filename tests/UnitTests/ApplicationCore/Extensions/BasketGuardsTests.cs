using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Extensions;

public class BasketGuardsTests
{
    [Fact]
    public void ThrowsGivenEmptyBasketItemsList()
    {
        var items = new List<BasketItem>();

        Assert.Throws<EmptyBasketOnCheckoutException>(() =>
            Guard.Against.EmptyBasketOnCheckout(items));
    }

    [Fact]
    public void DoesNotThrowGivenNonEmptyBasketItemsList()
    {
        var items = new List<BasketItem> { new BasketItem(1, 1, 10.00m) };

        var exception = Record.Exception(() =>
            Guard.Against.EmptyBasketOnCheckout(items));

        Assert.Null(exception);
    }
}
