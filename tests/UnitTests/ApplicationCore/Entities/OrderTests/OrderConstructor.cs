using System;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests;

public class OrderConstructor
{
    [Fact]
    public void SetsBuyerIdAndAddress()
    {
        var builder = new OrderBuilder();
        var address = new AddressBuilder().WithDefaultValues();
        var items = new List<OrderItem>
        {
            new OrderItem(builder.TestCatalogItemOrdered, builder.TestUnitPrice, builder.TestUnits)
        };

        var order = new Order(builder.TestBuyerId, address, items);

        Assert.Equal(builder.TestBuyerId, order.BuyerId);
        Assert.Equal(address, order.ShipToAddress);
    }

    [Fact]
    public void ThrowsWhenBuyerIdIsNull()
    {
        var address = new AddressBuilder().WithDefaultValues();
        var items = new List<OrderItem>();

        Assert.Throws<ArgumentNullException>(() => new Order(null!, address, items));
    }

    [Fact]
    public void ThrowsWhenBuyerIdIsEmpty()
    {
        var address = new AddressBuilder().WithDefaultValues();
        var items = new List<OrderItem>();

        Assert.Throws<ArgumentException>(() => new Order(string.Empty, address, items));
    }
}
