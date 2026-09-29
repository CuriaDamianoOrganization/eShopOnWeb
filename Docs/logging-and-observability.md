# Logging and observability

## Logging

Both hosts register the built-in Microsoft.Extensions console logger. Startup, database seeding, environment-specific middleware, and application launch are recorded with structured log calls.

ApplicationCore depends on `IAppLogger<T>` rather than directly on ASP.NET Core logging. Infrastructure implements that abstraction with `LoggerAdapter<T>`, which creates an `ILogger<T>` from the configured `ILoggerFactory`. This keeps the core project portable and makes the logging boundary explicit.

## Health checks

The Web host registers checks for the Public API and the home page. They are exposed as:

- `/health`: a combined JSON response containing status and individual check results.
- `api_health_check`: the API-tagged checks.
- `home_page_health_check`: the home-page-tagged checks.

These endpoints are intended for container, load balancer, and deployment probes. Their availability should be monitored separately from business metrics.

## Production guidance

- Forward console logs to the hosting platform's centralized log service.
- Prefer message templates and properties over string interpolation.
- Do not log passwords, tokens, connection strings, or personal data.
- Correlate logs with request and trace identifiers at the hosting layer.
- Alert on repeated startup, seeding, authentication, and health-check failures.

Relevant implementation points:

- `src/Infrastructure/Logging/LoggerAdapter.cs`
- `src/ApplicationCore/Interfaces/IAppLogger.cs`
- `src/Web/HealthChecks/`
- `src/Web/Program.cs`
