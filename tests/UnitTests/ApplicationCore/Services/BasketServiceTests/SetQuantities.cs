using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.BasketServiceTests;

public class SetQuantities
{
    private readonly string _buyerId = "Test buyerId";
    private readonly IRepository<Basket> _mockBasketRepo = Substitute.For<IRepository<Basket>>();
    private readonly IAppLogger<BasketService> _mockLogger = Substitute.For<IAppLogger<BasketService>>();

    private static void SetEntityId(BaseEntity entity, int id)
    {
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity, id);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenBasketDoesNotExist()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns((Basket?)null);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var result = await basketService.SetQuantities(1, new Dictionary<string, int>());

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdatesItemQuantitiesWhenMatchingKeysFound()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 10.00m, 1);
        basket.AddItem(2, 20.00m, 1);

        SetEntityId(basket.Items.First(i => i.CatalogItemId == 1), 101);
        SetEntityId(basket.Items.First(i => i.CatalogItemId == 2), 102);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { "101", 5 },
            { "102", 3 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(5, basket.Items.First(i => i.CatalogItemId == 1).Quantity);
        Assert.Equal(3, basket.Items.First(i => i.CatalogItemId == 2).Quantity);
    }

    [Fact]
    public async Task RemovesItemsWithZeroQuantityAfterSetting()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 10.00m, 2);
        basket.AddItem(2, 20.00m, 3);

        SetEntityId(basket.Items.First(i => i.CatalogItemId == 1), 201);
        SetEntityId(basket.Items.First(i => i.CatalogItemId == 2), 202);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { "201", 0 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items.First().CatalogItemId);
    }

    [Fact]
    public async Task DoesNotChangeItemQuantityIfIdNotInDictionary()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 10.00m, 4);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var quantities = new Dictionary<string, int>
        {
            { "999", 10 }
        };

        var result = await basketService.SetQuantities(1, quantities);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(4, basket.Items.First(i => i.CatalogItemId == 1).Quantity);
    }

    [Fact]
    public async Task CallsUpdateAsyncOnRepository()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 10.00m, 1);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        await basketService.SetQuantities(1, new Dictionary<string, int>());

        await _mockBasketRepo.Received().UpdateAsync(basket, default);
    }

    [Fact]
    public async Task WorksWithEmptyQuantitiesDictionary()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 10.00m, 2);
        basket.AddItem(2, 20.00m, 3);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);
        var basketService = new BasketService(_mockBasketRepo, _mockLogger);

        var result = await basketService.SetQuantities(1, new Dictionary<string, int>());

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(2, basket.Items.Count);
        Assert.Equal(2, basket.Items.First(i => i.CatalogItemId == 1).Quantity);
        Assert.Equal(3, basket.Items.First(i => i.CatalogItemId == 2).Quantity);
    }
}
