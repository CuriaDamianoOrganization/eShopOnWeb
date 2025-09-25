using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumTests.PageObjects;

public class BasketPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public BasketPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    // Page elements
    private By BasketItems => By.CssSelector(".esh-basket-item");
    private By ItemName => By.CssSelector(".esh-basket-item .esh-basket-item-name");
    private By ItemPrice => By.CssSelector(".esh-basket-item .esh-basket-item-price");
    private By ItemQuantity => By.CssSelector(".esh-basket-item input[name='quantity']");
    private By UpdateButton => By.CssSelector("input[value='Update']");
    private By CheckoutButton => By.CssSelector("input[value='Checkout'], a[href*='checkout']");
    private By EmptyBasketMessage => By.CssSelector(".esh-basket-empty");
    private By TotalPrice => By.CssSelector(".esh-basket-total");

    // Navigation methods
    public void Navigate(string baseUrl)
    {
        _driver.Navigate().GoToUrl($"{baseUrl}/Basket");
    }

    // Verification methods
    public bool IsBasketPageVisible()
    {
        try
        {
            // Wait for either basket items or empty basket message
            _wait.Until(driver => 
                driver.FindElements(BasketItems).Count > 0 ||
                driver.FindElements(EmptyBasketMessage).Count > 0);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool IsBasketEmpty()
    {
        return _driver.FindElements(EmptyBasketMessage).Count > 0;
    }

    public bool HasItems()
    {
        return _driver.FindElements(BasketItems).Count > 0;
    }

    public int GetItemCount()
    {
        return _driver.FindElements(BasketItems).Count;
    }

    public List<string> GetItemNames()
    {
        var itemElements = _driver.FindElements(ItemName);
        return itemElements.Select(element => element.Text).ToList();
    }

    public bool IsItemInBasket(string itemName)
    {
        var itemNames = GetItemNames();
        return itemNames.Any(name => name.Contains(itemName, StringComparison.OrdinalIgnoreCase));
    }

    public string GetTotalPrice()
    {
        var totalElement = _driver.FindElement(TotalPrice);
        return totalElement?.Text ?? string.Empty;
    }

    // Interaction methods
    public void UpdateQuantity(int itemIndex, int quantity)
    {
        var quantityElements = _driver.FindElements(ItemQuantity);
        if (itemIndex < quantityElements.Count)
        {
            var quantityElement = quantityElements[itemIndex];
            quantityElement.Clear();
            quantityElement.SendKeys(quantity.ToString());
        }
    }

    public void ClickUpdateButton()
    {
        var updateElement = _driver.FindElement(UpdateButton);
        updateElement.Click();
    }

    public void ClickCheckoutButton()
    {
        var checkoutElement = _wait.Until(driver => driver.FindElement(CheckoutButton));
        checkoutElement.Click();
    }

    public string GetCurrentUrl()
    {
        return _driver.Url;
    }

    public string GetPageTitle()
    {
        return _driver.Title;
    }
}