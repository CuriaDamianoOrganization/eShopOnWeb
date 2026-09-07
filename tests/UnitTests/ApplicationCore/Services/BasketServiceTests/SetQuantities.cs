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
    private readonly IRepository<Basket> _basketRepository = Substitute.For<IRepository<Basket>>();
    private readonly IAppLogger<BasketService> _logger = Substitute.For<IAppLogger<BasketService>>();

    [Fact]
    public async Task ReturnsNotFoundWhenBasketDoesNotExist()
    {
        _basketRepository.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns((Basket?)null);
        var service = new BasketService(_basketRepository, _logger);

        var result = await service.SetQuantities(123, new Dictionary<string, int>());

        Assert.Equal(ResultStatus.NotFound, result.Status);
        await _basketRepository.DidNotReceive().UpdateAsync(Arg.Any<Basket>(), default);
    }

    [Fact]
    public async Task UpdatesMatchingQuantitiesAndRemovesEmptyItems()
    {
        var basket = new Basket("buyer@example.com");
        basket.AddItem(1, 10m, 2);
        _basketRepository.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        var service = new BasketService(_basketRepository, _logger);

        var result = await service.SetQuantities(123, new Dictionary<string, int> { ["0"] = 0 });

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Empty(basket.Items);
        _logger.Received(1).LogInformation(Arg.Is<string>(message => message.Contains("to 0")));
        await _basketRepository.Received(1).UpdateAsync(basket, default);
    }

    [Fact]
    public async Task LeavesQuantityUnchangedWhenItemIsNotInRequest()
    {
        var basket = new Basket("buyer@example.com");
        basket.AddItem(1, 10m, 2);
        _basketRepository.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        var service = new BasketService(_basketRepository, _logger);

        await service.SetQuantities(123, new Dictionary<string, int> { ["999"] = 5 });

        Assert.Equal(2, Assert.Single(basket.Items).Quantity);
        _logger.DidNotReceive().LogInformation(Arg.Any<string>());
        await _basketRepository.Received(1).UpdateAsync(basket, default);
    }

    [Fact]
    public async Task UpdatesQuantityWhenLoggerIsNull()
    {
        var basket = new Basket("buyer@example.com");
        basket.AddItem(1, 10m, 1);
        _basketRepository.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default)
            .Returns(basket);
        var service = new BasketService(_basketRepository, null!);

        await service.SetQuantities(123, new Dictionary<string, int> { ["0"] = 3 });

        Assert.Equal(3, Assert.Single(basket.Items).Quantity);
    }
}