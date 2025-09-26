# Selenium UI Tests for eShopOnWeb

This project contains comprehensive end-to-end UI tests using Selenium WebDriver for the eShopOnWeb application.

## Overview

The Selenium UI tests cover the following functionality:
- **Home Page**: Product catalog display, navigation, filtering
- **Authentication**: User login/logout flows
- **Shopping Basket**: Adding products, quantity updates, item removal
- **Checkout**: Purchase flow and payment processing
- **End-to-End**: Complete user workflows from browsing to checkout

## Project Structure

```
SeleniumUITests/
├── Tests/                          # Test classes
│   ├── HomePageTests.cs           # Home page functionality tests
│   ├── AuthenticationTests.cs     # Login/logout tests
│   ├── BasketTests.cs            # Shopping basket tests
│   ├── CheckoutTests.cs          # Checkout flow tests
│   └── EndToEndTests.cs          # Complete workflow tests
├── PageObjects/                   # Page Object Model classes
│   ├── HomePage.cs               # Home page interactions
│   ├── LoginPage.cs              # Login page interactions
│   ├── BasketPage.cs             # Basket page interactions
│   └── CheckoutPage.cs           # Checkout page interactions
├── BaseSeleniumTest.cs           # Base test class with WebDriver setup
├── appsettings.json              # Configuration settings
└── SeleniumUITests.csproj        # Project file
```

## Prerequisites

1. **.NET 8.0 SDK** or later
2. **Google Chrome** browser installed
3. **ChromeDriver** (automatically managed by Selenium.WebDriver.ChromeDriver package)
4. **Running eShopOnWeb application** (the tests expect the app to be running)

## Configuration

The tests can be configured via `appsettings.json`:

```json
{
  "BaseUrl": "https://localhost:5001",
  "Selenium": {
    "Headless": "true",
    "ImplicitWaitSeconds": "10",
    "ExplicitWaitSeconds": "15"
  },
  "TestUsers": {
    "DefaultUser": {
      "Email": "demouser@microsoft.com",
      "Password": "Pass@word1"
    }
  }
}
```

### Configuration Options

- **BaseUrl**: The URL where the eShopOnWeb application is running
- **Selenium.Headless**: Set to "true" to run tests in headless mode (recommended for CI/CD)
- **TestUsers.DefaultUser**: Credentials for the demo user account

## Running the Tests

### Prerequisites - Start the Application

Before running UI tests, ensure the eShopOnWeb application is running:

```bash
# Navigate to the Web project
cd src/Web

# Run the application
dotnet run
```

The application should be accessible at `https://localhost:5001` (or the URL specified in BaseUrl).

### Run All UI Tests

```bash
# From the repository root
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj
```

### Run Specific Test Classes

```bash
# Run only home page tests
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj --filter "ClassName=Microsoft.eShopWeb.SeleniumUITests.Tests.HomePageTests"

# Run only authentication tests
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj --filter "ClassName=Microsoft.eShopWeb.SeleniumUITests.Tests.AuthenticationTests"

# Run only basket tests
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj --filter "ClassName=Microsoft.eShopWeb.SeleniumUITests.Tests.BasketTests"
```

### Run Specific Tests

```bash
# Run a specific test method
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj --filter "Name=HomePage_ShouldLoad_Successfully"
```

### Environment Variables

You can override configuration using environment variables:

```bash
# Run in non-headless mode for debugging
export BaseUrl="https://localhost:5001"
export Selenium__Headless="false"
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj
```

## Test Categories

### 1. Home Page Tests (`HomePageTests.cs`)
- Verifies home page loads successfully
- Tests product catalog display
- Validates navigation elements
- Tests adding products to basket
- Verifies filtering functionality

### 2. Authentication Tests (`AuthenticationTests.cs`)
- Tests login page functionality
- Verifies successful login with valid credentials
- Tests error handling for invalid credentials
- Validates navigation between home and login pages
- Tests "Remember Me" functionality

### 3. Basket Tests (`BasketTests.cs`)
- Verifies basket page loads
- Tests adding products from home page
- Validates quantity updates
- Tests item removal
- Verifies navigation to checkout

### 4. Checkout Tests (`CheckoutTests.cs`)
- Tests checkout page functionality
- Verifies authentication requirements
- Tests complete checkout flow
- Validates payment processing

### 5. End-to-End Tests (`EndToEndTests.cs`)
- Complete purchase workflows
- Multi-product shopping scenarios
- Navigation flow testing
- Product filtering scenarios

## Page Object Model

The tests use the Page Object Model pattern for maintainable and reusable code:

- **BaseSeleniumTest**: Provides WebDriver setup and common utilities
- **HomePage**: Encapsulates home page interactions
- **LoginPage**: Handles login form interactions
- **BasketPage**: Manages shopping basket operations
- **CheckoutPage**: Controls checkout flow interactions

## Browser Configuration

The tests are configured to run with Chrome WebDriver with the following options:
- `--no-sandbox`: Required for running in containerized environments
- `--disable-dev-shm-usage`: Prevents /dev/shm issues
- `--disable-gpu`: Disables GPU acceleration
- `--window-size=1920,1080`: Sets consistent window size
- `--headless`: Runs without UI (configurable)

## Troubleshooting

### Common Issues

1. **Application Not Running**: Ensure eShopOnWeb is running at the configured BaseUrl
2. **ChromeDriver Issues**: The ChromeDriver is automatically managed, but ensure Chrome browser is installed
3. **Timeout Issues**: Increase wait times in configuration if tests are flaky
4. **Port Conflicts**: Verify the BaseUrl matches where your application is running

### Debugging Tests

To debug tests, set headless mode to false:
```json
{
  "Selenium": {
    "Headless": "false"
  }
}
```

### Viewing Test Results

The tests output standard xUnit results. For detailed reporting, you can use:

```bash
dotnet test tests/SeleniumUITests/SeleniumUITests.csproj --logger "trx;LogFileName=selenium-results.trx"
```

## CI/CD Integration

These tests are designed to run in CI/CD environments:
- Headless mode is enabled by default
- All required dependencies are included in the project
- Tests are self-contained and don't require external setup beyond a running application

Example GitHub Actions integration:
```yaml
- name: Start Application
  run: |
    cd src/Web
    dotnet run &
    sleep 30  # Wait for app to start

- name: Run Selenium UI Tests
  run: dotnet test tests/SeleniumUITests/SeleniumUITests.csproj
```

## Contributing

When adding new tests:
1. Follow the existing Page Object Model pattern
2. Add new page objects for new pages
3. Use descriptive test names following the pattern: `[Page]_[Action]_[ExpectedResult]`
4. Include proper assertions and wait conditions
5. Consider both positive and negative test scenarios