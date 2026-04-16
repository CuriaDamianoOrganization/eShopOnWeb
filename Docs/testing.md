# Testing

## Overview

eShopOnWeb uses a layered testing strategy with four test projects, each targeting a different scope. All test projects live under the `tests/` directory.

```
tests/
├── UnitTests/                    # Fast, isolated tests (xUnit + NSubstitute)
├── IntegrationTests/             # Tests with real dependencies (EF Core in-memory)
├── FunctionalTests/              # End-to-end HTTP tests (WebApplicationFactory)
└── PublicApiIntegrationTests/    # API endpoint tests (WebApplicationFactory + MSTest)
```

## Running Tests

```bash
# Run all unit tests
dotnet test tests/UnitTests/UnitTests.csproj

# Run all tests in the solution
dotnet test eShopOnWeb.sln
```

## Unit Tests

Unit tests validate individual classes and methods in isolation, using **xUnit** as the test framework and **NSubstitute** for mocking dependencies.

### Conventions

- **Namespace** mirrors source: `Microsoft.eShopWeb.UnitTests.ApplicationCore.{Area}.{ClassUnderTest}Tests`
- **One test file per method** (or logical group), named after the method being tested (e.g., `AddItemToBasket.cs`, `SetQuantities.cs`)
- **Test method names** describe the scenario: `ReturnsNotFoundWhenBasketDoesNotExist`, `ThrowsWhenBuyerIdIsNull`
- **Builders** in `tests/UnitTests/Builders/` provide reusable test data factories (`OrderBuilder`, `BasketBuilder`, `AddressBuilder`)

### ApplicationCore Coverage

The following table lists unit test coverage for public classes in the business layer (`src/ApplicationCore/`).

#### Services

| Class | Method | Test File | Tests |
|---|---|---|---|
| `BasketService` | `AddItemToBasket` | `Services/BasketServiceTests/AddItemToBasket.cs` | Repository lookup, repository update |
| `BasketService` | `DeleteBasketAsync` | `Services/BasketServiceTests/DeleteBasket.cs` | Repository delete |
| `BasketService` | `SetQuantities` | `Services/BasketServiceTests/SetQuantities.cs` | NotFound result, quantity updates, zero-quantity removal, unmatched keys, empty dictionary, repository update |
| `BasketService` | `TransferBasketAsync` | `Services/BasketServiceTests/TransferBasket.cs` | Anonymous basket not found, item transfer with existing user basket, anonymous basket deletion, new user basket creation |
| `OrderService` | `CreateOrderAsync` | `Services/OrderServiceTests/CreateOrder.cs` | Null basket throws, empty basket throws `EmptyBasketOnCheckoutException`, correct order item mapping, repository add |
| `UriComposer` | `ComposePicUri` | `Services/UriComposerTests/ComposePicUri.cs` | Placeholder replacement, no-op when no placeholder, empty base URL |

#### Entities

| Class | Method / Property | Test File | Tests |
|---|---|---|---|
| `Basket` | `AddItem` | `Entities/BasketTests/BasketAddItem.cs` | New item added, existing item quantity incremented, original price kept, default quantity of 1, negative quantity rejected |
| `Basket` | `RemoveEmptyItems` | `Entities/BasketTests/BasketRemoveEmptyItems.cs`, `BasketRemoveEmptyItemsExtended.cs` | Zero-quantity items removed, non-empty items retained, mixed scenario, empty basket no-op |
| `Basket` | `TotalItems` | `Entities/BasketTests/BasketTotalItems.cs` | Single item total, multiple items total |
| `Basket` | `SetNewBuyerId` | `Entities/BasketTests/BasketSetNewBuyerId.cs` | Sets new buyer ID, empty string edge case |
| `BasketItem` | `AddQuantity` | `Entities/BasketItemTests/BasketItemAddQuantity.cs` | Increases quantity, zero quantity no-op, negative quantity throws |
| `BasketItem` | `SetQuantity` | `Entities/BasketItemTests/BasketItemSetQuantity.cs` | Sets quantity, zero allowed, negative throws |
| `CatalogItem` | `UpdateDetails` | `Entities/CatalogItemTests/CatalogItemUpdateDetails.cs` | Valid update, null/empty name throws, null/empty description throws, zero/negative price throws |
| `CatalogItem` | `UpdateBrand` | `Entities/CatalogItemTests/CatalogItemUpdateBrand.cs` | Valid update, zero throws |
| `CatalogItem` | `UpdateType` | `Entities/CatalogItemTests/CatalogItemUpdateType.cs` | Valid update, zero throws |
| `CatalogItem` | `UpdatePictureUri` | `Entities/CatalogItemTests/CatalogItemUpdatePictureUri.cs` | Valid name, null sets empty, empty sets empty, name in URI |
| `Order` | Constructor | `Entities/OrderTests/OrderConstructor.cs` | Sets properties, null buyerId throws, empty buyerId throws |
| `Order` | `Total` | `Entities/OrderTests/OrderTotal.cs` | Zero for empty order, single item, multiple items |
| `CatalogItemOrdered` | Constructor | `Entities/CatalogItemOrderedTests/CatalogItemOrderedConstructor.cs` | Sets properties, zero ID throws, null/empty name throws, null/empty URI throws |

