using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.UnitTests.Builders;

public class CatalogItemBuilder
{
    private int _testId = 1;
    private int _testCatalogTypeId = 1;
    private int _testCatalogBrandId = 1;
    private string _testDescription = "Test Description";
    private string _testName = "Test Name";
    private decimal _testPrice = 10.0m;
    private string _testPictureUri = "test.png";

    public CatalogItem Build()
    {
        var catalogItem = new CatalogItem(_testCatalogTypeId, _testCatalogBrandId, _testDescription, _testName, _testPrice, _testPictureUri);
        SetId(catalogItem, _testId);
        return catalogItem;
    }

    public CatalogItemBuilder WithId(int id)
    {
        _testId = id;
        return this;
    }

    public CatalogItemBuilder WithName(string name)
    {
        _testName = name;
        return this;
    }

    public CatalogItemBuilder WithPrice(decimal price)
    {
        _testPrice = price;
        return this;
    }

    public CatalogItemBuilder WithPictureUri(string pictureUri)
    {
        _testPictureUri = pictureUri;
        return this;
    }

    public CatalogItemBuilder WithDescription(string description)
    {
        _testDescription = description;
        return this;
    }

    public CatalogItemBuilder WithCatalogTypeId(int catalogTypeId)
    {
        _testCatalogTypeId = catalogTypeId;
        return this;
    }

    public CatalogItemBuilder WithCatalogBrandId(int catalogBrandId)
    {
        _testCatalogBrandId = catalogBrandId;
        return this;
    }

    private void SetId(CatalogItem catalogItem, int id)
    {
        // Use reflection to set the Id since it's a protected setter
        typeof(CatalogItem).GetProperty("Id")!.SetValue(catalogItem, id);
    }
}
