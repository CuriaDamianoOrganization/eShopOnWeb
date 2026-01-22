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
    public async Task ReturnsNotFoundIfBasketNotExists()
    {
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns((Basket?)null);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);
        var quantities = new Dictionary<string, int>();

        var result = await basketService.SetQuantities(1, quantities);

        Assert.True(result.Status == ResultStatus.NotFound);
    }

    [Fact]
    public async Task UpdatesItemQuantitiesWhenBasketExists()
    {
        var basket = new Basket("testuser");
        basket.AddItem(1, 10.0m, 2);
        basket.AddItem(2, 20.0m, 3);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);
        
        Assert.Equal(2, basket.Items.Count);
        var itemToUpdate = basket.Items.First(i => i.CatalogItemId == 1);
        var itemId = itemToUpdate.Id;
        var quantities = new Dictionary<string, int>
        {
            { itemId.ToString(), 5 }
        };

        var result = await basketService.SetQuantities(basket.Id, quantities);

        Assert.True(result.IsSuccess);
        var updatedItem = basket.Items.Single(i => i.CatalogItemId == 1);
        Assert.Equal(5, updatedItem.Quantity);
        await _mockBasketRepo.Received().UpdateAsync(basket, default);
    }

    [Fact]
    public async Task RemovesEmptyItemsAfterUpdate()
    {
        var basket = new Basket("testuser");
        basket.AddItem(1, 10.0m, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);
        
        Assert.Single(basket.Items);
        var firstItem = basket.Items.First();
        var itemId = firstItem.Id;
        var quantities = new Dictionary<string, int>
        {
            { itemId.ToString(), 0 }
        };

        var result = await basketService.SetQuantities(basket.Id, quantities);

        Assert.True(result.IsSuccess);
        Assert.Empty(basket.Items);
    }

    [Fact]
    public async Task LogsInformationWhenLoggerIsNotNull()
    {
        var basket = new Basket("testuser");
        basket.AddItem(1, 10.0m, 2);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);

        var basketService = new BasketService(_mockBasketRepo, _mockLogger);
        
        Assert.Single(basket.Items);
        var firstItem = basket.Items.First();
        var itemId = firstItem.Id;
        var quantities = new Dictionary<string, int>
        {
            { itemId.ToString(), 5 }
        };

        await basketService.SetQuantities(basket.Id, quantities);

        _mockLogger.Received().LogInformation(Arg.Is<string>(s => s.Contains($"Updating quantity of item ID:{itemId} to 5")));
    }
}
