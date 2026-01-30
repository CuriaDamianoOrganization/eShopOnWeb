# API Documentation

This document provides information about the eShopOnWeb Public API.

## Overview

The PublicApi project exposes RESTful endpoints for accessing and managing the eShop catalog and shopping baskets. This API is primarily used by the Blazor Admin application but can be consumed by any client.

## Base URL

When running locally:
- **Development**: `https://localhost:5200` or `http://localhost:5201`
- **Docker**: `http://localhost:5200`

## Authentication

The API uses token-based authentication. Most endpoints require authentication.

### Getting an Access Token

```http
POST /api/authenticate
Content-Type: application/json

{
  "username": "demouser@microsoft.com",
  "password": "Pass@word1"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-01T12:00:00Z"
}
```

Use the token in subsequent requests:
```http
Authorization: Bearer {token}
```

## API Endpoints

### Catalog Items

#### Get All Catalog Items

```http
GET /api/catalog-items
```

Query Parameters:
- `pageSize` (optional): Number of items per page (default: 10)
- `pageIndex` (optional): Page number (default: 0)

Response:
```json
{
  "pageIndex": 0,
  "pageSize": 10,
  "count": 50,
  "data": [
    {
      "id": 1,
      "name": ".NET Blue Sweatshirt",
      "description": "A comfortable sweatshirt...",
      "price": 29.99,
      "pictureUri": "/images/products/1.png",
      "catalogTypeId": 1,
      "catalogTypeName": "Sweatshirt",
      "catalogBrandId": 1,
      "catalogBrandName": ".NET"
    }
  ]
}
```

#### Get Catalog Item by ID

```http
GET /api/catalog-items/{id}
```

Response:
```json
{
  "id": 1,
  "name": ".NET Blue Sweatshirt",
  "description": "A comfortable sweatshirt...",
  "price": 29.99,
  "pictureUri": "/images/products/1.png",
  "catalogTypeId": 1,
  "catalogTypeName": "Sweatshirt",
  "catalogBrandId": 1,
  "catalogBrandName": ".NET"
}
```

#### Create Catalog Item

```http
POST /api/catalog-items
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product description",
  "price": 19.99,
  "pictureUri": "/images/products/new.png",
  "catalogTypeId": 1,
  "catalogBrandId": 1
}
```

Response: `201 Created`
```json
{
  "id": 51,
  "name": "New Product",
  ...
}
```

#### Update Catalog Item

```http
PUT /api/catalog-items/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Product Name",
  "description": "Updated description",
  "price": 24.99,
  "pictureUri": "/images/products/1.png",
  "catalogTypeId": 1,
  "catalogBrandId": 1
}
```

Response: `200 OK`

#### Delete Catalog Item

```http
DELETE /api/catalog-items/{id}
Authorization: Bearer {token}
```

Response: `204 No Content`

### Catalog Types

#### Get All Catalog Types

```http
GET /api/catalog-types
```

Response:
```json
[
  {
    "id": 1,
    "name": "Mug"
  },
  {
    "id": 2,
    "name": "T-Shirt"
  }
]
```

#### Create Catalog Type

```http
POST /api/catalog-types
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "New Type"
}
```

#### Update Catalog Type

```http
PUT /api/catalog-types/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Type Name"
}
```

#### Delete Catalog Type

```http
DELETE /api/catalog-types/{id}
Authorization: Bearer {token}
```

### Catalog Brands

#### Get All Catalog Brands

```http
GET /api/catalog-brands
```

Response:
```json
[
  {
    "id": 1,
    "name": ".NET"
  },
  {
    "id": 2,
    "name": "Visual Studio"
  }
]
```

#### Create Catalog Brand

```http
POST /api/catalog-brands
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "New Brand"
}
```

#### Update Catalog Brand

```http
PUT /api/catalog-brands/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Brand Name"
}
```

#### Delete Catalog Brand

```http
DELETE /api/catalog-brands/{id}
Authorization: Bearer {token}
```

### Baskets

#### Get Basket

```http
GET /api/baskets/{buyerId}
Authorization: Bearer {token}
```

