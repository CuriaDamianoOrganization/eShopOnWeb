# Architecture Overview

## Introduction

eShopOnWeb is designed as a monolithic ASP.NET Core application that demonstrates common architectural patterns and best practices for building modern web applications.

## Application Architecture

The application follows Clean Architecture principles with the following main layers:

### 1. ApplicationCore (Domain Layer)

The **ApplicationCore** project contains the business logic and entities of the application. It has no dependencies on other projects and includes:

- **Entities**: Core business objects (e.g., CatalogItem, Basket, Order)
- **Interfaces**: Repository and service contracts
- **Specifications**: Query specifications for data access
- **Services**: Business logic implementation

### 2. Infrastructure (Data Access Layer)

The **Infrastructure** project handles data persistence and external concerns:

- **Data Context**: Entity Framework Core DbContext implementations
  - `CatalogContext`: Handles catalog and shopping cart data
  - `AppIdentityDbContext`: Manages user authentication and identity
- **Repositories**: Implementation of repository interfaces
- **Migrations**: Database schema migrations
- **External Services**: Integration with external systems

### 3. Web (Presentation Layer)

The **Web** project is the main ASP.NET Core MVC application:

- **Controllers**: Handle HTTP requests
- **Views**: Razor views for UI rendering
- **ViewModels**: Data transfer objects for views
- **Services**: UI-specific services
- **Extensions**: Application configuration and setup

### 4. PublicApi

The **PublicApi** project provides a RESTful API:

- RESTful endpoints for catalog and basket operations
- Used by the Blazor Admin application
- Swagger/OpenAPI documentation

### 5. BlazorAdmin

The **BlazorAdmin** project is a Blazor WebAssembly application:

- Admin interface for managing catalog items
- Runs entirely in the browser
- Communicates with the server via PublicApi

### 6. BlazorShared

The **BlazorShared** project contains shared Blazor components and models used by BlazorAdmin.

## Design Patterns

### Repository Pattern

The application uses the Repository pattern to abstract data access:

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<List<T>> ListAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### Specification Pattern

The Specification pattern is used for complex queries:

```csharp
public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
{
    public CatalogFilterSpecification(int? brandId, int? typeId)
        : base(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
                    (!typeId.HasValue || i.CatalogTypeId == typeId))
    {
        AddInclude(i => i.CatalogBrand);
        AddInclude(i => i.CatalogType);
    }
}
```

### Dependency Injection

All services and repositories are registered in the DI container and injected into constructors.

## Database Architecture

The application uses two separate databases:

1. **Catalog Database**: Stores product catalog, shopping baskets, and orders
2. **Identity Database**: Stores user accounts and authentication data

This separation follows the principle of database per bounded context.

## Project Dependencies

```
Web → Infrastructure → ApplicationCore
PublicApi → Infrastructure → ApplicationCore
BlazorAdmin → BlazorShared
```

The ApplicationCore has no dependencies on other projects, ensuring the domain layer remains isolated and testable.

## Security

- ASP.NET Core Identity for authentication
- Cookie-based authentication for the Web application
- Token-based authentication for the API
- Passwords are hashed using Identity's default password hasher

## Performance Considerations

- Entity Framework Core for efficient data access
- Repository pattern with caching capabilities
- Specification pattern for optimized queries
- Async/await throughout for better scalability

## Testing Strategy

The application includes three types of tests:

1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test multiple components working together
3. **Functional Tests**: Test the application end-to-end

## Further Reading

- [Architecting Modern Web Applications with ASP.NET Core and Azure](https://aka.ms/webappebook)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
