# Getting Started Guide

This guide will help you set up and run the eShopOnWeb application on your local machine.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or SQL Server Express/LocalDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads)

Optional:
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for running in containers)
- [Azure Developer CLI](https://aka.ms/azure-dev/install) (for Azure deployment)

## Clone the Repository

```bash
git clone https://github.com/dotnet-architecture/eShopOnWeb.git
cd eShopOnWeb
```

## Setup Options

You can run eShopOnWeb in several ways:

### Option 1: Run with In-Memory Database (Quickest)

This is the fastest way to get started without setting up SQL Server.

1. Navigate to the Web project:
   ```bash
   cd src/Web
   ```

2. Update `appsettings.json` to use in-memory database:
   ```json
   {
     "UseOnlyInMemoryDatabase": true
   }
   ```

3. Run the application:
   ```bash
   dotnet run --launch-profile Web
   ```

4. Open your browser and navigate to `https://localhost:5001`

### Option 2: Run with SQL Server Database

For a more realistic setup with persistent data:

1. Ensure your SQL Server is running

2. Navigate to the Web project:
   ```bash
   cd src/Web
   ```

3. Update connection strings in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "CatalogConnection": "Server=(localdb)\\mssqllocaldb;Database=Microsoft.eShopOnWeb.CatalogDb;Trusted_Connection=True;MultipleActiveResultSets=true",
       "IdentityConnection": "Server=(localdb)\\mssqllocaldb;Database=Microsoft.eShopOnWeb.Identity;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. Install Entity Framework Core tools:
   ```bash
   dotnet tool update --global dotnet-ef
   ```

5. Apply database migrations:
   ```bash
   dotnet restore
   dotnet tool restore
   dotnet ef database update -c catalogcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
   dotnet ef database update -c appidentitydbcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
   ```

6. Run the application:
   ```bash
   dotnet run --launch-profile Web
   ```

### Option 3: Run with Docker

Run the entire application stack using Docker:

1. From the repository root:
   ```bash
   docker-compose build
   docker-compose up
   ```

2. Access the application:
   - Web: `http://localhost:5106`
   - API: `http://localhost:5200`

### Option 4: Run with Dev Container

If you're using Visual Studio Code or GitHub Codespaces:

1. Open the project in VS Code
2. Install the "Dev Containers" extension
3. Click "Reopen in Container" when prompted
4. The dev container will automatically set up the environment

See [.devcontainer/devcontainerreadme.md](../.devcontainer/devcontainerreadme.md) for more details.

## Running with the Admin Interface

To use the full admin functionality:

1. Start the PublicApi project:
   ```bash
   cd src/PublicApi
   dotnet run
   ```

2. In a separate terminal, start the Web project:
   ```bash
   cd src/Web
   dotnet run --launch-profile Web
   ```

3. Access the admin interface at `https://localhost:5001/admin`

## Default Credentials

The application seeds a default user account:

- **Email**: demouser@microsoft.com
- **Password**: Pass@word1

## Verify Installation

After starting the application, you should see:

1. **Home Page**: Product catalog with sample items
2. **Login**: Ability to log in with the demo user
3. **Shopping Cart**: Add items to cart
4. **Admin Panel** (if running PublicApi): Manage catalog items

## Troubleshooting

### Port Already in Use

If you get a port conflict error, you can change the ports in `launchSettings.json`:

```json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```

### Database Connection Issues

- Verify SQL Server is running
- Check connection strings in `appsettings.json`
- Ensure you've run the database migrations

### File Locking Errors

If you get file locking errors when building:
- Stop the running application
- Close any instances of the app
- Try building again

### Migration Errors

If migrations fail:
- Ensure dotnet-ef tools are installed: `dotnet tool install --global dotnet-ef`
- Check that you're in the correct directory (src/Web)
- Verify the connection string is correct

## Next Steps

- Explore the [Architecture Overview](./Architecture.md)
- Learn about [Deployment Options](./Deployment.md)
- Review the [API Documentation](./API.md)
- Read the [eBook](https://aka.ms/webappebook) for deeper understanding

## Visual Studio Setup

If using Visual Studio:

1. Open `eShopOnWeb.sln`
2. Set `Web` as the startup project
3. Or configure multiple startup projects (Web and PublicApi) for full functionality
4. Press F5 to run

## Building the Solution

From the repository root:

```bash
dotnet restore
dotnet build
```

## Running Tests

```bash
dotnet test
```

For more details on testing, see the tests folder documentation.

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
