# Logging

## Overview

eShopOnWeb follows the **Adapter** pattern to insulate business logic from any dependency on ASP.NET Core's `Microsoft.Extensions.Logging` framework. The `ApplicationCore` layer depends only on the `IAppLogger<T>` abstraction; the concrete binding to `ILogger<T>` lives in `Infrastructure`.

---

## IAppLogger\<T\>

`ApplicationCore/Interfaces/IAppLogger.cs`

```csharp
/// <summary>
/// This type eliminates the need to depend directly on the ASP.NET Core logging types.
/// </summary>
public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
}
```

Only two log levels are currently exposed. If additional levels (Debug, Error, Critical) are required, this interface and its adapter should be extended accordingly.

---

## LoggerAdapter\<T\>

`Infrastructure/Logging/LoggerAdapter.cs`

```csharp
public class LoggerAdapter<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
    }

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);

    public void LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);
}
```

The adapter is registered as a generic open-type service so any class in ApplicationCore can request `IAppLogger<T>` without additional registration:

```csharp
services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
```

---

## Providers & Configuration

Both host projects add the console provider and rely on the default `appsettings.json` / `appsettings.{Environment}.json` configuration for log levels:

```csharp
// Web/Program.cs and PublicApi/Program.cs
builder.Logging.AddConsole();
```

Standard ASP.NET Core log-level filtering applies:

```json
// appsettings.json (default)
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

---

## Application Startup Logging

`Web/Program.cs` emits structured log messages at key startup milestones using the host's `app.Logger`:

```csharp
app.Logger.LogInformation("App created...");
app.Logger.LogInformation("Seeding Database...");
app.Logger.LogInformation("Adding Development middleware...");
app.Logger.LogInformation("LAUNCHING");
```

These messages are visible in the console output and help diagnose startup failures in containers or cloud environments.

---

## Usage in Services

Services inject `IAppLogger<T>` through the constructor:

```csharp
// BasketService.cs
public class BasketService : IBasketService
{
    private readonly IAppLogger<BasketService> _logger;

    public BasketService(IRepository<Basket> basketRepository, IAppLogger<BasketService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<Basket>> SetQuantities(int basketId, Dictionary<string, int> quantities)
    {
        // ...
        _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
        // ...
    }
}
```

The Blazor admin's `CustomAuthStateProvider` uses the standard `ILogger<T>` directly (acceptable since it lives in the host project):

```csharp
_logger.LogInformation("Fetching user details from web api.");
_logger.LogWarning(exc, "Fetching user failed.");
```

---

## Extending Logging

### Adding a new log level

1. Add the method signature to `IAppLogger<T>`.
2. Implement it in `LoggerAdapter<T>` by delegating to the corresponding `ILogger` method.

### Adding a structured logging provider (e.g., Serilog)

1. Install the Serilog ASP.NET Core package.
2. Replace or supplement `builder.Logging.AddConsole()` in the host's `Program.cs`.
3. No changes to `ApplicationCore` or `LoggerAdapter` are required.

### Correlation IDs

API requests include a `CorrelationId` on request/response objects (from `BaseRequest`). To propagate this ID through log messages, consider enriching the log scope in middleware:

```csharp
using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
{
    await _next(context);
}
```
