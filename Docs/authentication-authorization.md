# Authentication and Authorization

## Overview

eShopOnWeb uses two distinct authentication mechanisms depending on the host:

| Host | Mechanism | Used By |
|---|---|---|
| `Web` | Cookie-based (ASP.NET Core Identity) | Browser storefront |
| `PublicApi` | JWT Bearer tokens | Blazor admin client |

---

## Cookie-Based Authentication (Web)

### Identity Setup

ASP.NET Core Identity is registered in `Web/Program.cs`:

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();
```

`ApplicationUser` extends `IdentityUser` without additional properties; the Identity store is an EF Core `AppIdentityDbContext` backed by SQL Server (or in-memory for tests).

### Cookie Settings

`Web/Configuration/ConfigureCookieSettings.cs` applies strict cookie security:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });
```

The `ConfigureCookieSettings` class further tightens the policy:
- `SameSiteMode.Strict`
- 60-minute sliding expiration via `ValidityPeriod`
- Custom login and logout paths

An identifier cookie (`EshopIdentifier`) is used together with a memory cache to support **server-side token revocation** (see below).

### Token Revocation

`RevokeAuthenticationEvents` extends `CookieAuthenticationEvents` and hooks into `ValidatePrincipal`:

1. On every authenticated request, the event handler reads the user's name claim and the identifier cookie.
2. It checks `IMemoryCache` for a revocation entry keyed as `"{userId}:{identityKey}"`.
3. If found, it calls `context.RejectPrincipal()` and signs the user out immediately.

This allows administrators to invalidate active sessions without waiting for cookie expiry.

---

## JWT Bearer Authentication (PublicApi)

### Configuration

`PublicApi/Program.cs` registers JWT bearer authentication:

```csharp
var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);

builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
```

> **Note:** `RequireHttpsMetadata = false` is acceptable for local development but should be `true` in production.

### Secret Key

The HMAC-SHA256 signing key is stored in `ApplicationCore/Constants/AuthorizationConstants`:

```
JWT_SECRET_KEY = "SecretKeyOfDoomThatMustBeAMinimumNumberOfBytes"
```

In production this value must be replaced with a secret stored outside source control (e.g., Azure Key Vault or environment variable).

### Token Generation

`Infrastructure/Identity/IdentityTokenClaimService.cs` implements `ITokenClaimsService`:

1. Looks up the user by username via `UserManager<ApplicationUser>`.
2. Fetches all roles assigned to the user.
3. Builds a `ClaimsIdentity` containing `ClaimTypes.Name` and one `ClaimTypes.Role` claim per role.
4. Issues a `SecurityTokenDescriptor` with a 7-day expiration, signed with the symmetric key.
5. Returns the serialised JWT string.

### Authentication Endpoint

`PublicApi/AuthEndpoints/AuthenticateEndpoint.cs` exposes `POST /api/authenticate`:

1. Calls `SignInManager.PasswordSignInAsync` to validate credentials.
2. On success, calls `ITokenClaimsService.GetTokenAsync` to obtain a JWT.
3. Returns `AuthenticateResponse` with the token and sign-in result flags (`IsLockedOut`, `RequiresTwoFactor`, etc.).

---

## Authorization

### Roles

A single role is defined in `BlazorShared/Authorization/Constants`:

```csharp
public static class Roles
{
    public const string ADMINISTRATORS = "Administrators";
}
```

### Protected Endpoints

Write operations in the PublicApi (create, update, delete catalog items) require the `ADMINISTRATORS` role via the JWT scheme:

```csharp
[Authorize(
    Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS,
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
```

Read-only endpoints (list, get by id) are unauthenticated.

Blazor admin pages are similarly guarded with `[Authorize(Roles = Constants.Roles.ADMINISTRATORS)]`.

---

## Blazor Client Authentication State

`BlazorAdmin/CustomAuthStateProvider.cs` extends `AuthenticationStateProvider`:

- Makes an HTTP GET to the `User` endpoint to determine the current user.
- Caches the result for 60 seconds (`UserCacheRefreshInterval`) to reduce round-trips.
- Builds a `ClaimsPrincipal` from the returned `UserInfo` DTO.
- Sets `HttpClient.DefaultRequestHeaders.Authorization` to the returned Bearer token so all subsequent API calls are authenticated.
- Returns an anonymous identity if the fetch fails or the user is not authenticated.

---

## Seed Users

`AppIdentityDbContextSeed.SeedAsync` creates two default accounts at startup:

| Username | Role | Password |
|---|---|---|
| `demouser@microsoft.com` | *(none)* | `AuthorizationConstants.DEFAULT_PASSWORD` |
| `admin@microsoft.com` | `Administrators` | `AuthorizationConstants.DEFAULT_PASSWORD` |

> **Security reminder:** Change seed passwords before deploying to any non-development environment.
