# Data and persistence

## Contexts and stores

Infrastructure uses Entity Framework Core with two DbContexts:

- `CatalogContext` stores catalog, basket, and order data.
- `AppIdentityDbContext` stores ASP.NET Core Identity users, roles, claims, and tokens.

By default both contexts use SQL Server. Setting `UseOnlyInMemoryDatabase` enables separate in-memory stores for local development and tests.

## Repository and specification boundaries

ApplicationCore consumes `IReadRepository<T>` and `IRepository<T>`. Infrastructure implements both with `EfRepository<T>`. Query shape and filtering are expressed through specifications, keeping EF-specific query construction out of application services.

Entity configurations and migrations are maintained under `src/Infrastructure/Data/Config` and `src/Infrastructure/Data/Migrations`. Identity migrations are maintained under `src/Infrastructure/Identity/Migrations`.

## Startup and deployment

The Web and PublicApi hosts seed catalog and Identity data during startup. Local database setup uses the EF Core migrations described in the root README. Production Web deployments retrieve SQL connection settings from Azure Key Vault and enable SQL transient-failure retries.

## Data safety

- Apply migrations through a controlled deployment process.
- Back up catalog and Identity databases independently.
- Keep connection strings and credentials in secret management, not source files.
- Review cascade behavior and transaction boundaries when adding aggregates.
- Avoid returning persistence entities directly from API contracts.
