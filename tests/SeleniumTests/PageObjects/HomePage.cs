using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumTests.PageObjects;

public class HomePage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public HomePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    // Page elements
    private By ProductsContainer => By.CssSelector(".esh-catalog-items");
    private By ProductItems => By.CssSelector(".esh-catalog-item");
    private By ProductTitle => By.CssSelector(".esh-catalog-item h3");
    private By LoginLink => By.CssSelector("a[href*='login']");
    private By BrandFilter => By.CssSelector(".esh-filters select[name='BrandFilterApplied']");
    private By TypeFilter => By.CssSelector(".esh-filters select[name='TypesFilterApplied']");
    private By AddToBasketButtons => By.CssSelector(".esh-catalog-item form input[type='submit']");

    // Navigation methods
    public void Navigate(string baseUrl)
    {
        _driver.Navigate().GoToUrl(baseUrl);
    }

    // Verification methods
    public bool IsProductListingVisible()
    {
        try
        {
            _wait.Until(driver => driver.FindElement(ProductsContainer));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool HasProducts()
    {
        return _driver.FindElements(ProductItems).Count > 0;
    }

    public List<string> GetProductTitles()
    {
        var productElements = _driver.FindElements(ProductTitle);
        return productElements.Select(element => element.Text).ToList();
    }

    public bool IsProductDisplayed(string productName)
    {
        var productTitles = GetProductTitles();
        return productTitles.Any(title => title.Contains(productName, StringComparison.OrdinalIgnoreCase));
    }

    // Interaction methods
    public void ClickLoginLink()
    {
        var loginElement = _wait.Until(driver => driver.FindElement(LoginLink));
        loginElement.Click();
    }

    public void SelectBrandFilter(string brandName)
    {
        var brandSelect = new SelectElement(_driver.FindElement(BrandFilter));
        brandSelect.SelectByText(brandName);
    }

    public void SelectTypeFilter(string typeName)
    {
        var typeSelect = new SelectElement(_driver.FindElement(TypeFilter));
        typeSelect.SelectByText(typeName);
    }

    public void AddFirstProductToBasket()
    {
        var addToBasketButtons = _driver.FindElements(AddToBasketButtons);
        if (addToBasketButtons.Count > 0)
        {
            addToBasketButtons[0].Click();
        }
    }

    public string GetPageTitle()
    {
        return _driver.Title;
    }
}