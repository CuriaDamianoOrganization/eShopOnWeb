# Configuration and deployment

## Configuration sources

ASP.NET Core loads settings from the appsettings files and environment variables. Local development supports SQL Server or the in-memory provider. The Web production path adds Azure Key Vault using Azure credentials and reads the catalog and Identity SQL connection values from the configured secret names.

Configuration includes the Web base URL, catalog settings, database connection strings, and deployment-specific Azure settings. Keep environment-specific values outside committed source whenever they contain credentials or signing material.

## Deployment shape

The sample can run locally, in Docker Compose, or as an Azure-hosted deployment provisioned by the `infra` Bicep templates. Web and PublicApi are deployed as separate application processes. The Web application serves the customer experience and hosts the Blazor administration client; PublicApi serves administration catalog operations.

## Startup sequence

1. Build configuration and register infrastructure and application services.
2. Register authentication, authorization, MVC/Razor/Blazor, and health checks.
3. Build the application and seed catalog and Identity data.
4. Select development or production error handling and security middleware.
5. Start endpoint routing and health probes.

## Operational checklist

- Provision SQL Server and Identity storage before first startup.
- Configure secret-store access for production.
- Apply database migrations in the deployment pipeline.
- Verify health-check endpoints after deployment.
- Confirm CORS, HTTPS, cookie, and JWT settings match the deployed host names.
- Ensure diagnostic logging is collected centrally.
