# Authentication and authorization

## Web application

The Web host uses ASP.NET Core cookie authentication and ASP.NET Core Identity backed by `AppIdentityDbContext`. Cookies are configured as HTTP-only, secure, and `SameSite=Lax`. Identity supplies registration, login, password management, external-login support, two-factor authentication, roles, and token providers.

The checkout Razor Page is protected by an endpoint convention in `src/Web/Program.cs`. Controllers and pages can also use the normal ASP.NET Core authorization attributes and policies.

## Public API

The Public API registers the same Identity store but authenticates requests with JWT bearer tokens. Authentication tokens are produced through `IdentityTokenClaimService` and consumed by the JWT bearer middleware. Administrative catalog create, update, and delete endpoints require the administrator role and the JWT bearer scheme.

The API exposes its bearer security definition through Swagger. CORS is restricted to the configured Web base URL rather than allowing arbitrary origins.

## Operational considerations

- Keep signing keys, connection strings, and initial credentials outside source control; use environment configuration or a managed secret store.
- Rotate JWT signing material with an explicit migration strategy for active clients.
- Use HTTPS in every non-local environment.
- Review role assignment and seeded accounts before deploying.
- Treat authentication and authorization as separate checks: a valid identity does not automatically grant administrator access.

Relevant implementation points:

- `src/Web/Program.cs`
- `src/PublicApi/Program.cs`
- `src/Infrastructure/Identity/IdentityTokenClaimService.cs`
- `src/PublicApi/CatalogItemEndpoints/`
