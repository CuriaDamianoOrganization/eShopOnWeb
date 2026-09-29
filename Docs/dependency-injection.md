# Dependency injection

eShopOnWeb uses the ASP.NET Core built-in dependency injection container. Each host builds its service collection during startup, while the application core depends on interfaces and Infrastructure supplies concrete implementations.

## Composition roots

- `src/Web/Program.cs` registers the Web host, Identity, authentication, health checks, and UI services.
- `src/PublicApi/Program.cs` registers API endpoints, Identity, JWT authentication, Swagger, and API services.
- `src/Infrastructure/Dependencies.cs` registers `CatalogContext` and `AppIdentityDbContext`, selecting SQL Server or separate in-memory databases from configuration.
- `src/Web/Configuration/ConfigureCoreServices.cs` registers repositories, basket and order services, URI composition, logging, and email.
- `src/Web/Configuration/ConfigureWebServices.cs` registers MediatR handlers, view-model services, catalog services, and caching.

The Web and PublicApi hosts share Infrastructure registrations but compose their transport-specific dependencies independently. This allows the API and customer-facing application to use the same domain and persistence boundaries without sharing an HTTP pipeline.

## Lifetimes

Registrations follow the lifetime of the work they perform:

- **Scoped:** EF Core repositories and contexts, application services, view-model services, and `IAppLogger<T>`. A scope normally corresponds to an HTTP request.
- **Transient:** `IEmailSender`, which has no request-level state.
- **Singleton:** `IUriComposer`, constructed from catalog configuration, and the framework services that are safe to share.

When adding a registration, choose the shortest lifetime that satisfies the dependency's state and resource requirements. A singleton must not capture a scoped service, and a transient or scoped service should not retain request data beyond its scope.

## Abstraction boundaries

ApplicationCore defines interfaces such as `IRepository<T>`, `IReadRepository<T>`, `IBasketService`, `IOrderService`, `IAppLogger<T>`, and `ITokenClaimsService`. Implementations are registered at the host boundary, so application services can be unit tested with substitutes without constructing a database or web server.

## Adding a service

1. Define the contract in the layer that owns the behavior.
2. Implement it in ApplicationCore or Infrastructure, depending on whether it is business logic or an external adapter.
3. Register it in the appropriate composition-root extension method.
4. Select a lifetime based on state and dependencies.
5. Add unit coverage for the behavior and integration coverage when the registration or external adapter is significant.

Avoid resolving services directly from `IServiceProvider` in application code. Prefer constructor injection so dependencies are explicit and missing registrations fail when the consuming type is activated.
