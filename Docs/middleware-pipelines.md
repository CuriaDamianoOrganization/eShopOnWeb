# Middleware Pipelines

## Overview

Both host projects (`Web` and `PublicApi`) build an ASP.NET Core middleware pipeline in `Program.cs`. The order of middleware registration is critical; each entry below is listed in registration order.

---

## Web Middleware Pipeline

```
Request
  │
  ▼
1.  HealthChecks (/health)
  │
  ▼
2.  DeveloperExceptionPage      ← Development / Docker only
    OR
    ExceptionHandler (/Error)   ← Production
    + HSTS
  │
  ▼
3.  ShowAllServicesMiddleware   ← Development only
  │
  ▼
4.  MigrationsEndPoint          ← Development only
  │
  ▼
5.  HttpsRedirection
  │
  ▼
6.  BlazorFrameworkFiles (serves _framework/*)
  │
  ▼
7.  StaticFiles
  │
  ▼
8.  Routing
  │
  ▼
9.  CookiePolicy
  │
  ▼
10. Authentication
  │
  ▼
11. Authorization
  │
  ▼
12. Endpoints
    ├── MapControllerRoute ("default")
    ├── MapRazorPages
    ├── MapHealthChecks ("home_page_health_check")
    ├── MapHealthChecks ("api_health_check")
    └── MapFallbackToFile ("index.html")  ← for Blazor SPA
```

### Health Checks

The `/health` endpoint returns a JSON object:

```json
{
  "status": "Healthy",
  "errors": [
    { "key": "self", "value": "Healthy" }
  ]
}
```

Two additional tagged health check endpoints are mapped for selective probing:
- `home_page_health_check` — tag `homePageHealthCheck`
- `api_health_check` — tag `apiHealthCheck`

### Environment Branching

| Environment | Exception Handling | Extra Middleware |
|---|---|---|
| Development / Docker | `UseDeveloperExceptionPage` | `ShowAllServicesMiddleware`, `UseMigrationsEndPoint`, `UseWebAssemblyDebugging` |
| Production | `UseExceptionHandler("/Error")` + `UseHsts` | — |

### Slug Routing

Controller routes use `{controller:slugify=Home}/{action:slugify=Index}/{id?}` via `SlugifyParameterTransformer`, which converts PascalCase route segments to kebab-case URLs (e.g., `CatalogItem` → `catalog-item`).

---

## PublicApi Middleware Pipeline

```
Request
  │
  ▼
1.  DeveloperExceptionPage  ← Development only
  │
  ▼
2.  ExceptionMiddleware     ← catches all unhandled exceptions → JSON error
  │
  ▼
3.  HttpsRedirection
  │
  ▼
4.  Routing
  │
  ▼
5.  CORS (CorsPolicy)
  │
  ▼
6.  Authorization
  │
  ▼
7.  Swagger / SwaggerUI
  │
  ▼
8.  Endpoints
    ├── MapControllers
    └── MapEndpoints      ← Ardalis minimal API endpoints
```

### CORS Policy

Named policy `CorsPolicy` allows requests from the Web host's base URL only:

```csharp
corsPolicyBuilder.WithOrigins(baseUrlConfig.WebBase.TrimEnd('/'));
corsPolicyBuilder.AllowAnyMethod();
corsPolicyBuilder.AllowAnyHeader();
```

The origin URL is read from `BaseUrlConfiguration.WebBase` (set via `appsettings.json`). `host.docker.internal` is normalised to `localhost` so that the Docker and local development origins match.

### Swagger

Swagger is available at all times in PublicApi (not only in development). The JSON spec is served at `/swagger/v1/swagger.json` and the interactive UI at `/swagger`.

---

## Custom Middleware

### ExceptionMiddleware

Registered in PublicApi only. See [Exception Management](exception-management.md) for full details.

### RevokeAuthenticationEvents

Registered as a `CookieAuthenticationEvents` handler in the Web host. Hooks `ValidatePrincipal` to support server-side cookie revocation. See [Authentication and Authorization](authentication-authorization.md) for full details.

### ShowAllServicesMiddleware

A development-only diagnostic middleware (from `Ardalis.ListStartupServices`) that exposes a `/allservices` endpoint listing all registered DI services. Registered only when the environment is Development or Docker.

---

## Middleware Ordering Rules

The following ordering constraints are enforced:

1. **Exception handling must be first** so it wraps all subsequent middleware.
2. **Routing must precede Authorization** — `UseRouting` sets the matched endpoint; `UseAuthorization` reads it.
3. **Authentication must precede Authorization** — the user identity must be established before policy evaluation.
4. **CookiePolicy must precede Authentication** — cookie settings are applied before the authentication handler reads cookies.
5. **StaticFiles must precede Routing** — static file requests should be short-circuited before route matching.
6. **CORS must precede Routing** in PublicApi to ensure preflight responses are handled before the routing pipeline processes the request.
