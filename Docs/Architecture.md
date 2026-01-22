# eShopOnWeb Architecture

## Overview

eShopOnWeb is a sample ASP.NET Core reference application demonstrating a single-process (monolithic) application architecture. This document provides an overview of the application's architectural design, key components, and design patterns.

## Architecture Pattern

The application follows a **Clean Architecture** approach, organizing code into distinct layers with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                 │
│  (Web, PublicApi, BlazorAdmin)                      │
└────────────────┬────────────────────────────────────┘
                 │
┌────────────────┴────────────────────────────────────┐
│                Infrastructure Layer                  │
│  (Data Access, External Services)                   │
└────────────────┬────────────────────────────────────┘
                 │
┌────────────────┴────────────────────────────────────┐
│                 ApplicationCore Layer                │
│  (Domain Entities, Business Logic, Interfaces)      │
└─────────────────────────────────────────────────────┘
```

## Project Structure

### ApplicationCore
The **ApplicationCore** project contains the domain model and business logic. It has no dependencies on other projects and defines:
- **Entities**: Core business objects (e.g., Product, Order, Basket)
- **Interfaces**: Abstractions for data access and services
- **Specifications**: Query specifications using the Specification pattern
- **Services**: Domain services containing business logic

### Infrastructure
The **Infrastructure** project implements interfaces defined in ApplicationCore and handles:
- **Data Access**: Entity Framework Core implementations
- **Database Contexts**: CatalogContext, AppIdentityDbContext
- **Repositories**: Generic repository pattern implementations
- **External Services**: Email, logging, and other external integrations

### Web
The **Web** project is the main ASP.NET Core MVC application that:
- Serves the public-facing e-commerce website
- Implements the presentation layer using Razor Pages and MVC
- Handles user authentication and authorization
- Manages the shopping cart and order processing UI

### PublicApi
The **PublicApi** project provides a RESTful API layer:
- Exposes endpoints for external integrations
- Used by the BlazorAdmin application
- Implements API versioning and documentation

### BlazorAdmin
The **BlazorAdmin** project is a Blazor WebAssembly application:
- Provides an admin interface for managing catalog items
- Consumes the PublicApi for data operations
- Demonstrates Blazor WebAssembly integration with ASP.NET Core

### BlazorShared
The **BlazorShared** project contains shared components and models used by Blazor projects.

## Key Design Patterns

### Repository Pattern
The application uses the Repository pattern to abstract data access logic, making it easier to test and maintain.

### Specification Pattern
The Specification pattern is used to encapsulate query logic, making it reusable and testable.

### Dependency Injection
ASP.NET Core's built-in dependency injection is used throughout the application to manage dependencies and promote loose coupling.

### Domain-Driven Design (DDD)
The application follows DDD principles:
- **Entities**: Objects with identity
- **Value Objects**: Immutable objects defined by their attributes
- **Aggregates**: Clusters of related entities
- **Domain Services**: Encapsulate business logic that doesn't naturally fit within entities

## Data Storage

The application uses **SQL Server** for persistent storage with two separate databases:
1. **Catalog Database**: Stores product catalog, shopping cart, and order information
2. **Identity Database**: Manages user authentication and authorization data

Entity Framework Core is used as the ORM, with Code-First migrations for database schema management.

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages, MVC, Blazor WebAssembly
- **Data Access**: Entity Framework Core
- **Database**: SQL Server (with in-memory option for development)
- **Authentication**: ASP.NET Core Identity
- **API**: RESTful API with Swagger/OpenAPI documentation
- **Testing**: xUnit, Moq

## Deployment

The application supports multiple deployment scenarios:
- **Local Development**: Run with SQL Server or in-memory database
- **Docker**: Containerized deployment using Docker Compose
- **Azure**: Cloud deployment using Azure Developer CLI (azd)
- **Dev Containers**: Development in containerized environments

## Testing Strategy

The solution includes comprehensive testing:
- **Unit Tests**: Test business logic in isolation
- **Integration Tests**: Test data access and infrastructure
- **Functional Tests**: Test complete user workflows

## References

For more detailed information about the architectural principles and patterns used in this application, refer to:
- [Architecting Modern Web Applications with ASP.NET Core and Azure](https://aka.ms/webappebook)
- [.NET Architecture Documentation](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/)
