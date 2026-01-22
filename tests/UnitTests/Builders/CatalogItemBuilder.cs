using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.UnitTests.Builders;

public class CatalogItemBuilder
{
    private CatalogItem _catalogItem;
    private int _testId = 1;
    private int _testCatalogTypeId = 1;
    private int _testCatalogBrandId = 1;
    private string _testDescription = "Test Description";
    private string _testName = "Test Name";
    private decimal _testPrice = 10.0m;
    private string _testPictureUri = "test.png";

    public CatalogItemBuilder()
    {
        _catalogItem = WithDefaultValues();
    }

    public CatalogItem Build()
    {
        return _catalogItem;
    }

    public CatalogItem WithDefaultValues()
    {
        _catalogItem = new CatalogItem(_testCatalogTypeId, _testCatalogBrandId, _testDescription, _testName, _testPrice, _testPictureUri);
        
        // Use reflection to set the Id since it's a protected setter
        typeof(CatalogItem).GetProperty("Id")!.SetValue(_catalogItem, _testId);
        
        return _catalogItem;
    }

    public CatalogItemBuilder WithId(int id)
    {
        _testId = id;
        typeof(CatalogItem).GetProperty("Id")!.SetValue(_catalogItem, id);
        return this;
    }

    public CatalogItemBuilder WithName(string name)
    {
        _testName = name;
        _catalogItem = new CatalogItem(_testCatalogTypeId, _testCatalogBrandId, _testDescription, name, _testPrice, _testPictureUri);
        typeof(CatalogItem).GetProperty("Id")!.SetValue(_catalogItem, _testId);
        return this;
    }

    public CatalogItemBuilder WithPrice(decimal price)
    {
        _testPrice = price;
        _catalogItem = new CatalogItem(_testCatalogTypeId, _testCatalogBrandId, _testDescription, _testName, price, _testPictureUri);
        typeof(CatalogItem).GetProperty("Id")!.SetValue(_catalogItem, _testId);
        return this;
    }

    public CatalogItemBuilder WithPictureUri(string pictureUri)
    {
        _testPictureUri = pictureUri;
        _catalogItem = new CatalogItem(_testCatalogTypeId, _testCatalogBrandId, _testDescription, _testName, _testPrice, pictureUri);
        typeof(CatalogItem).GetProperty("Id")!.SetValue(_catalogItem, _testId);
        return this;
    }
}
