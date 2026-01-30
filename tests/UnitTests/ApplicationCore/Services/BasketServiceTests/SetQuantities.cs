using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.BasketServiceTests;

public class SetQuantities
{
    private readonly IRepository<Basket> _mockBasketRepo = Substitute.For<IRepository<Basket>>();
    private readonly IAppLogger<BasketService> _mockLogger = Substitute.For<IAppLogger<BasketService>>();

    [Fact]
    public async Task ReturnsNotFoundIfBasketDoesNotExist()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(Task.FromResult<Basket>(null!));

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var result = await basketService.SetQuantities(1, new Dictionary<string, int>());

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdatesItemQuantitiesSuccessfully()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);
        basket.AddItem(2, 5.00m, 3);

        // Set unique IDs for each item
        SetItemId(basket.Items.First(i => i.CatalogItemId == 1), 101);
        SetItemId(basket.Items.First(i => i.CatalogItemId == 2), 102);

        var itemId1 = "101";
        var itemId2 = "102";

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { itemId1, 10 },
            { itemId2, 1 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, basket.Items.First(i => i.CatalogItemId == 1).Quantity);
        Assert.Equal(1, basket.Items.First(i => i.CatalogItemId == 2).Quantity);
    }

    [Fact]
    public async Task RemovesItemsWithZeroQuantity()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);
        basket.AddItem(2, 5.00m, 3);

        // Set unique IDs for each item
        SetItemId(basket.Items.First(i => i.CatalogItemId == 1), 101);
        SetItemId(basket.Items.First(i => i.CatalogItemId == 2), 102);

        var itemId1 = "101";

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { itemId1, 0 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.True(result.IsSuccess);
        Assert.Single(basket.Items);
        Assert.DoesNotContain(basket.Items, i => i.CatalogItemId == 1);
    }

    [Fact]
    public async Task InvokesBasketRepositoryFirstOrDefaultAsyncOnce()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        await basketService.SetQuantities(1, new Dictionary<string, int>());

        await _mockBasketRepo.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default);
    }

    [Fact]
    public async Task InvokesBasketRepositoryUpdateAsyncOnce()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        await basketService.SetQuantities(1, new Dictionary<string, int>());

        await _mockBasketRepo.Received(1).UpdateAsync(basket, default);
    }

    [Fact]
    public async Task IgnoresItemIdsNotInBasket()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { "999", 10 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, basket.Items.First().Quantity);
    }

    [Fact]
    public async Task ReturnsUpdatedBasket()
    {
        var basket = new Basket("testbuyer");
        basket.AddItem(1, 10.00m, 5);

        // Set unique ID for the item
        SetItemId(basket.Items.First(), 101);

        var itemId = "101";

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { itemId, 10 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.True(result.IsSuccess);
        Assert.Same(basket, result.Value);
    }

    private static void SetItemId(BasketItem item, int id)
    {
        var idProperty = typeof(BasketItem).BaseType!.GetProperty("Id");
        idProperty!.SetValue(item, id);
    }
}
