# Exception Management

## Overview

Exception handling is split between the two host projects:

| Host | Strategy |
|---|---|
| `Web` | ASP.NET Core built-in error pages (`UseExceptionHandler` / `UseDeveloperExceptionPage`) |
| `PublicApi` | Custom `ExceptionMiddleware` that converts exceptions to JSON error responses |

---

## Domain Exceptions

Custom exception types live in `ApplicationCore/Exceptions/` so they can be thrown from business logic without any dependency on infrastructure or web frameworks.

### BasketNotFoundException

Thrown when a basket lookup by ID returns no result.

```csharp
public class BasketNotFoundException : Exception
{
    public BasketNotFoundException(int basketId)
        : base($"No basket found with id {basketId}") { }
}
```

### DuplicateException

Thrown when a uniqueness constraint is violated at the application level (e.g., creating a catalog item with a name that already exists).

```csharp
public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message) { }
}
```

### EmptyBasketOnCheckoutException

Thrown when a checkout is attempted on an empty basket.

```csharp
public class EmptyBasketOnCheckoutException : Exception
{
    public EmptyBasketOnCheckoutException()
        : base("Basket cannot have 0 items on checkout") { }

    // Additional constructors for serialization and message overriding
}
```

---

## Guard Clauses

Business services use **Ardalis.GuardClauses** (`Guard.Against.*`) to validate preconditions instead of manual null checks or `if`/`throw` blocks:

```csharp
// OrderService.cs
Guard.Against.Null(basket, nameof(basket));
Guard.Against.EmptyBasketOnCheckout(basket.Items);
```

A custom guard extension (`EmptyBasketOnCheckout`) encapsulates the rule and throws `EmptyBasketOnCheckoutException` when `basket.Items` is empty.

---

## ExceptionMiddleware (PublicApi)

`PublicApi/Middleware/ExceptionMiddleware.cs` wraps the entire request pipeline and intercepts unhandled exceptions before they reach ASP.NET Core's default error handling.

### Flow

```
Request
  │
  ▼
ExceptionMiddleware.InvokeAsync
  │── try
  │     └─► next(httpContext)   ← rest of the pipeline
  └── catch (Exception ex)
        └─► HandleExceptionAsync(httpContext, ex)
```

### Exception-to-Status Mapping

| Exception Type | HTTP Status Code |
|---|---|
| `DuplicateException` | `409 Conflict` |
| Any other `Exception` | `500 Internal Server Error` |

### Response Format

All error responses are serialised as JSON using the `ErrorDetails` class:

```json
{
  "statusCode": 409,
  "message": "A catalogItem with name 'Widget' already exists"
}
```

### Registration

```csharp
// PublicApi/Program.cs
app.UseMiddleware<ExceptionMiddleware>();
```

The middleware is registered **after** `UseDeveloperExceptionPage` (development only) and **before** all other middleware so that exceptions from any layer are caught.

---

## Web Error Handling

The Web host delegates to ASP.NET Core's built-in mechanisms:

```csharp
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseDeveloperExceptionPage();  // full stack trace page
}
else
{
    app.UseExceptionHandler("/Error"); // friendly error page
    app.UseHsts();
}
```

The `/Error` Razor Page is responsible for displaying a user-friendly error message in production.

---

## Extending Exception Handling

To handle a new exception type in the PublicApi:

1. Create the exception class in `ApplicationCore/Exceptions/`.
2. Throw it from a service or endpoint.
3. Add an `else if` branch in `ExceptionMiddleware.HandleExceptionAsync` with the appropriate HTTP status code.

To add a guard clause extension for a new rule:

1. Create a static extension class in `ApplicationCore/Extensions/` or alongside the related guard.
2. Implement `Guard.Against.YourRule(...)` following the Ardalis.GuardClauses extension pattern.
