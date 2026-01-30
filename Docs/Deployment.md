# Deployment Guide

This guide covers various deployment options for the eShopOnWeb application.

## Deployment Options

eShopOnWeb can be deployed to:

1. Azure (using Azure Developer CLI)
2. Azure (manual deployment)
3. Docker/Containers
4. IIS/Windows Server
5. Linux servers

## 1. Deploy to Azure with Azure Developer CLI (Recommended)

The fastest way to deploy to Azure is using the Azure Developer CLI (azd).

### Prerequisites

- Azure subscription
- Azure Developer CLI installed

### Install Azure Developer CLI

**Windows:**
```powershell
powershell -ex AllSigned -c "Invoke-RestMethod 'https://aka.ms/install-azd.ps1' | Invoke-Expression"
```

**Linux/MacOS:**
```bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

Or install with package managers:
- Windows: `winget install microsoft.azd`
- macOS: `brew tap azure/azd && brew install azd`
- Linux: See [documentation](https://aka.ms/azure-dev/install)

### Deploy Steps

1. **Login to Azure:**
   ```bash
   azd auth login
   ```

2. **Initialize the environment:**
   ```bash
   azd init -t dotnet-architecture/eShopOnWeb
   ```

3. **Deploy to Azure:**
   ```bash
   azd up
   ```

4. **Follow the prompts:**
   - Enter an environment name
   - Select your Azure subscription
   - Select a location (e.g., eastus, westus2)

5. **Wait for deployment** (typically 5-10 minutes)

6. **Access your application** using the URL provided at the end of deployment

### What Gets Deployed

The `azd up` command provisions:

- Azure App Service (for the Web application)
- Azure SQL Database (for data storage)
- Azure Key Vault (for secure credential storage)
- Application Insights (for monitoring)
- Resource Group (named `rg-{env-name}`)

### Cleanup

To remove all Azure resources:
```bash
azd down
```

## 2. Manual Azure Deployment

### Deploy to Azure App Service

1. **Create Azure Resources:**
   ```bash
   # Create resource group
   az group create --name eShopOnWeb-rg --location eastus

   # Create App Service plan
   az appservice plan create --name eShopOnWeb-plan --resource-group eShopOnWeb-rg --sku B1 --is-linux

   # Create Web App
   az webapp create --name eshoponweb-webapp --resource-group eShopOnWeb-rg --plan eShopOnWeb-plan --runtime "DOTNET:8.0"

   # Create SQL Server
   az sql server create --name eshoponweb-sql --resource-group eShopOnWeb-rg --location eastus --admin-user sqladmin --admin-password {your-password}

   # Create databases
   az sql db create --name CatalogDb --server eshoponweb-sql --resource-group eShopOnWeb-rg --service-objective Basic
   az sql db create --name IdentityDb --server eshoponweb-sql --resource-group eShopOnWeb-rg --service-objective Basic
   ```

2. **Configure Connection Strings:**
   ```bash
   az webapp config connection-string set --name eshoponweb-webapp --resource-group eShopOnWeb-rg --connection-string-type SQLAzure --settings CatalogConnection="{connection-string}" IdentityConnection="{connection-string}"
   ```

3. **Publish the Application:**
   ```bash
   # From the Web project directory
   dotnet publish -c Release -o ./publish
   
   # Zip the output
   cd publish
   zip -r ../app.zip *
   cd ..
   
   # Deploy to Azure
   az webapp deployment source config-zip --name eshoponweb-webapp --resource-group eShopOnWeb-rg --src app.zip
   ```

## 3. Docker Deployment

### Build Docker Images

From the repository root:

```bash
# Build all images
docker-compose build

# Or build individual images
docker build -f src/Web/Dockerfile -t eshoponweb:latest .
docker build -f src/PublicApi/Dockerfile -t eshoponweb-api:latest .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

Access the application:
- Web: http://localhost:5106
- API: http://localhost:5200

### Push to Container Registry

```bash
# Tag images
docker tag eshoponweb:latest myregistry.azurecr.io/eshoponweb:latest

# Login to registry
az acr login --name myregistry

# Push images
docker push myregistry.azurecr.io/eshoponweb:latest
```

### Deploy to Azure Container Instances

```bash
az container create \
  --resource-group eShopOnWeb-rg \
  --name eshoponweb-container \
  --image myregistry.azurecr.io/eshoponweb:latest \
  --dns-name-label eshoponweb-app \
  --ports 80
```

### Deploy to Azure Kubernetes Service (AKS)

1. Create AKS cluster:
   ```bash
   az aks create --resource-group eShopOnWeb-rg --name eShopOnWebCluster --node-count 2 --generate-ssh-keys
   ```

2. Get credentials:
   ```bash
   az aks get-credentials --resource-group eShopOnWeb-rg --name eShopOnWebCluster
   ```