#### Specifications

| Specification | Test File | Tests |
|---|---|---|
| `BasketWithItemsSpecification` | `Specifications/BasketWithItemsSpecification.cs` | Match by basket ID, no match for bad ID, match by buyer ID, no match for bad buyer ID |
| `CatalogFilterPaginatedSpecification` | `Specifications/CatalogFilterPaginatedSpecification.cs` | Returns all items, filters by brand and type |
| `CatalogFilterSpecification` | `Specifications/CatalogFilterSpecification.cs` | Parameterized: null/null, brand only, type only, both, no match |
| `CatalogItemsSpecification` | `Specifications/CatalogItemsSpecification.cs` | Single ID match, multiple ID match |
| `CustomerOrdersWithItemsSpecification` | `Specifications/CustomerOrdersWithItemsSpecification.cs` | Returns order with items, includes `ItemOrdered` navigation |

#### Extensions & Constants

| Class | Method | Test File | Tests |
|---|---|---|---|
| `BasketGuards` | `EmptyBasketOnCheckout` | `Extensions/BasketGuardsTests.cs` | Empty list throws `EmptyBasketOnCheckoutException`, non-empty list passes |
| `JsonExtensions` | `FromJson` / `ToJson` | `Extensions/JsonExtensions.cs` | Round-trip serialization, parameterized deserialization |
| `AuthorizationConstants` | `IsValidEmail` | `Constants/AuthorizationConstantsTests.cs` | Valid email, null, empty, whitespace, invalid format, missing domain, subdomain |

### Web Layer Coverage

| Class | Method | Test File | Tests |
|---|---|---|---|
| `CacheHelpers` | `GenerateBrandsCacheKey` | `Web/Extensions/CacheHelpersTests/GenerateBrandsCacheKey.cs` | Returns expected cache key |
| `CacheHelpers` | `GenerateCatalogItemCacheKey` | `Web/Extensions/CacheHelpersTests/GenerateCatalogItemCacheKey.cs` | Returns expected cache key |
| `CacheHelpers` | `GenerateTypesCacheKey` | `Web/Extensions/CacheHelpersTests/GenerateTypesCacheKey.cs` | Returns expected cache key |

### MediatR Handlers

| Handler | Test File |
|---|---|
| `GetMyOrders` | `MediatorHandlers/OrdersTests/GetMyOrders.cs` |
| `GetOrderDetails` | `MediatorHandlers/OrdersTests/GetOrderDetails.cs` |

## Integration Tests

Integration tests verify that components work together with real dependencies (e.g., EF Core in-memory database). Located in `tests/IntegrationTests/`, they use xUnit and NSubstitute.

## Functional Tests

Functional tests exercise the full HTTP pipeline using `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory`. Located in `tests/FunctionalTests/`, they cover end-to-end scenarios for both the Web and PublicApi projects.

## PublicApi Integration Tests

API-level integration tests for the PublicApi project using **MSTest** and `WebApplicationFactory`. Located in `tests/PublicApiIntegrationTests/`.

## Guard Clause Testing

eShopOnWeb uses [Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses) for input validation. When writing tests for guard clauses:

- `Guard.Against.NullOrEmpty` throws `ArgumentNullException` for `null` inputs and `ArgumentException` for empty strings
- `Guard.Against.OutOfRange` throws `ArgumentOutOfRangeException`
- `Guard.Against.Zero` throws `ArgumentException`
- Use `Assert.Throws<T>` with the **exact** exception type (xUnit requires exact match, not base class)

```csharp
// Correct — null triggers ArgumentNullException
Assert.Throws<ArgumentNullException>(() => new Order(null!, address, items));

// Correct — empty triggers ArgumentException
Assert.Throws<ArgumentException>(() => new Order(string.Empty, address, items));
```
