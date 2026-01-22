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

    [Fact]
    public async Task ThrowsExceptionIfBasketIsNull()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns((Basket?)null);

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("123 Main St", "Kent", "OH", "US", "44240");

        await Assert.ThrowsAsync<System.ArgumentNullException>(async () =>
            await orderService.CreateOrderAsync(1, address));
    }

    [Fact]
    public async Task ThrowsExceptionIfBasketIsEmpty()
    {
        var basket = new Basket("testbuyer");
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("123 Main St", "Kent", "OH", "US", "44240");

        await Assert.ThrowsAsync<EmptyBasketOnCheckoutException>(async () =>
            await orderService.CreateOrderAsync(basket.Id, address));
    }

    [Fact]
    public async Task CreatesOrderWithCorrectBuyerAndShippingAddress()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.0m, 2);

        var catalogItem = new CatalogItem(1, 1, "Description", "Item 1", 10.0m, "item1.png");
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns("http://test.com/item1.png");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("123 Main St", "Kent", "OH", "US", "44240");

        await orderService.CreateOrderAsync(basket.Id, address);

        await _mockOrderRepo.Received().AddAsync(Arg.Is<Order>(o => 
            o.BuyerId == "testbuyer" && 
            o.ShipToAddress == address), default);
    }

    [Fact]
    public async Task CreatesOrderWithCorrectItems()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.0m, 2);
        basket.AddItem(2, 15.0m, 1);

        var catalogItem1 = new CatalogItem(1, 1, "Description 1", "Item 1", 10.0m, "item1.png");
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem1, 1);
        
        var catalogItem2 = new CatalogItem(1, 1, "Description 2", "Item 2", 15.0m, "item2.png");
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem2, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem1, catalogItem2 });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns(callInfo => $"http://test.com/{callInfo.Arg<string>()}");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("123 Main St", "Kent", "OH", "US", "44240");

        await orderService.CreateOrderAsync(basket.Id, address);

        await _mockOrderRepo.Received().AddAsync(Arg.Is<Order>(o => 
            o.OrderItems.Count == 2 &&
            o.OrderItems.Any(oi => oi.ItemOrdered.CatalogItemId == 1 && oi.Units == 2 && oi.UnitPrice == 10.0m) &&
            o.OrderItems.Any(oi => oi.ItemOrdered.CatalogItemId == 2 && oi.Units == 1 && oi.UnitPrice == 15.0m)), default);
    }

    [Fact]
    public async Task InvokesUriComposerForEachItem()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.0m, 2);
        basket.AddItem(2, 15.0m, 1);

        var catalogItem1 = new CatalogItem(1, 1, "Description 1", "Item 1", 10.0m, "item1.png");
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem1, 1);
        
        var catalogItem2 = new CatalogItem(1, 1, "Description 2", "Item 2", 15.0m, "item2.png");
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem2, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem1, catalogItem2 });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns("http://test.com/pic.png");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("123 Main St", "Kent", "OH", "US", "44240");

        await orderService.CreateOrderAsync(basket.Id, address);

        _mockUriComposer.Received(2).ComposePicUri(Arg.Any<string>());
    }
}
