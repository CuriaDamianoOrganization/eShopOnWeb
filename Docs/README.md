# eShopOnWeb — Architectural Documentation

| Document | Description |
|---|---|
| [Architecture Overview](architecture-overview.md) | Solution structure, layers, design patterns, and dependency flow |
| [Authentication & Authorization](authentication-authorization.md) | Cookie auth, JWT Bearer, roles, token revocation, Blazor auth state |
| [Exception Management](exception-management.md) | Domain exceptions, guard clauses, ExceptionMiddleware, error responses |
| [Logging](logging.md) | IAppLogger abstraction, LoggerAdapter, providers, startup logging |
| [Middleware Pipelines](middleware-pipelines.md) | Ordered middleware for Web and PublicApi, CORS, health checks |
| [Data Access](data-access.md) | Repository pattern, Specification pattern, EF Core contexts, seeding |
| [API Design](api-design.md) | Endpoint-per-feature pattern, request/response conventions, Swagger, CQRS |