3. Deploy using kubectl:
   ```bash
   kubectl apply -f k8s/deployment.yaml
   ```

## 4. IIS Deployment (Windows Server)

### Prerequisites

- Windows Server with IIS installed
- .NET 8.0 Hosting Bundle
- SQL Server

### Steps

1. **Install .NET Hosting Bundle:**
   Download from [.NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0)

2. **Publish the application:**
   ```bash
   dotnet publish -c Release -o C:\inetpub\wwwroot\eShopOnWeb
   ```

3. **Create IIS Application Pool:**
   - Open IIS Manager
   - Create new Application Pool
   - Set .NET CLR Version to "No Managed Code"
   - Set Managed Pipeline Mode to "Integrated"

4. **Create IIS Website:**
   - Create new website
   - Set physical path to `C:\inetpub\wwwroot\eShopOnWeb`
   - Select the application pool created above
   - Configure bindings (HTTP/HTTPS)

5. **Configure Connection Strings:**
   Update `appsettings.json` with production connection strings

6. **Restart IIS:**
   ```bash
   iisreset
   ```

## 5. Linux Server Deployment

### Prerequisites

- Linux server (Ubuntu 20.04+ recommended)
- .NET 8.0 Runtime
- Nginx or Apache
- PostgreSQL or SQL Server for Linux

### Steps

1. **Install .NET Runtime:**
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --channel 8.0
   ```

2. **Publish the application:**
   ```bash
   dotnet publish -c Release -o /var/www/eshoponweb
   ```

3. **Configure systemd service:**
   Create `/etc/systemd/system/eshoponweb.service`:
   ```ini
   [Unit]
   Description=eShopOnWeb Application

   [Service]
   WorkingDirectory=/var/www/eshoponweb
   ExecStart=/usr/bin/dotnet /var/www/eshoponweb/Web.dll
   Restart=always
   RestartSec=10
   SyslogIdentifier=eshoponweb
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production

   [Install]
   WantedBy=multi-user.target
   ```

4. **Start the service:**
   ```bash
   systemctl enable eshoponweb
   systemctl start eshoponweb
   ```

5. **Configure Nginx as reverse proxy:**
   Create `/etc/nginx/sites-available/eshoponweb`:
   ```nginx
   server {
       listen 80;
       server_name your-domain.com;
       
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }
   ```

6. **Enable and restart Nginx:**
   ```bash
   ln -s /etc/nginx/sites-available/eshoponweb /etc/nginx/sites-enabled/
   nginx -t
   systemctl restart nginx
   ```

## Environment Variables

Key environment variables to configure:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__CatalogConnection={connection-string}
ConnectionStrings__IdentityConnection={connection-string}
UseOnlyInMemoryDatabase=false
```

## Database Migrations in Production

Before deploying, ensure database migrations are applied:

```bash
dotnet ef database update -c catalogcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
dotnet ef database update -c appidentitydbcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
```

Or use SQL scripts:
```bash
dotnet ef migrations script -c catalogcontext -o catalog.sql
dotnet ef migrations script -c appidentitydbcontext -o identity.sql
```

## Health Checks

The application includes health check endpoints:

- `/health` - Basic health check
- `/health/ready` - Readiness check
- `/health/live` - Liveness check

Configure your load balancer or orchestrator to use these endpoints.

## Monitoring and Logging

### Application Insights (Azure)

Configure in `appsettings.json`:
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "{your-key}"
  }
}
```

### Logging Configuration

Configure logging levels in `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

## Security Considerations

1. **Use HTTPS** in production
2. **Secure connection strings** (use Azure Key Vault or environment variables)
3. **Update authentication keys** for production
4. **Enable CORS** only for trusted domains
5. **Configure firewall rules** for database access
6. **Use managed identities** when possible (Azure)
7. **Regularly update dependencies** and security patches

## Performance Tuning

1. Enable response caching
2. Use CDN for static assets
3. Configure connection pooling
4. Enable output caching where appropriate
5. Use distributed caching (Redis) for multi-instance deployments

## Troubleshooting

### Application won't start
- Check logs in `/var/log/` (Linux) or Event Viewer (Windows)
- Verify .NET runtime is installed
- Check file permissions

### Database connection errors
- Verify connection strings
- Check firewall rules
- Ensure database migrations are applied

### Performance issues
- Check Application Insights metrics
- Review database query performance
- Verify resource allocation (CPU, memory)

## Rollback Strategy

1. Keep previous deployment packages
2. Use blue-green deployment for zero-downtime
3. Have database backup and restore procedures
4. Test rollback procedures in staging

## Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Docker Documentation](https://docs.docker.com/)
- [Nginx Documentation](https://nginx.org/en/docs/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
