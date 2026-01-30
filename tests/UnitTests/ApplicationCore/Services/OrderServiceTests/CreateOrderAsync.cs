using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
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

public class CreateOrderAsync
{
    private readonly IRepository<Basket> _mockBasketRepo = Substitute.For<IRepository<Basket>>();
    private readonly IRepository<CatalogItem> _mockItemRepo = Substitute.For<IRepository<CatalogItem>>();
    private readonly IRepository<Order> _mockOrderRepo = Substitute.For<IRepository<Order>>();
    private readonly IUriComposer _mockUriComposer = Substitute.For<IUriComposer>();

    [Fact]
    public async Task ThrowsExceptionGivenNullBasket()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(Task.FromResult<Basket>(null!));

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await orderService.CreateOrderAsync(1, new Address("Test", "Test", "Test", "Test", "Test")));
    }

    [Fact]
    public async Task ThrowsExceptionGivenEmptyBasket()
    {
        var basket = new Basket("testbuyer");
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await Assert.ThrowsAsync<EmptyBasketOnCheckoutException>(async () =>
            await orderService.CreateOrderAsync(1, new Address("Test", "Test", "Test", "Test", "Test")));
    }

    [Fact]
    public async Task CreatesOrderGivenValidBasket()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 2);
        basket.AddItem(2, 5.00m, 1);

        var catalogItem1 = new CatalogItem(1, 1, "Item 1", "Item 1", 10.00m, "item1.png");
        SetId(catalogItem1, 1);
        var catalogItem2 = new CatalogItem(1, 1, "Item 2", "Item 2", 5.00m, "item2.png");
        SetId(catalogItem2, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem1, catalogItem2 });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns(x => $"http://test.com/{x[0]}");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);
        var address = new Address("Test", "Test", "Test", "Test", "Test");

        await orderService.CreateOrderAsync(1, address);

        await _mockOrderRepo.Received(1).AddAsync(Arg.Is<Order>(o => 
            o.BuyerId == "testbuyer" && 
            o.OrderItems.Count == 2 &&
            o.ShipToAddress == address), default);
    }

    [Fact]
    public async Task InvokesBasketRepositoryFirstOrDefaultAsyncOnce()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 1);

        var catalogItem = new CatalogItem(1, 1, "Item 1", "Item 1", 10.00m, "item1.png");
        SetId(catalogItem, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns("http://test.com/item1.png");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await orderService.CreateOrderAsync(1, new Address("Test", "Test", "Test", "Test", "Test"));

        await _mockBasketRepo.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default);
    }

    [Fact]
    public async Task InvokesCatalogItemRepositoryListAsyncOnce()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 1);

        var catalogItem = new CatalogItem(1, 1, "Item 1", "Item 1", 10.00m, "item1.png");
        SetId(catalogItem, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns("http://test.com/item1.png");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await orderService.CreateOrderAsync(1, new Address("Test", "Test", "Test", "Test", "Test"));

        await _mockItemRepo.Received(1).ListAsync(Arg.Any<CatalogItemsSpecification>(), default);
    }

    [Fact]
    public async Task InvokesUriComposerForEachBasketItem()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 1);
        basket.AddItem(2, 5.00m, 1);

        var catalogItem1 = new CatalogItem(1, 1, "Item 1", "Item 1", 10.00m, "item1.png");
        SetId(catalogItem1, 1);
        var catalogItem2 = new CatalogItem(1, 1, "Item 2", "Item 2", 5.00m, "item2.png");
        SetId(catalogItem2, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        _mockItemRepo.ListAsync(Arg.Any<CatalogItemsSpecification>(), default)
            .Returns(new List<CatalogItem> { catalogItem1, catalogItem2 });
        _mockUriComposer.ComposePicUri(Arg.Any<string>())
            .Returns(x => $"http://test.com/{x[0]}");

        var orderService = new OrderService(_mockBasketRepo, _mockItemRepo, _mockOrderRepo, _mockUriComposer);

        await orderService.CreateOrderAsync(1, new Address("Test", "Test", "Test", "Test", "Test"));

        _mockUriComposer.Received(2).ComposePicUri(Arg.Any<string>());
    }

    private static void SetId(CatalogItem item, int id)
    {
        var idProperty = typeof(CatalogItem).BaseType!.GetProperty("Id");
        idProperty!.SetValue(item, id);
    }
}
