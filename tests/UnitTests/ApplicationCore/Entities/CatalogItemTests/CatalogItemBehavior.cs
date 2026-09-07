using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemTests;

public class CatalogItemBehavior
{
    private readonly CatalogItem _item = new(1, 2, "Description", "Name", 10m, "picture.jpg");

    [Fact]
    public void ConstructorSetsAllValues()
    {
        Assert.Equal(1, _item.CatalogTypeId);
        Assert.Equal(2, _item.CatalogBrandId);
        Assert.Equal("Description", _item.Description);
        Assert.Equal("Name", _item.Name);
        Assert.Equal(10m, _item.Price);
        Assert.Equal("picture.jpg", _item.PictureUri);
    }

    [Fact]
    public void UpdateDetailsSetsValues()
    {
        _item.UpdateDetails(new CatalogItem.CatalogItemDetails("New name", "New description", 25m));

        Assert.Equal("New name", _item.Name);
        Assert.Equal("New description", _item.Description);
        Assert.Equal(25m, _item.Price);
    }

    [Theory]
    [InlineData(null, "Description", 1)]
    [InlineData("", "Description", 1)]
    [InlineData("Name", null, 1)]
    [InlineData("Name", "", 1)]
    [InlineData("Name", "Description", 0)]
    [InlineData("Name", "Description", -1)]
    public void UpdateDetailsRejectsInvalidValues(string? name, string? description, decimal price)
    {
        var details = new CatalogItem.CatalogItemDetails(name, description, price);

        Assert.ThrowsAny<ArgumentException>(() => _item.UpdateDetails(details));
    }

    [Fact]
    public void UpdateBrandSetsBrandId()
    {
        _item.UpdateBrand(5);

        Assert.Equal(5, _item.CatalogBrandId);
    }

    [Fact]
    public void UpdateBrandRejectsZero()
    {
        Assert.Throws<ArgumentException>(() => _item.UpdateBrand(0));
    }

    [Fact]
    public void UpdateTypeSetsTypeId()
    {
        _item.UpdateType(5);

        Assert.Equal(5, _item.CatalogTypeId);
    }

    [Fact]
    public void UpdateTypeRejectsZero()
    {
        Assert.Throws<ArgumentException>(() => _item.UpdateType(0));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdatePictureUriClearsUriForMissingPictureName(string? pictureName)
    {
        _item.UpdatePictureUri(pictureName!);

        Assert.Equal(string.Empty, _item.PictureUri);
    }

    [Fact]
    public void UpdatePictureUriBuildsProductImagePath()
    {
        _item.UpdatePictureUri("new.jpg");

        Assert.Equal("images\\products\\new.jpg?0", _item.PictureUri);
    }
}