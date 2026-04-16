# API Design

## Overview

eShopOnWeb exposes two API surfaces:

| Surface | Project | Pattern | Auth |
|---|---|---|---|
| Admin REST API | `PublicApi` | Minimal API + Ardalis.ApiEndpoints | JWT Bearer |
| Internal Web API | `Web` | MVC Controllers | Cookie |

---

## PublicApi — Endpoint-per-Feature Pattern

The PublicApi uses **Ardalis.ApiEndpoints** (`IEndpoint<TResult, TRequest, TDependency>`), where each HTTP operation is a dedicated class. This avoids "fat controller" syndrome and makes each endpoint independently testable.

### Endpoint Anatomy

```
PublicApi/
├── AuthEndpoints/
│   └── AuthenticateEndpoint.cs       POST /api/authenticate
├── CatalogItemEndpoints/
│   ├── CatalogItemListPagedEndpoint   GET  /api/catalog-items?pageSize=&pageIndex=&brandId=&typeId=
│   ├── CatalogItemGetByIdEndpoint     GET  /api/catalog-items/{catalogItemId}
│   ├── CreateCatalogItemEndpoint      POST /api/catalog-items          [Admin]
│   ├── UpdateCatalogItemEndpoint      PUT  /api/catalog-items          [Admin]
│   └── DeleteCatalogItemEndpoint      DELETE /api/catalog-items/{id}   [Admin]
├── CatalogBrandEndpoints/
│   └── CatalogBrandListEndpoint       GET  /api/catalog-brands
└── CatalogTypeEndpoints/
    └── CatalogTypeListEndpoint        GET  /api/catalog-types
```

### Endpoint Lifecycle

1. `AddRoute(IEndpointRouteBuilder app)` — called at startup to register the route, HTTP method, response type, and authorization requirements.
2. `HandleAsync(TRequest request, TDependency dependency)` — handles the request, invokes the domain service or repository, maps to DTO, and returns an `IResult`.

```csharp
public void AddRoute(IEndpointRouteBuilder app)
{
    app.MapPost("api/catalog-items",
        [Authorize(Roles = Constants.Roles.ADMINISTRATORS,
                   AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        async (CreateCatalogItemRequest request, IRepository<CatalogItem> itemRepository) =>
        {
            return await HandleAsync(request, itemRepository);
        })
        .Produces<CreateCatalogItemResponse>()
        .WithTags("CatalogItemEndpoints");
}
```

### Request / Response Conventions

All request types inherit from `BaseRequest`, which provides a `CorrelationId()` helper for tracing.  
All response types inherit from `BaseResponse`, which carries that correlation ID back to the caller.

```csharp
public class CreateCatalogItemRequest : BaseRequest
{
    public int CatalogTypeId { get; init; }
    public int CatalogBrandId { get; init; }
    public string Name { get; init; } = string.Empty;
    // ...
}

public class CreateCatalogItemResponse : BaseResponse
{
    public CatalogItemDto CatalogItem { get; set; } = new();
}
```

### HTTP Status Codes

| Scenario | Status |
|---|---|
| Resource created | `201 Created` with `Location` header |
| Successful read | `200 OK` |
| Resource not found | `404 Not Found` |
| Duplicate name | `409 Conflict` (via `ExceptionMiddleware`) |
| Unhandled error | `500 Internal Server Error` (via `ExceptionMiddleware`) |
| Unauthenticated | `401 Unauthorized` |
| Forbidden (wrong role) | `403 Forbidden` |

---

## AutoMapper Profile

`PublicApi/MappingProfile.cs` defines AutoMapper mappings between domain entities and DTOs:

```csharp
// Example mappings (illustrative)
CreateMap<CatalogItem, CatalogItemDto>();
CreateMap<CatalogBrand, CatalogBrandDto>();
CreateMap<CatalogType, CatalogTypeDto>();
```

DTOs are declared in `BlazorShared/Models/` so they are shared with the Blazor client without re-declaring them.

---

## Swagger / OpenAPI

The PublicApi registers Swagger via Swashbuckle:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
    c.SchemaFilter<CustomSchemaFilters>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });
});
```

- **Annotations** — endpoints use `[SwaggerOperation(Summary = "...")]` for human-readable documentation.
- **Custom Schema Filters** — `CustomSchemaFilters.cs` adjusts schema generation for complex types.
- **Security Definition** — Bearer token support is declared so the Swagger UI can issue authenticated requests.

Swagger UI is accessible at `/swagger` for all environments (not gated by `IsDevelopment`).

---

## Paging

List endpoints return paginated results. The client controls pagination via query parameters:

| Parameter | Default | Description |
|---|---|---|
| `pageSize` | 10 | Items per page |
| `pageIndex` | 0 | Zero-based page number |
| `brandId` | null | Optional brand filter |
| `typeId` | null | Optional type filter |

The response includes `PageCount` and `PageIndex` so clients can render pagination controls without an additional count request.

---

## Web MVC Controllers

The `Web` project exposes MVC controllers primarily to serve data to Blazor components embedded in the Razor Pages host:

- `OrderController` — returns order history for the authenticated user
- `BasketController` — handles add/update/remove operations on the shopping cart

These are not part of the public API surface; they are consumed by the Blazor admin via the cookie session and are protected by `[Authorize]` without a specific scheme (defaults to the cookie scheme).

---

## CQRS in the Web Layer

The Web project uses **MediatR** for its internal query dispatch:

```
GET /orders/{orderId}
  → OrderDetailsPageModel.OnGetAsync()
    → mediator.Send(new GetOrderDetails(orderId))
      → GetOrderDetailsHandler
        → IOrderRepository.FirstOrDefaultAsync(spec)
          → OrderDetailViewModel
```

This keeps Razor Page code-behind files thin and makes handlers independently testable. Commands (writes) follow the same pattern.
