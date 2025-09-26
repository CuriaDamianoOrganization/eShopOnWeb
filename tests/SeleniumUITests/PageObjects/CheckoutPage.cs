using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumUITests.PageObjects;

public class CheckoutPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Locators
    private readonly By _checkoutItems = By.CssSelector(".esh-basket-item");
    private readonly By _itemNames = By.CssSelector(".esh-basket-item-name");
    private readonly By _itemPrices = By.CssSelector(".esh-basket-item-price");
    private readonly By _itemQuantities = By.CssSelector(".esh-basket-item-quantity");
    private readonly By _totalPrice = By.CssSelector(".esh-basket-total");
    private readonly By _payNowButton = By.CssSelector("input[value='[ Pay Now ]']");
    private readonly By _backButton = By.LinkText("[ Back ]");
    private readonly By _orderSummaryTitle = By.TagName("h1");
    private readonly By _emptyBasketMessage = By.CssSelector(".esh-catalog-items h3");

    public CheckoutPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void NavigateTo(string baseUrl)
    {
        _driver.Navigate().GoToUrl($"{baseUrl}/Basket/Checkout");
    }

    public bool IsLoaded()
    {
        try
        {
            _wait.Until(driver => 
                driver.FindElement(_orderSummaryTitle).Text.Contains("Review") ||
                driver.FindElements(_emptyBasketMessage).Count > 0);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool IsBasketEmpty()
    {
        try
        {
            var emptyMessage = _driver.FindElement(_emptyBasketMessage);
            return emptyMessage.Text.Contains("Basket is empty");
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public int GetCheckoutItemCount()
    {
        return _driver.FindElements(_checkoutItems).Count;
    }

    public IList<string> GetItemNames()
    {
        return _driver.FindElements(_itemNames)
                     .Select(element => element.Text)
                     .ToList();
    }

    public IList<string> GetItemPrices()
    {
        return _driver.FindElements(_itemPrices)
                     .Select(element => element.Text)
                     .ToList();
    }

    public IList<string> GetItemQuantities()
    {
        return _driver.FindElements(_itemQuantities)
                     .Select(element => element.Text)
                     .ToList();
    }

    public string GetTotalPrice()
    {
        try
        {
            var totalElement = _driver.FindElement(_totalPrice);
            return totalElement.Text;
        }
        catch (NoSuchElementException)
        {
            return "0";
        }
    }

    public void ClickPayNow()
    {
        var payNowButton = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_payNowButton);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        payNowButton.Click();
    }

    public void ClickBack()
    {
        var backButton = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_backButton);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        backButton.Click();
    }

    public bool ContainsProduct(string productName)
    {
        var itemNames = GetItemNames();
        return itemNames.Any(name => name.Contains(productName, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsPayNowButtonVisible()
    {
        try
        {
            var payButton = _driver.FindElement(_payNowButton);
            return payButton.Displayed && payButton.Enabled;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public string GetPageTitle()
    {
        try
        {
            return _driver.FindElement(_orderSummaryTitle).Text;
        }
        catch (NoSuchElementException)
        {
            return string.Empty;
        }
    }
}