using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumUITests.PageObjects;

public class HomePage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Locators
    private readonly By _catalogItems = By.CssSelector(".esh-catalog-item");
    private readonly By _productNames = By.CssSelector(".esh-catalog-item .esh-catalog-title");
    private readonly By _productPrices = By.CssSelector(".esh-catalog-item .esh-catalog-price");
    private readonly By _addToBasketButtons = By.CssSelector(".esh-catalog-item .btn-cart");
    private readonly By _loginLink = By.LinkText("Login");
    private readonly By _basketIcon = By.CssSelector(".esh-basket-link");
    private readonly By _searchBox = By.Id("search");
    private readonly By _brandFilter = By.Id("CatalogModel_BrandFilterApplied");
    private readonly By _typeFilter = By.Id("CatalogModel_TypesFilterApplied");
    private readonly By _filterButton = By.CssSelector("input[value='FILTER']");

    public HomePage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void NavigateTo()
    {
        _driver.Navigate().GoToUrl(_driver.Url.Replace(_driver.Url.Split('/').Last(), ""));
    }

    public bool IsLoaded()
    {
        try
        {
            _wait.Until(driver => driver.FindElements(_catalogItems).Count > 0);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public IList<IWebElement> GetCatalogItems()
    {
        return _driver.FindElements(_catalogItems);
    }

    public IList<string> GetProductNames()
    {
        return _driver.FindElements(_productNames)
                     .Select(element => element.Text)
                     .ToList();
    }

    public void AddFirstProductToBasket()
    {
        var firstAddButton = _wait.Until(driver => driver.FindElement(_addToBasketButtons));
        firstAddButton.Click();
    }

    public void AddProductToBasketByIndex(int index)
    {
        var addButtons = _driver.FindElements(_addToBasketButtons);
        if (index < addButtons.Count)
        {
            addButtons[index].Click();
        }
        else
        {
            throw new ArgumentException($"Product index {index} out of range. Only {addButtons.Count} products available.");
        }
    }

    public void AddProductToBasketByName(string productName)
    {
        var catalogItems = _driver.FindElements(_catalogItems);
        
        foreach (var item in catalogItems)
        {
            var title = item.FindElement(By.CssSelector(".esh-catalog-title"));
            if (title.Text.Contains(productName, StringComparison.OrdinalIgnoreCase))
            {
                var addButton = item.FindElement(By.CssSelector(".btn-cart"));
                addButton.Click();
                return;
            }
        }
        
        throw new ArgumentException($"Product '{productName}' not found on the page.");
    }

    public void ClickLoginLink()
    {
        var loginLink = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_loginLink);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        loginLink.Click();
    }

    public void ClickBasketIcon()
    {
        var basketIcon = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_basketIcon);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        basketIcon.Click();
    }

    public void FilterByBrand(string brandName)
    {
        var brandSelect = new SelectElement(_driver.FindElement(_brandFilter));
        brandSelect.SelectByText(brandName);
        
        var filterButton = _driver.FindElement(_filterButton);
        filterButton.Click();
    }

    public void FilterByType(string typeName)
    {
        var typeSelect = new SelectElement(_driver.FindElement(_typeFilter));
        typeSelect.SelectByText(typeName);
        
        var filterButton = _driver.FindElement(_filterButton);
        filterButton.Click();
    }

    public bool IsUserLoggedIn()
    {
        try
        {
            _driver.FindElement(By.LinkText("Log Out"));
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public string GetBasketItemCount()
    {
        try
        {
            var basketCount = _driver.FindElement(By.CssSelector(".esh-basket-quantity"));
            return basketCount.Text;
        }
        catch (NoSuchElementException)
        {
            return "0";
        }
    }
}