Response:
```json
{
  "id": 1,
  "buyerId": "user@example.com",
  "items": [
    {
      "id": 1,
      "catalogItemId": 5,
      "productName": "Product Name",
      "unitPrice": 19.99,
      "quantity": 2,
      "pictureUrl": "/images/products/5.png"
    }
  ]
}
```

#### Update Basket

```http
POST /api/baskets
Authorization: Bearer {token}
Content-Type: application/json

{
  "buyerId": "user@example.com",
  "items": [
    {
      "catalogItemId": 5,
      "quantity": 2
    }
  ]
}
```

#### Delete Basket

```http
DELETE /api/baskets/{id}
Authorization: Bearer {token}
```

## Swagger/OpenAPI Documentation

When running in development mode, interactive API documentation is available at:

```
https://localhost:5200/swagger
```

The Swagger UI provides:
- Complete API endpoint documentation
- Request/response examples
- Try-it-out functionality
- Schema definitions

## Error Responses

The API uses standard HTTP status codes:

- `200 OK`: Request succeeded
- `201 Created`: Resource created successfully
- `204 No Content`: Request succeeded with no content to return
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Error Response Format:
```json
{
  "error": "Error message description",
  "details": "Additional error details if available"
}
```

## Rate Limiting

Currently, the API does not implement rate limiting. In production, consider implementing rate limiting to prevent abuse.

## Versioning

The current API is version 1.0. Future versions will be indicated in the URL:
- v1: `/api/v1/...`
- v2: `/api/v2/...`

## CORS Configuration

The API is configured to accept requests from the Blazor Admin application. To allow requests from other origins, update the CORS policy in `Startup.cs`:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://your-domain.com")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

## Pagination

List endpoints support pagination with the following query parameters:

- `pageIndex`: Zero-based page number (default: 0)
- `pageSize`: Number of items per page (default: 10, max: 100)

Example:
```http
GET /api/catalog-items?pageIndex=2&pageSize=20
```

## Filtering

Some endpoints support filtering:

```http
GET /api/catalog-items?brandId=1&typeId=2
```

## Sorting

Sorting is not currently implemented but can be added using query parameters like:
```
?sortBy=name&sortDirection=asc
```

## Best Practices

1. **Use HTTPS** in production
2. **Store tokens securely** (not in localStorage for sensitive apps)
3. **Implement token refresh** for long-lived sessions
4. **Handle errors gracefully** with proper retry logic
5. **Use appropriate HTTP methods**:
   - GET for reading data
   - POST for creating resources
   - PUT for updating resources
   - DELETE for removing resources

## Code Examples

### C# Client Example

```csharp
using System.Net.Http;
using System.Net.Http.Json;

var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:5200");

// Authenticate
var loginRequest = new { Username = "user@example.com", Password = "password" };
var loginResponse = await client.PostAsJsonAsync("/api/authenticate", loginRequest);
var token = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

// Use token
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token.Token);

// Get catalog items
var items = await client.GetFromJsonAsync<CatalogItemsResponse>("/api/catalog-items");
```

### JavaScript Client Example

```javascript
// Authenticate
const loginResponse = await fetch('https://localhost:5200/api/authenticate', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'user@example.com',
    password: 'password'
  })
});
const { token } = await loginResponse.json();

// Get catalog items
const catalogResponse = await fetch('https://localhost:5200/api/catalog-items', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const items = await catalogResponse.json();
```

### cURL Examples

```bash
# Authenticate
curl -X POST https://localhost:5200/api/authenticate \
  -H "Content-Type: application/json" \
  -d '{"username":"user@example.com","password":"password"}'

# Get catalog items
curl -X GET https://localhost:5200/api/catalog-items \
  -H "Authorization: Bearer {token}"

# Create catalog item
curl -X POST https://localhost:5200/api/catalog-items \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name":"New Item","price":29.99,"catalogTypeId":1,"catalogBrandId":1}'
```

## Additional Resources

- [ASP.NET Core Web API Documentation](https://docs.microsoft.com/aspnet/core/web-api/)
- [OpenAPI Specification](https://swagger.io/specification/)
- [REST API Best Practices](https://restfulapi.net/)
