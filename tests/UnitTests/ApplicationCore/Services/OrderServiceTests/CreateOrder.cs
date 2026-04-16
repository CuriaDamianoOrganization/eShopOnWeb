using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.OrderServiceTests;

public class CreateOrder
{
    private readonly IRepository<Basket> _mockBasketRepo = Substitute.For<IRepository<Basket>>();
    private readonly IRepository<CatalogItem> _mockItemRepo = Substitute.For<IRepository<CatalogItem>>();
    private readonly IRepository<Order> _mockOrderRepo = Substitute.For<IRepository<Order>>();
    private readonly IUriComposer _mockUriComposer = Substitute.For<IUriComposer>();

    private readonly Address _testAddress = new Address("123 Street", "City", "State", "Country", "12345");

    [Fact]
    public async Task ThrowsWhenBasketIsNull()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns((Basket?)null);
        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await orderService.CreateOrderAsync(1, _testAddress));
    }

    [Fact]
    public async Task ThrowsEmptyBasketOnCheckoutExceptionWhenBasketHasNoItems()
    {
        var basket = new Basket("testbuyer");
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await Assert.ThrowsAsync<EmptyBasketOnCheckoutException>(async () =>
            await orderService.CreateOrderAsync(1, _testAddress));
    }

    [Fact]
    public async Task CreatesOrderWithCorrectItemsWhenBasketIsValid()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(10, 15.00m, 2);
        basket.AddItem(20, 25.00m, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);

        var catalogItem1 = new CatalogItem(1, 1, "Desc1", "Product1", 15.00m, "http://catalogbaseurltobereplaced/images/1.png");
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(catalogItem1, 10);

        var catalogItem2 = new CatalogItem(1, 1, "Desc2", "Product2", 25.00m, "http://catalogbaseurltobereplaced/images/2.png");
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(catalogItem2, 20);

        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem1, catalogItem2 });

        _mockUriComposer.ComposePicUri(Arg.Any<string>()).Returns(x => (string)x[0]);

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await orderService.CreateOrderAsync(1, _testAddress);

        await _mockOrderRepo.Received(1).AddAsync(Arg.Is<Order>(o =>
            o.OrderItems.Count == 2 &&
            o.OrderItems.Any(oi => oi.UnitPrice == 15.00m && oi.Units == 2) &&
            o.OrderItems.Any(oi => oi.UnitPrice == 25.00m && oi.Units == 1)),
            default);
    }

    [Fact]
    public async Task CallsAddAsyncOnOrderRepository()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(10, 15.00m, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);

        var catalogItem = new CatalogItem(1, 1, "Desc", "Product", 15.00m, "http://catalogbaseurltobereplaced/images/1.png");
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(catalogItem, 10);

        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem });

        _mockUriComposer.ComposePicUri(Arg.Any<string>()).Returns(x => (string)x[0]);

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await orderService.CreateOrderAsync(1, _testAddress);

        await _mockOrderRepo.Received(1).AddAsync(Arg.Any<Order>(), default);
    }
}
