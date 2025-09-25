# eShopOnWeb Selenium End-to-End Tests

This directory contains Selenium-based end-to-end tests for the eShopOnWeb application.

## Overview

The Selenium test project provides browser-based automation testing capabilities to validate user interactions and end-to-end scenarios in the eShopOnWeb application.

## Project Structure

```
tests/SeleniumTests/
├── Infrastructure/
│   └── SeleniumTestBase.cs          # Base class for all Selenium tests
├── PageObjects/
│   ├── HomePage.cs                  # Page Object Model for home page
│   ├── LoginPage.cs                 # Page Object Model for login page
│   └── BasketPage.cs                # Page Object Model for basket/cart page
├── Tests/
│   ├── HomePageTests.cs             # Tests for home page functionality
│   ├── LoginTests.cs                # Tests for authentication flows
│   ├── BasketTests.cs               # Tests for shopping cart functionality
│   └── SeleniumDemoTests.cs         # Demonstration tests for Selenium framework
└── SeleniumTests.csproj            # Project configuration file
```

## Dependencies

The project uses the following NuGet packages:
- **Selenium.WebDriver** (4.16.2) - Core WebDriver functionality
- **Selenium.WebDriver.ChromeDriver** (120.0.6099.10900) - Chrome browser driver
- **Selenium.Support** (4.16.2) - Additional Selenium utilities (Select elements, etc.)
- **xUnit** - Test framework
- **Microsoft.NET.Test.Sdk** - .NET test SDK

## Test Categories

### 1. Home Page Tests (`HomePageTests.cs`)
- Verify basic page loading functionality
- Test navigation capabilities
- Validate HTML structure

### 2. Login Tests (`LoginTests.cs`)
- Test login page loading
- Validate form structure
- Test form interactions

### 3. Basket Tests (`BasketTests.cs`)
- Test basket/cart page loading
- Validate empty basket handling
- Test JavaScript functionality

### 4. Selenium Demo Tests (`SeleniumDemoTests.cs`)
- Verify Selenium WebDriver setup
- Test basic browser automation
- Validate JavaScript execution
- Test form interactions on static content

## Configuration

The tests are configured to run in **headless mode** by default for CI/CD compatibility. The browser configuration is set in `SeleniumTestBase.cs`:

```csharp
var chromeOptions = new ChromeOptions();
chromeOptions.AddArguments(
    "--headless",              // Run in headless mode
    "--no-sandbox",            // Required for containers
    "--disable-dev-shm-usage", // Resource optimization
    "--disable-gpu",           // Disable GPU acceleration
    "--window-size=1920,1080"  // Set browser window size
);
```

## Running Tests

### Prerequisites
- .NET 8.0 SDK
- Chrome browser (automatically managed by ChromeDriver)

### Commands

```bash
# Build the test project
dotnet build tests/SeleniumTests/SeleniumTests.csproj

# Run all Selenium tests
dotnet test tests/SeleniumTests/SeleniumTests.csproj

# Run only demo tests (recommended for framework verification)
dotnet test tests/SeleniumTests/SeleniumTests.csproj --filter "SeleniumDemoTests"

# Run tests with verbose output
dotnet test tests/SeleniumTests/SeleniumTests.csproj --verbosity normal

# List available tests
dotnet test tests/SeleniumTests/SeleniumTests.csproj --list-tests
```

## Page Object Model Pattern

The tests follow the **Page Object Model (POM)** design pattern to:
- Encapsulate page-specific functionality
- Improve test maintainability
- Reduce code duplication
- Provide clear separation between test logic and page interactions

Example usage:
```csharp
var homePage = new HomePage(Driver);
homePage.Navigate(baseUrl);
Assert.True(homePage.HasProducts());
```

## Test Strategy

### Graceful Failure Handling
All tests are designed to handle scenarios where:
- The target application is not running
- Network connectivity issues exist
- Browser initialization fails

Tests use try-catch blocks to skip gracefully when the application is unavailable, making them suitable for various environments.

### Base URL Configuration
Tests use a configurable base URL (default: `https://localhost:5001`) that can be adjusted for different environments.

## CI/CD Integration

The tests are designed to run in containerized environments and CI/CD pipelines:
- Headless Chrome configuration
- No-sandbox mode for container compatibility
- Graceful handling of missing dependencies
- Timeout configurations for reliable execution

## Future Enhancements

Potential improvements for this test suite:
1. **Environment Configuration**: Add support for multiple environment URLs
2. **Data-Driven Tests**: Implement parameterized tests with test data
3. **Visual Regression Testing**: Add screenshot comparison capabilities
4. **Performance Testing**: Include page load time validations
5. **Cross-Browser Testing**: Extend support to Firefox, Edge, Safari
6. **Test Reporting**: Integrate with reporting frameworks
7. **Parallel Execution**: Configure tests for parallel execution

## Contributing

When adding new tests:
1. Follow the Page Object Model pattern
2. Inherit from `SeleniumTestBase` for consistency
3. Use descriptive test names that explain the scenario
4. Include proper error handling and graceful failure
5. Add appropriate assertions and validation
6. Update this README for significant additions

## Troubleshooting

### Common Issues

**Chrome not found**: Ensure Chrome is installed or update ChromeDriver version
**Connection timeout**: Verify the application is running on the expected URL
**Element not found**: Check page load timing and add appropriate waits
**Permission errors**: Ensure proper permissions for browser executable

### Debug Mode
To debug tests, temporarily remove the `--headless` argument in `SeleniumTestBase.cs` to see the browser in action.