# StockitApi — Documentation & Feature Map

## Overview

StockitApi is a .NET Web API backend for a stock/inventory management system ("Stockit"). It covers products, inventory, invoicing/payments, customers, users/roles, analytics, and observability (audit logs + API usage metrics). It appears designed to power an admin/mobile client — product lookup supports barcode/QR scanning, and usage metrics track per-request telemetry.

## Tech Stack

- **.NET 10** Web API (nullable + implicit usings enabled)
- **JWT Bearer authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **EF Core + SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`)
- **OpenAPI** doc generation, exposed at `/openapi` in Development
- **Docker** support (Linux container target)

## Architecture

| Layer | Responsibility |
|---|---|
| `Controllers/` | Thin API endpoints, delegate to services |
| `Services/` | Business logic (`AnalyticsService`, `AuthenticationService`, `InvoiceService`, `ObservabilityService`, `ProductService`, `UserService`, `TokenService`, `PasswordHasherService`, `AppRequestContext`) |
| `Interfaces/` | Service/repository abstractions (`IAsyncRepository`, `IUnitOfWork`, `ISpecification`, etc.) |
| `Data/` | `StockitContext` (EF Core), `Repositories/`, `Migrations/`, `Configuration/`, `DatabaseSeeder` |
| `Entities/` | EF Core domain model |
| `Dtos/` | Request/response contracts |
| `Authorization/` | Custom permission-based authorization |
| `Middlewares/` | `UsageMetricsMiddleware` |
| `Extensions/` | DI/composition-root wiring used by `Program.cs` |
| `Models/` | Cross-cutting config (`Permissions`, `Roles`, `JwtOptions`) |

### Startup pipeline ([StockitApi/Program.cs](../StockitApi/Program.cs))

Services: `AddHttpContextAccessor` → `AddInfrastructureServices` (DbContext, JWT options, `IUnitOfWork`, `IAppRequestContext`, `IPasswordHasherService`, `ITokenService`) → `AddApplicationServices` → `AddJwtAuthentication` → `AddPermissionAuthorization` → `AddControllers` / `AddOpenApi`.

On startup: optionally runs EF Core migrations (`ApplyMigrations` config flag), then always runs `DatabaseSeeder.SeedAsync`.

Request pipeline: `UseHttpsRedirection` → `UseAuthentication` → `UseAuthorization` → `UsageMetricsMiddleware` → `MapControllers`.

## Domain Entities

All entities inherit `BaseEntity<TKey>` (an `Id` primary key).

| Entity | Key | Description |
|---|---|---|
| `Product` | int | Catalog item: name, barcode, QR code, price, category, active flag |
| `Category` | int | Product category, supports parent/child hierarchy |
| `Inventory` | int | Stock quantity of a product at a warehouse |
| `StockMovement` | long | Stock change ledger (Sale, Restock, Transfer, Adjustment, Return) |
| `Warehouse` | int | Physical storage location |
| `Customer` | int | Buyer record with contact info and invoices |
| `Invoice` | int | Sales invoice: number, totals, status (Pending/Paid/Refunded/Partially Paid/Overdue/Void), items, payments |
| `InvoiceItem` | int | Line item on an invoice |
| `Payment` | int | Payment against an invoice (Cash, Credit Card, Bank Transfer, PayPal, Check) |
| `User` | int | System account: credentials, role, active flag |
| `Role` | int | Named role with associated permissions |
| `Permission` | int | Permission code catalog entry |
| `RolePermission` | composite | Join entity: Role ↔ Permission |
| `RefreshToken` | Guid | JWT session/refresh record, supports revocation |
| `Notification` | Guid | User notification (title, message, read flag) |
| `AuditLog` | Guid | Records entity changes (action, entity type/id, JSON diff, user, timestamp) |
| `ApiUsageLog` | Guid | Per-request telemetry (endpoint, method, status, IP, user agent, response time) |

## Auth & Permission System

**Authentication:** JWT bearer tokens carry a `Permissions` claim and an `rt_id` (refresh token ID) claim. On every token validation, the refresh token is checked against the DB so revoked sessions are rejected even with a still-valid JWT.

**Authorization:** Custom permission-based policies (`Authorization/` folder):
- `[RequirePermission(Permission.X.Y)]` attribute maps to a dynamic policy `Permission:{value}`.
- `PermissionPolicyProvider` builds policies on demand (no need to pre-register one per permission).
- `PermissionAuthorizationHandler` succeeds if the user's `Permissions` claim matches the required permission (case-insensitive).

**Permission catalog** ([Models/Permissions.cs](../StockitApi/Models/Permissions.cs)):

| Group | Permissions |
|---|---|
| User | `users.read`, `users.write`, `users.delete` |
| Role | `roles.manage` |
| Product | `products.read`, `products.write`, `products.delete` |
| Inventory | `inventory.read`, `inventory.adjust` |
| Customer | `customers.read`, `customers.write` |
| Invoice | `invoices.create`, `invoices.read`, `invoices.update`, `invoices.void`, `invoices.export` |
| Analytics | `analytics.read` |
| Logs | `logs.read` |
| Metrics | `metrics.read` |

**Roles** ([Models/Roles.cs](../StockitApi/Models/Roles.cs)):

| Role | Permissions |
|---|---|
| Admin | all permissions |
| Manager | users.read, products.read/write, inventory.read/adjust, customers.read/write, invoices.create/read/void/export, analytics.read, metrics.read |
| SalesRep | products.read, inventory.read, customers.read/write, invoices.create/read |
| WarehouseStaff | products.read, inventory.read/adjust, invoices.read |
| Auditor | users.read, products.read, inventory.read, customers.read, invoices.read/export, analytics.read, logs.read, metrics.read |

> Note: `roles.manage`, `inventory.*`, `customers.*`, `invoices.update`, and `invoices.export` are defined in the permission catalog and role sets but have no wired controller endpoint yet — likely planned features (role management, inventory adjustments, customer management, invoice export).

## Middleware

**`UsageMetricsMiddleware`** ([StockitApi/Middlewares/UsageMetricsMiddleware.cs](../StockitApi/Middlewares/UsageMetricsMiddleware.cs)) runs after auth/authorization, times each request, and persists an `ApiUsageLog` (user, path, method, status code, IP, user agent, response time). Logging failures are swallowed so metrics never break the actual response. Backs `GET /api/metrics/usage`.

## Feature Map / Endpoints

### Authentication & Sessions — `api/auth`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| POST | `api/auth/login` | anonymous | Log in, returns access/refresh tokens |
| POST | `api/auth/refresh-token` | anonymous | Exchange refresh token for a new token pair |
| POST | `api/auth/logout` | authenticated | Revoke a given refresh token |
| POST | `api/auth/register` | `users.write` | Create a new user account |
| GET | `api/auth/me` | authenticated | Get current user's profile |
| GET | `api/auth/sessions` | authenticated | List current user's active sessions |
| POST | `api/auth/sessions/{id:guid}/revoke` | authenticated | Revoke one session |
| POST | `api/auth/sessions/revoke` | authenticated | Revoke all sessions (optionally keep current) |

### Products — `api/products`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| GET | `api/products` | `products.read` | List all products |
| GET | `api/products/{id:int}` | `products.read` | Get product by ID |
| GET | `api/products/lookup?code=` | `products.read` | Look up product by barcode/QR code |
| POST | `api/products` | `products.write` | Create product |
| PUT | `api/products/{id:int}` | `products.write` | Update product |
| DELETE | `api/products/{id:int}` | `products.delete` | Deactivate (soft-delete) product |

### Invoices — `api/invoices`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| GET | `api/invoices` | `invoices.read` | Paged list of invoices |
| GET | `api/invoices/{id:int}` | `invoices.read` | Get invoice by ID |
| POST | `api/invoices` | `invoices.create` | Create invoice |
| POST | `api/invoices/{id:int}/void` | `invoices.void` | Void invoice |

### Users — `api/users`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| GET | `api/users` | `users.read` | List all users |
| GET | `api/users/{id:int}` | `users.read` | Get user by ID |
| PUT | `api/users/{id:int}` | `users.write` | Update user |
| DELETE | `api/users/{id:int}` | `users.delete` | Deactivate user |
| POST | `api/users/{id:int}/change-password` | `users.write` | Change user password |

### Analytics — `api/analytics`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| GET | `api/analytics/summary` | `analytics.read` | Aggregate summary for optional date range |
| GET | `api/analytics/sales-by-day` | `analytics.read` | Daily sales breakdown for a date range |

### Observability — `api`

| Verb | Route | Auth | Purpose |
|---|---|---|---|
| GET | `api/logs/audit` | `logs.read` | Paged audit log entries |
| GET | `api/metrics/usage` | `metrics.read` | API usage metrics since optional date |

## Planned / Not-Yet-Exposed Features

Based on the permission catalog and role definitions, the following are modeled but not yet exposed via controllers:
- Role management (`roles.manage`)
- Inventory read/adjust endpoints (`inventory.read`, `inventory.adjust`)
- Customer management endpoints (`customers.read`, `customers.write`)
- Invoice update and export (`invoices.update`, `invoices.export`)
