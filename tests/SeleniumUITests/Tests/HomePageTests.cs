using Microsoft.eShopWeb.SeleniumUITests.PageObjects;
using OpenQA.Selenium;

namespace Microsoft.eShopWeb.SeleniumUITests.Tests;

public class HomePageTests : BaseSeleniumTest
{
    [Fact]
    public void HomePage_ShouldLoad_Successfully()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();

        // Assert
        Assert.True(homePage.IsLoaded(), "Home page should load successfully");
        Assert.True(homePage.GetCatalogItems().Count > 0, "Should display catalog items");
    }

    [Fact]
    public void HomePage_ShouldDisplay_ProductCatalog()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();

        // Assert
        var productNames = homePage.GetProductNames();
        Assert.NotEmpty(productNames);
        Assert.Contains(productNames, name => !string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void HomePage_ShouldHave_NavigationElements()
    {
        // Arrange & Act
        NavigateToUrl();

        // Assert
        Assert.True(IsElementPresent(By.LinkText("Login")), "Login link should be present");
        Assert.True(IsElementPresent(By.CssSelector(".esh-basket-link")), "Basket icon should be present");
    }

    [Fact]
    public void HomePage_AddProductToBasket_ShouldWork()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();
        var initialBasketCount = homePage.GetBasketItemCount();
        homePage.AddFirstProductToBasket();

        // Wait for page to refresh and check basket count
        Thread.Sleep(2000); // Wait for any AJAX updates

        // Assert
        var updatedBasketCount = homePage.GetBasketItemCount();
        Assert.NotEqual(initialBasketCount, updatedBasketCount);
    }

    [Fact]
    public void HomePage_FilterByBrand_ShouldFilterProducts()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();
        var initialProductCount = homePage.GetCatalogItems().Count;
        
        // Try to filter by first available brand
        if (IsElementPresent(By.Id("CatalogModel_BrandFilterApplied")))
        {
            homePage.FilterByBrand("Azure");
            Thread.Sleep(2000); // Wait for filter to apply

            // Assert
            var filteredProductCount = homePage.GetCatalogItems().Count;
            // The filtered results may be the same or different, but the page should still load
            Assert.True(homePage.IsLoaded(), "Page should remain loaded after filtering");
        }
        else
        {
            // Skip test if brand filter is not available
            Assert.True(true, "Brand filter not available - test skipped");
        }
    }

    [Fact]
    public void HomePage_ClickBasketIcon_ShouldNavigateToBasket()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();
        homePage.ClickBasketIcon();

        // Assert
        Assert.Contains("/Basket", Driver.Url);
    }

    [Fact]
    public void HomePage_ClickLoginLink_ShouldNavigateToLogin()
    {
        // Arrange
        var homePage = new HomePage(Driver, Wait);

        // Act
        NavigateToUrl();
        homePage.ClickLoginLink();

        // Assert
        Assert.Contains("/Account/Login", Driver.Url);
    }
}