# Smart Warehouse & Inventory Management API

A professional **.NET 8 Clean Architecture Web API** for warehouse and inventory management, designed to showcase enterprise-level backend patterns for technical interviews.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Business Problem](#business-problem)
- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Database Design](#database-design)
- [Authentication & Authorization](#authentication--authorization)
- [CQRS & MediatR](#cqrs--mediatr)
- [Business Rules](#business-rules)
- [Concurrency Strategy](#concurrency-strategy)
- [Transactions](#transactions)
- [Hangfire Background Jobs](#hangfire-background-jobs)
- [API Endpoints](#api-endpoints)
- [How to Run](#how-to-run)
- [Database Migration](#database-migration)
- [Swagger](#swagger)
- [Sample Users & Roles](#sample-users--roles)
- [Project Backlog](#project-backlog)

---

## Project Overview

This is a modular monolithic Web API that manages:
- **Products** organized by categories
- **Warehouses** with employee assignments
- **Inventory** tracking across multiple warehouses
- **Purchase Orders** (supplier → warehouse receiving)
- **Sales Orders** (warehouse → customer shipping)
- **Stock Transfers** (warehouse → warehouse)
- **Notifications** and **Audit Logs**
- **Background Jobs** for low-stock detection and daily summaries
- **Dashboard** and **Reports** with filtering and pagination

---

## Business Problem

A mid-to-large company operates multiple warehouses. They need to:
1. Track what products are stored in which warehouse and how many units are available.
2. Receive stock from suppliers via Purchase Orders.
3. Ship stock to customers via Sales Orders.
4. Transfer stock between warehouses.
5. Prevent overselling (selling more than what's available).
6. Handle concurrent operations (two employees trying to reserve the last item).
7. Detect low-stock products automatically and notify administrators.
8. Maintain a full audit trail of business operations.

---

## Features

| Feature | Pattern / Technology |
|---|---|
| Clean Architecture | Domain → Application → Infrastructure → API |
| CQRS | Separate Commands and Queries via MediatR |
| Authentication | JWT + Refresh Tokens via ASP.NET Core Identity |
| Authorization | Role-Based (`[Authorize(Roles = "...")]`) |
| Validation | FluentValidation with MediatR pipeline behavior |
| Concurrency | Optimistic concurrency with `RowVersion` on Inventory |
| Transactions | `IDbContextTransaction` for stock receive/ship/transfer |
| Background Jobs | Hangfire recurring jobs (hourly low-stock, daily summary) |
| Audit Logging | `IAuditService` for tracking business operations |
| Notifications | In-app notifications with duplicate prevention |
| Reports | Inventory, Sales, Purchases, Stock Movements, Low Stock |
| Dashboard | Aggregated KPIs via optimized database queries |
| Pagination | `PaginatedList<T>` with `PageNumber`, `PageSize`, `TotalCount` |
| Exception Handling | Global middleware returning `ProblemDetails` |
| Specification Pattern | Reusable query specifications with criteria, includes, ordering |

---

## Architecture

```
SmartWarehouse.API          → ASP.NET Core Controllers, Middleware, Swagger
        ↓
SmartWarehouse.Application  → Commands, Queries, Handlers, DTOs, Validators, Interfaces
        ↓
SmartWarehouse.Domain       → Entities, Enums, Exceptions, Value Objects, Base Classes

SmartWarehouse.Infrastructure → EF Core, Identity, JWT, Repositories, Services
        ↓
SmartWarehouse.Application
        ↓
SmartWarehouse.Domain
```

**Dependency Rule:** Dependencies always point inward. Domain has zero external dependencies. Application defines interfaces. Infrastructure implements them. API is the composition root.

---

## Technology Stack

| Technology | Purpose |
|---|---|
| .NET 8 | Runtime |
| ASP.NET Core Web API | HTTP layer |
| Entity Framework Core | ORM |
| SQL Server (LocalDB) | Database |
| ASP.NET Core Identity | User management, password hashing |
| JWT Bearer | Authentication tokens |
| MediatR | Mediator / CQRS |
| FluentValidation | Input validation |
| Hangfire | Background job processing |
| Swagger / Swashbuckle | API documentation |

---

## Database Design

### Core Entities

| Entity | Key Type | Purpose |
|---|---|---|
| `User` | `string` (GUID) | Identity user with FirstName, LastName |
| `Role` | `string` (GUID) | Identity role |
| `RefreshToken` | `int` | JWT refresh token storage |
| `Category` | `int` | Product classification |
| `Product` | `int` | SKU, Name, Price, MinimumStockLevel |
| `Warehouse` | `int` | Name, Location, IsActive |
| `WarehouseEmployee` | composite | Many-to-many User↔Warehouse |
| `Inventory` | `int` | Quantity + ReservedQuantity per Product/Warehouse |
| `InventoryMovement` | `long` | Immutable audit record of every stock change |
| `Supplier` | `int` | External supplier |
| `Customer` | `int` | External customer |
| `PurchaseOrder` | `int` | Order from supplier with lifecycle |
| `PurchaseOrderItem` | `int` | Line item |
| `SalesOrder` | `int` | Order to customer with lifecycle |
| `SalesOrderItem` | `int` | Line item |
| `StockTransfer` | `int` | Warehouse-to-warehouse transfer |
| `StockTransferItem` | `int` | Line item |
| `Notification` | `long` | In-app notification |
| `AuditLog` | `long` | Business operation audit trail |

### Key Indexes
- `Product.SKU` (unique)
- `Inventory(ProductId, WarehouseId)` (unique composite)
- `InventoryMovement(ProductId, WarehouseId)`
- `PurchaseOrder.Status`
- `SalesOrder.Status`

### Concurrency Token
- `Inventory.RowVersion` — `byte[]` configured as `IsRowVersion()` for optimistic concurrency.

---

## Authentication & Authorization

### Authentication Flow
1. `POST /api/auth/register` — Create user account
2. `POST /api/auth/login` — Returns JWT access token + refresh token
3. `POST /api/auth/refresh-token` — Exchange expired JWT for a new one
4. `POST /api/auth/logout` — Revoke refresh token

### Roles & Permissions

| Role | Permissions |
|---|---|
| **Admin** | Full system access, user management |
| **WarehouseManager** | Manage warehouses, inventory, transfers |
| **Employee** | View inventory, perform assigned operations |
| **Sales** | Create and manage sales orders |
| **Purchasing** | Create and manage purchase orders |

---

## CQRS & MediatR

Every use case is implemented as either a **Command** (write) or **Query** (read):

```
Controller → mediator.Send(Command/Query) → Handler → Response
```

**Commands** modify state: `CreateProductCommand`, `ReceivePurchaseOrderCommand`, `ShipSalesOrderCommand`

**Queries** read state: `GetProductsQuery`, `GetDashboardQuery`, `GetInventoryReportQuery`

**Why MediatR?**
- Decouples controllers from business logic
- Enables pipeline behaviors (validation, logging)
- Each handler follows Single Responsibility Principle
- Easy to add cross-cutting concerns

---

## Business Rules

### Purchase Order Lifecycle
```
Draft → Submitted → Approved → Received
                  ↘ Rejected
       ↘ Cancelled
```
- **Receive:** Transaction wraps inventory increase + movement creation + status update.

### Sales Order Lifecycle
```
Draft → Confirmed → Shipped → Completed
      ↘ Cancelled
```
- **Confirm:** Checks `AvailableQuantity = Quantity - ReservedQuantity`. Reserves stock.
- **Ship:** Transaction decreases both `Quantity` and `ReservedQuantity`, creates `Sale` movements.
- **Cancel:** Releases reserved quantity.

### Stock Transfer
```
Pending → Completed
        ↘ Cancelled
```
- Source and destination warehouses must differ.
- Atomic: source decrement + destination increment + movements in one transaction.

### Inventory Invariants
- `Quantity >= 0` always
- `ReservedQuantity >= 0` always
- `ReservedQuantity <= Quantity` always
- Every change creates an `InventoryMovement` record

---

## Concurrency Strategy

### The Problem
Two employees simultaneously try to sell the last unit of a product:
```
Inventory.Quantity = 1

Employee A reads Quantity = 1, tries to reserve 1
Employee B reads Quantity = 1, tries to reserve 1

Without protection: Quantity = -1 (INVALID!)
```

### The Solution: Optimistic Concurrency
1. `Inventory` entity has a `RowVersion` column (`byte[]`).
2. EF Core includes `WHERE RowVersion = @originalValue` in UPDATE statements.
3. If another transaction modified the row, the `WHERE` matches zero rows.
4. EF Core throws `DbUpdateConcurrencyException`.
5. Our handler catches it and returns **409 Conflict**.

### Why Not Pessimistic Locking?
- Pessimistic locks reduce throughput and increase deadlock risk.
- Optimistic concurrency is ideal for scenarios with infrequent conflicts.
- The client can simply retry the operation.

---

## Transactions

Critical multi-table operations use explicit `IDbContextTransaction`:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    // 1. Update Inventory
    // 2. Create InventoryMovement
    // 3. Update Order status
    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

**Used in:** Purchase Order Receive, Sales Order Ship, Stock Transfer Complete.

---

## Hangfire Background Jobs

| Job | Schedule | Purpose |
|---|---|---|
| `LowStockCheckJob` | Every hour | Detect products where `AvailableQuantity <= MinimumStockLevel`, create notifications |
| `DailyInventorySummaryJob` | Daily at midnight | Log total products, total quantity, inventory value, low-stock count |

**Design principles:**
- **Idempotent** — Duplicate notifications are prevented by checking for existing unread notifications.
- **Retry-safe** — If a job fails, Hangfire retries automatically.
- **Logged** — All job executions are logged via `ILogger`.

**Dashboard:** Visit `/hangfire` to monitor job status.

---

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT |
| POST | `/api/auth/refresh-token` | Refresh expired JWT |
| POST | `/api/auth/logout` | Revoke refresh token |

### Categories
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| GET | `/api/categories` | List all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| DELETE | `/api/categories/{id}` | Delete category |

### Products
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| GET | `/api/products` | List products (paginated, filterable) |
| GET | `/api/products/{id}` | Get product by ID |
| PATCH | `/api/products/{id}/activate` | Activate product |
| PATCH | `/api/products/{id}/deactivate` | Deactivate product |

### Warehouses
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/warehouses` | Create warehouse |
| PUT | `/api/warehouses/{id}` | Update warehouse |
| GET | `/api/warehouses` | List warehouses |
| GET | `/api/warehouses/{id}` | Get warehouse by ID |
| POST | `/api/warehouses/{id}/employees/{empId}` | Assign employee |
| DELETE | `/api/warehouses/{id}/employees/{empId}` | Unassign employee |
| GET | `/api/warehouses/{id}/employees` | List warehouse employees |

### Inventory
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory` | List inventory (paginated, filterable) |
| GET | `/api/inventory/{productId}/{warehouseId}` | Get specific inventory |
| GET | `/api/warehouses/{id}/inventory` | Warehouse inventory |

### Inventory Movements
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory/movements` | List movements (filterable) |
| GET | `/api/inventory/movements/{id}` | Get movement by ID |

### Purchase Orders
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/purchase-orders` | Create purchase order |
| GET | `/api/purchase-orders` | List purchase orders |
| GET | `/api/purchase-orders/{id}` | Get purchase order |
| POST | `/api/purchase-orders/{id}/submit` | Submit for approval |
| POST | `/api/purchase-orders/{id}/approve` | Approve |
| POST | `/api/purchase-orders/{id}/receive` | Receive (transactional) |
| POST | `/api/purchase-orders/{id}/cancel` | Cancel |

### Sales Orders
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/sales-orders` | Create sales order |
| GET | `/api/sales-orders` | List sales orders |
| GET | `/api/sales-orders/{id}` | Get sales order |
| POST | `/api/sales-orders/{id}/confirm` | Confirm (reserves stock) |
| POST | `/api/sales-orders/{id}/ship` | Ship (transactional) |
| POST | `/api/sales-orders/{id}/complete` | Mark completed |
| POST | `/api/sales-orders/{id}/cancel` | Cancel (releases stock) |

### Stock Transfers
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/stock-transfers` | Create transfer |
| GET | `/api/stock-transfers` | List transfers |
| GET | `/api/stock-transfers/{id}` | Get transfer |
| POST | `/api/stock-transfers/{id}/complete` | Complete (transactional) |
| POST | `/api/stock-transfers/{id}/cancel` | Cancel |

### Notifications
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/notifications` | Get user notifications |
| PATCH | `/api/notifications/{id}/read` | Mark as read |
| PATCH | `/api/notifications/read-all` | Mark all as read |

### Dashboard
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/dashboard` | Get aggregated KPIs |

### Reports
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/reports/inventory` | Inventory report |
| GET | `/api/reports/sales` | Sales report |
| GET | `/api/reports/purchases` | Purchase report |
| GET | `/api/reports/stock-movements` | Stock movement report |
| GET | `/api/reports/low-stock` | Low stock report |

---

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB is configured by default)

### Steps

```bash
# 1. Clone the repository
git clone <repository-url>
cd SmartWarehouse

# 2. Restore packages
dotnet restore

# 3. Apply database migrations
dotnet ef database update --project src/SmartWarehouse.Infrastructure --startup-project src/SmartWarehouse.API

# 4. Run the API
dotnet run --project src/SmartWarehouse.API

# 5. Open Swagger
# Navigate to: https://localhost:<port>/swagger
```

---

## Database Migration

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> \
    --project src/SmartWarehouse.Infrastructure \
    --startup-project src/SmartWarehouse.API \
    --output-dir Data/Migrations

# Update database
dotnet ef database update \
    --project src/SmartWarehouse.Infrastructure \
    --startup-project src/SmartWarehouse.API

# Remove last migration (if not applied)
dotnet ef migrations remove \
    --project src/SmartWarehouse.Infrastructure \
    --startup-project src/SmartWarehouse.API
```

> **Note:** The application also auto-migrates on startup in `Program.cs`.

---

## Swagger

1. Run the API.
2. Navigate to `https://localhost:<port>/swagger`.
3. Call `POST /api/auth/login` with admin credentials.
4. Copy the `token` from the response.
5. Click the **Authorize** button (🔒) at the top.
6. Enter: `Bearer <your_token>`
7. Click **Authorize**.
8. All subsequent requests will include the JWT.

---

## Sample Users & Roles

| Email | Password | Role |
|---|---|---|
| `admin@smartwarehouse.com` | `Admin@123!` | Admin |
| `warehouse.manager@smartwarehouse.com` | `Manager@123!` | WarehouseManager |
| `sales@smartwarehouse.com` | `Sales@123!` | Sales |
| `purchasing@smartwarehouse.com` | `Purchasing@123!` | Purchasing |
| `employee@smartwarehouse.com` | `Employee@123!` | Employee |

### Pre-seeded Demo Data
- **5 Categories:** Electronics, Office Supplies, Raw Materials, Packaging, Furniture
- **8 Products:** Wireless Mouse, USB-C Hub, Mechanical Keyboard, A4 Paper, Pens, Steel Sheet, Boxes, Bubble Wrap
- **3 Warehouses:** Main Warehouse (Dallas), East Coast Hub (Newark), West Coast Hub (Los Angeles)
- **3 Suppliers:** TechParts Inc., Global Office Supply, Industrial Materials Co.
- **3 Customers:** Acme Corporation, Beta Industries, Gamma Solutions

---

## Project Backlog

### Epic 1: Core Infrastructure
| User Story | Acceptance Criteria |
|---|---|
| As a developer, I want Clean Architecture so layers are independently testable | Domain has zero dependencies; API depends on Application |
| As a developer, I want global exception handling so errors return consistent ProblemDetails | All exceptions map to appropriate HTTP status codes |
| As a developer, I want Swagger configured with JWT so I can test protected endpoints | Bearer token input works, all endpoints visible |

### Epic 2: Authentication & Authorization
| User Story | Acceptance Criteria |
|---|---|
| As a user, I want to register and login so I can access the system | Register returns 201, Login returns JWT + refresh token |
| As an admin, I want role-based access so only authorized users can perform actions | Unauthorized users receive 403 Forbidden |
| As a user, I want refresh tokens so I don't have to login repeatedly | Expired JWT can be refreshed without re-entering credentials |

### Epic 3: Product & Category Management
| User Story | Acceptance Criteria |
|---|---|
| As an admin, I want to create products with unique SKUs | Duplicate SKU returns 400 |
| As an admin, I want to manage categories | Categories with products cannot be deleted (400) |
| As a user, I want to search and filter products | Pagination, search, category filter, sort all work |

### Epic 4: Warehouse & Inventory
| User Story | Acceptance Criteria |
|---|---|
| As a manager, I want to create warehouses and assign employees | Assignment prevents duplicates |
| As a user, I want to view inventory levels per warehouse | Available = Quantity - Reserved is calculated correctly |
| As a user, I want to filter low-stock products | lowStock=true returns products below minimum |

### Epic 5: Purchase Orders
| User Story | Acceptance Criteria |
|---|---|
| As purchasing, I want to create and submit purchase orders | Only Draft → Submitted is allowed |
| As a manager, I want to approve purchase orders | Only Submitted → Approved is allowed |
| As a manager, I want to receive purchase orders | Inventory increases atomically, movements created |

### Epic 6: Sales Orders
| User Story | Acceptance Criteria |
|---|---|
| As sales, I want to create and confirm sales orders | Insufficient stock returns 400 |
| As a manager, I want to ship sales orders | Quantity and ReservedQuantity decrease atomically |
| As a user, I want concurrency protection | Two simultaneous reservations don't oversell (409 Conflict) |

### Epic 7: Stock Transfers
| User Story | Acceptance Criteria |
|---|---|
| As a manager, I want to transfer stock between warehouses | Source decreases, destination increases atomically |
| As a user, I want transfer to fail if source has insufficient stock | 400 Bad Request returned |

### Epic 8: Notifications & Audit
| User Story | Acceptance Criteria |
|---|---|
| As a user, I want notifications for important events | Low stock, order approvals create notifications |
| As an admin, I want audit logs for business operations | All critical actions logged with user, entity, timestamp |

### Epic 9: Background Jobs
| User Story | Acceptance Criteria |
|---|---|
| As an admin, I want automatic low-stock detection | Hangfire runs hourly, creates notifications idempotently |
| As an admin, I want daily inventory summaries | Summary logged daily with total value, low-stock count |

### Epic 10: Dashboard & Reports
| User Story | Acceptance Criteria |
|---|---|
| As a manager, I want a dashboard with KPIs | Single endpoint returns all key metrics |
| As a user, I want filterable reports | Date range, warehouse, product, category filters all work |

---

## Design Patterns Demonstrated

| Pattern | Where |
|---|---|
| **CQRS** | All Features (Commands vs Queries) |
| **Mediator** | MediatR dispatches all requests |
| **Repository** | `IGenericRepository<T>` in Infrastructure |
| **Unit of Work** | `IUnitOfWork` wrapping `DbContext.SaveChangesAsync` |
| **Specification** | Reusable query objects with criteria + includes |
| **Strategy** | Different order lifecycle handlers per status transition |

## SOLID Principles in Practice

| Principle | Example |
|---|---|
| **SRP** | Each command handler does exactly one thing |
| **OCP** | New features = new handlers, no existing code modified |
| **LSP** | `BaseEntity<TKey>` substitutable across entity types |
| **ISP** | `IApplicationDbContext`, `IJwtTokenGenerator`, `IAuditService` — small, focused |
| **DIP** | Application defines interfaces; Infrastructure implements them |

---

*Built with ❤️ for technical interview preparation.*
