# Architecture Overview

## Solution Structure

eShopOnWeb is a reference application demonstrating a clean, layered ASP.NET Core architecture. It consists of six projects organized into distinct responsibility layers.

```
eShopOnWeb.sln
├── src/
│   ├── ApplicationCore/     # Domain & business logic
│   ├── Infrastructure/      # Data access & external services
│   ├── Web/                 # MVC/Razor Pages front-end
│   ├── PublicApi/           # REST API (Minimal API endpoints)
│   ├── BlazorAdmin/         # Blazor WebAssembly admin client
│   └── BlazorShared/        # Shared models & constants
└── tests/
    ├── UnitTests/
    ├── IntegrationTests/
    ├── FunctionalTests/
    └── PublicApiIntegrationTests/
```

## Layers

### ApplicationCore
The innermost layer; has no dependencies on other projects in the solution. Contains:
- **Entities** — Domain models (`CatalogItem`, `Order`, `Basket`, etc.) organized as aggregate roots
- **Interfaces** — Abstractions for repositories, services, and logging
- **Services** — Business logic (`BasketService`, `OrderService`)
- **Specifications** — Query encapsulations using the Specification pattern
- **Exceptions** — Domain-specific exception types
- **Constants** — Shared constants such as `AuthorizationConstants`

### Infrastructure
Implements the interfaces defined in ApplicationCore. Depends only on ApplicationCore. Contains:
- **Data** — EF Core `DbContext` classes, `EfRepository`, migrations, query services, and seed data
- **Identity** — ASP.NET Core Identity setup, `ApplicationUser`, token claims service
- **Logging** — `LoggerAdapter<T>` wrapping `ILogger<T>`
- **Services** — External service integrations (e.g., `EmailSender`)

### Web
ASP.NET Core MVC/Razor Pages host. Depends on ApplicationCore and Infrastructure. Contains:
- Razor Pages for the storefront experience
- MVC controllers for API calls from the Blazor admin
- Configuration classes for cookie policy, health checks, etc.
- MediatR feature handlers (CQRS for queries)
- Caching decorators over view-model services

### PublicApi
ASP.NET Core Minimal API host. Depends on ApplicationCore and Infrastructure. Exposes a JSON REST API consumed by the Blazor admin client. Authentication uses **JWT Bearer tokens**.

### BlazorAdmin
Blazor WebAssembly client for catalog management. Authenticates via JWT obtained from PublicApi. Uses a custom `AuthenticationStateProvider` that caches the current user identity.

### BlazorShared
A class library shared between BlazorAdmin and Web. Contains shared DTOs, authorization constants, and shared attribute types.

## Key Design Patterns

| Pattern | Location | Purpose |
|---|---|---|
| Repository | `Infrastructure/Data/EfRepository.cs` | Abstracts data access behind generic interfaces |
| Specification | `ApplicationCore/Specifications/` | Encapsulates query logic as reusable objects |
| Aggregate Root | `Entities/BasketAggregate/`, `Entities/OrderAggregate/` | Enforces consistency boundaries |
| Decorator | `Web/Services/CachedCatalogViewModelService.cs` | Adds caching to view-model service |
| Adapter | `Infrastructure/Logging/LoggerAdapter.cs` | Decouples domain from framework logging |
| CQRS (MediatR) | `Web/Features/` | Separates read queries from write commands in the Web layer |
| Endpoint-per-feature | `PublicApi/*Endpoints/` | One class per API operation via Ardalis.ApiEndpoints |

## Dependency Flow

```
Web / PublicApi / BlazorAdmin
        │
        ▼
  ApplicationCore  ◄───  Infrastructure
```

ApplicationCore never references Infrastructure or the host projects. All wiring is done via constructor injection and registered in each host's `Program.cs` and `Infrastructure.Dependencies.ConfigureServices()`.

## Infrastructure Registration

`src/Infrastructure/Dependencies.cs` centralises EF Core context registration. The application supports two database modes:
- **SQL Server** — default production mode via connection strings `CatalogConnection` and `IdentityConnection`
- **In-Memory** — enabled when `UseOnlyInMemoryDatabase` configuration key is set to `true`; used in tests

## Health Checks

Both the Web and PublicApi hosts expose health check endpoints:
- `/health` — returns JSON with overall status and per-check status
- `home_page_health_check` — tagged `homePageHealthCheck`
- `api_health_check` — tagged `apiHealthCheck`
