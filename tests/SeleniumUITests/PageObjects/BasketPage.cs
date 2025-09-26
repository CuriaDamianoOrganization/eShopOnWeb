using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.eShopWeb.SeleniumUITests.PageObjects;

public class BasketPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Locators
    private readonly By _basketItems = By.CssSelector(".esh-basket-item");
    private readonly By _itemNames = By.CssSelector(".esh-basket-item-name");
    private readonly By _itemPrices = By.CssSelector(".esh-basket-item-price");
    private readonly By _quantityInputs = By.CssSelector(".esh-basket-input");
    private readonly By _updateButtons = By.CssSelector(".btn-update");
    private readonly By _checkoutButton = By.CssSelector(".btn-checkout");
    private readonly By _continueShoppingButton = By.LinkText("Continue Shopping");
    private readonly By _totalPrice = By.CssSelector(".esh-basket-total");
    private readonly By _emptyBasketMessage = By.CssSelector(".esh-basket-empty");

    public BasketPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void NavigateTo(string baseUrl)
    {
        _driver.Navigate().GoToUrl($"{baseUrl}/Basket");
    }

    public bool IsLoaded()
    {
        try
        {
            // Page is loaded if we can find either basket items or empty basket message
            _wait.Until(driver => 
                driver.FindElements(_basketItems).Count > 0 || 
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
            return _driver.FindElement(_emptyBasketMessage).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public int GetBasketItemCount()
    {
        return _driver.FindElements(_basketItems).Count;
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

    public void UpdateQuantity(int itemIndex, int quantity)
    {
        var quantityInputs = _driver.FindElements(_quantityInputs);
        if (itemIndex < quantityInputs.Count)
        {
            var quantityInput = quantityInputs[itemIndex];
            quantityInput.Clear();
            quantityInput.SendKeys(quantity.ToString());
            
            var updateButtons = _driver.FindElements(_updateButtons);
            if (itemIndex < updateButtons.Count)
            {
                updateButtons[itemIndex].Click();
            }
        }
        else
        {
            throw new ArgumentException($"Item index {itemIndex} out of range.");
        }
    }

    public void RemoveItem(int itemIndex)
    {
        UpdateQuantity(itemIndex, 0);
    }

    public void ClickCheckout()
    {
        var checkoutButton = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_checkoutButton);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        checkoutButton.Click();
    }

    public void ClickContinueShopping()
    {
        var continueButton = _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_continueShoppingButton);
                return element.Displayed && element.Enabled ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
        continueButton.Click();
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

    public bool ContainsProduct(string productName)
    {
        var itemNames = GetItemNames();
        return itemNames.Any(name => name.Contains(productName, StringComparison.OrdinalIgnoreCase));
    }

    public int GetQuantityForItem(int itemIndex)
    {
        var quantityInputs = _driver.FindElements(_quantityInputs);
        if (itemIndex < quantityInputs.Count)
        {
            var quantityValue = quantityInputs[itemIndex].GetAttribute("value");
            return int.TryParse(quantityValue, out int quantity) ? quantity : 0;
        }
        return 0;
    }
}