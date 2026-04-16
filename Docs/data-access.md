# Data Access

## Overview

Data access is handled entirely in the `Infrastructure` project, which depends on `ApplicationCore` but is never referenced directly by host projects — all interaction happens through the interfaces and entities defined in `ApplicationCore`.

Two EF Core `DbContext` classes manage separate databases (or in-memory stores during testing):

| DbContext | Purpose |
|---|---|
| `CatalogContext` | All business entities: catalog, baskets, orders |
| `AppIdentityDbContext` | ASP.NET Core Identity users and roles |

---

## Repository Pattern

### Interfaces (ApplicationCore)

```
IReadRepository<T>   — read-only operations
IRepository<T>       — read/write operations (extends IReadRepository<T>)
```

Both interfaces are defined in `ApplicationCore/Interfaces/` and implemented in `Infrastructure`. Neither depends on EF Core; they accept `ISpecification<T>` objects for query composition.

Common operations provided by `IRepository<T>`:

| Method | Description |
|---|---|
| `GetByIdAsync(int id)` | Fetch a single entity by primary key |
| `FirstOrDefaultAsync(spec)` | Fetch first entity matching a specification |
| `ListAsync(spec)` | Fetch all entities matching a specification |
| `CountAsync(spec)` | Count entities matching a specification |
| `AddAsync(T entity)` | Insert and return the new entity |
| `UpdateAsync(T entity)` | Update an existing entity |
| `DeleteAsync(T entity)` | Remove an entity |

### Implementation (Infrastructure)

`Infrastructure/Data/EfRepository.cs` inherits from Ardalis.Specification's `RepositoryBase<T>`, which provides the specification evaluation logic on top of EF Core's `DbSet<T>`:

```csharp
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public EfRepository(CatalogContext dbContext) : base(dbContext) { }
}
```

The generic constraint `IAggregateRoot` ensures only aggregate roots are managed directly by repositories, enforcing DDD consistency boundaries.

---

## Aggregate Roots

Aggregate roots are identified by the marker interface `IAggregateRoot`. The application has three aggregates:

| Aggregate Root | Child Entities |
|---|---|
| `Basket` | `BasketItem` |
| `Order` | `OrderItem` |
| `CatalogItem` | *(none; references `CatalogBrand`, `CatalogType` by value)* |

Child entities are never fetched or mutated through a repository directly — only through their aggregate root.

---

## Specification Pattern

Specifications encapsulate query criteria, including filters, ordering, pagination, and `Include` directives. They live in `ApplicationCore/Specifications/` so business logic can define queries without knowing EF Core.

### Examples

**BasketWithItemsSpecification** — loads a basket and its items by ID or buyer ID:

```csharp
public sealed class BasketWithItemsSpecification : Specification<Basket>
{
    public BasketWithItemsSpecification(int basketId)
    {
        Query.Where(b => b.Id == basketId).Include(b => b.Items);
    }

    public BasketWithItemsSpecification(string buyerId)
    {
        Query.Where(b => b.BuyerId == buyerId).Include(b => b.Items);
    }
}
```

**CatalogFilterSpecification** — filters catalog items by optional brand and/or type:

```csharp
public class CatalogFilterSpecification : Specification<CatalogItem>
{
    public CatalogFilterSpecification(int? brandId, int? typeId)
    {
        Query.Where(i =>
            (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId));
    }
}
```

**CatalogFilterPaginatedSpecification** — adds skip/take for paged results:

```csharp
public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId)
    {
        Query.Where(...)
             .Skip(skip)
             .Take(take);
    }
}
```

---

## DbContexts

### CatalogContext

```csharp
public class CatalogContext : DbContext
{
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

Entity configurations are applied automatically from the `Infrastructure` assembly via `ApplyConfigurationsFromAssembly`, keeping the `DbContext` class clean.

### AppIdentityDbContext

```csharp
public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options) { }
}
```

Extends `IdentityDbContext<ApplicationUser>` to inherit all ASP.NET Core Identity tables.

---

## Direct Query Service

For aggregation queries that do not map cleanly to the repository/specification model, `Infrastructure/Data/Queries/BasketQueryService.cs` queries the `CatalogContext` directly:

```csharp
public async Task<int> CountTotalBasketItems(string username)
{
    return await _dbContext.Baskets
        .Where(basket => basket.BuyerId == username)
        .SelectMany(item => item.Items)
        .SumAsync(sum => sum.Quantity);
}
```

This implements `IBasketQueryService`, which is defined in `ApplicationCore/Interfaces/`.

---

## Database Provider Configuration

`Infrastructure/Dependencies.cs` registers both contexts with the appropriate provider:

```csharp
if (useOnlyInMemoryDatabase)
{
    services.AddDbContext<CatalogContext>(c => c.UseInMemoryDatabase("Catalog"));
    services.AddDbContext<AppIdentityDbContext>(c => c.UseInMemoryDatabase("Identity"));
}
else
{
    services.AddDbContext<CatalogContext>(c =>
        c.UseSqlServer(configuration.GetConnectionString("CatalogConnection")));
    services.AddDbContext<AppIdentityDbContext>(c =>
        c.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
}
```

The `UseOnlyInMemoryDatabase` flag is set to `true` in test project configuration files, ensuring no real database is required for unit or integration tests.

---

## Seeding

| Seeder | Context | Data |
|---|---|---|
| `CatalogContextSeed` | `CatalogContext` | Catalog brands, types, and items with images |
| `AppIdentityDbContextSeed` | `AppIdentityDbContext` | Default `demouser` and `admin` accounts |

Both seeders check whether data already exists before inserting, making them safe to run on repeated startups. On SQL Server they also apply pending migrations automatically.
