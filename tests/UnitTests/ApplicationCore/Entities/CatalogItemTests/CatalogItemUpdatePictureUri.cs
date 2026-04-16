using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.CatalogItemTests;

public class CatalogItemUpdatePictureUri
{
    private CatalogItem CreateCatalogItem()
    {
        return new CatalogItem(1, 1, "desc", "name", 10m, "uri");
    }

    [Fact]
    public void SetsPictureUriWithPictureName()
    {
        var item = CreateCatalogItem();

        item.UpdatePictureUri("test-image.png");

        Assert.NotEqual(string.Empty, item.PictureUri);
        Assert.Contains("test-image.png", item.PictureUri);
    }

    [Fact]
    public void SetsEmptyPictureUriWhenNameIsNull()
    {
        var item = CreateCatalogItem();

        item.UpdatePictureUri(null!);

        Assert.Equal(string.Empty, item.PictureUri);
    }

    [Fact]
    public void SetsEmptyPictureUriWhenNameIsEmpty()
    {
        var item = CreateCatalogItem();

        item.UpdatePictureUri(string.Empty);

        Assert.Equal(string.Empty, item.PictureUri);
    }

    [Fact]
    public void PictureUriContainsPictureName()
    {
        var item = CreateCatalogItem();
        var pictureName = "my-product-photo.jpg";

        item.UpdatePictureUri(pictureName);

        Assert.Contains(pictureName, item.PictureUri);
    }
}
