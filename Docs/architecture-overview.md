# Architecture overview

## System shape

eShopOnWeb is a single-process ASP.NET Core application with a separate Public API process used by the Blazor administration client. The solution is organized around a domain-focused core and infrastructure adapters:

```text
Web (Razor Pages, MVC, Blazor host)
        |
        +--> ApplicationCore (entities, services, specifications, interfaces)
        |          ^
        +--> Infrastructure (EF Core, Identity, email, logging)
        |
        +--> PublicApi (HTTP API and endpoint authorization)
                   ^
             BlazorAdmin / BlazorShared
```

The main projects are:

- `ApplicationCore`: business entities, application services, specifications, exceptions, and abstractions.
- `Infrastructure`: SQL Server or in-memory persistence, ASP.NET Core Identity, email, and logging adapters.
- `Web`: the customer-facing Razor Pages/MVC application and server-side Blazor host.
- `PublicApi`: catalog endpoints and JWT authentication for administration operations.
- `BlazorAdmin` and `BlazorShared`: the administration UI and shared contracts.

## Dependency direction

ApplicationCore defines interfaces such as `IRepository<T>`, `IAppLogger<T>`, and `ITokenClaimsService`. Web and PublicApi compose those abstractions through dependency injection; Infrastructure supplies their implementations. This keeps domain and application code from depending directly on EF Core, Identity, or ASP.NET logging.

## Request flow

1. The Web or PublicApi host builds configuration and registers services.
2. Middleware handles transport concerns such as HTTPS, routing, authentication, authorization, health checks, and errors.
3. Endpoints and page handlers invoke application services.
4. Application services use repositories and specifications to access persistence.
5. Infrastructure adapters perform database, Identity, email, or logging operations.

Database seeding runs during startup for both the catalog and Identity contexts. The two contexts are intentionally separate: catalog and commerce data are isolated from user and role data.
