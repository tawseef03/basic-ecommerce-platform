# Basic E-Commerce Platform

A REST API for an online store, built as a monolith with ASP.NET Core. Based on the roadmap.sh Scalable E-Commerce Platform project.

## Stack
- .NET 10, ASP.NET Core Web API
- EF Core + SQL Server
- ASP.NET Core Identity + JWT authentication
- xUnit

## Features
- Register/login with JWT; Customer and Admin roles
- Categories and products (paged, filtered, soft delete)
- Saved addresses
- Cart with stock validation
- Checkout with price and address snapshots
- Mock payment with atomic stock reservation and protection against double payment
- Order history; admin order search by status, user, category and date range

## Getting started

**Prerequisites:** .NET 10 SDK, SQL Server (Express or Developer)

1. Clone and restore:
   ```bash
   git clone <repo-url>
   cd basic-ecommerce-platform
   dotnet restore
   ```
2. Set secrets:
   ```bash
   cd src/basic-ecommerce-platform
   dotnet user-secrets set "Jwt:Key" "<64-byte base64 key>"
   dotnet user-secrets set "Admin:Email" "admin@example.com"
   dotnet user-secrets set "Admin:Password" "<password>"
   ```
3. Update `ConnectionStrings:DefaultConnection` in `appsettings.json` if needed, then apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run:
   ```bash
   dotnet run
   ```
   Roles and the admin account are seeded on startup.

## API overview

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Categories | `GET/POST /api/category`, `GET/PUT/DELETE /api/category/{id}` |
| Products | `GET/POST /api/product`, `GET/PUT/DELETE /api/product/{id}` |
| Addresses | `GET/POST /api/address`, `GET/PUT/DELETE /api/address/{id}` |
| Cart | `GET/DELETE /api/cart`, `POST /api/cart/items`, `PUT/DELETE /api/cart/items/{productId}` |
| Orders | `GET/POST /api/order`, `GET /api/order/{id}`, `POST /api/order/{id}/pay` |
| Admin | `GET /api/admin/product`, `GET /api/admin/product/{id}`, `GET /api/admin/order`, `GET /api/admin/order/{id}` |

### Mock payment tokens
| Token | Result |
|---|---|
| `tok_visa` | Success |
| `tok_chargeDeclined` | Declined |
| `tok_insufficientFunds` | Declined |