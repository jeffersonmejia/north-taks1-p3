# 1. Summary

NorthwindStore is an ASP.NET Core MVC project for the Northwind purchasing and inventory flow — product browsing, shopping cart, orders, stock updates, admin reports, authentication, soft delete, session control, and caching.

# 2. Technologies Used

| Technology | Version |
|---|---|
| .NET SDK (ASP.NET Core MVC, C#, LINQ) | 10.0.301 |
| ASP.NET Core (runtime, MVC, Identity) | 10.0.2 |
| Entity Framework Core | 10.0.2 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.2 |
| PostgreSQL (database server) | 17.10 |
| Serilog.AspNetCore | 10.0.0 |
| Serilog.Formatting.Compact | 3.0.0 |
| Serilog.Sinks.File | 7.0.0 |
| HTML / CSS | — |
| JavaScript | ES2024+ |
| Git | 2.47.3 |
| GitHub | — |

# 3. Architecture

Layered Architecture with Repository + Service pattern. Data flows one-way:

```
Views/Controllers  →  Services  →  Repositories  →  Data/DbContext  →  PostgreSQL
```

Each layer depends only on the one below. Cross-cutting concerns (caching, auth, session) live in `Infrastructure/`.

| Layer | Folder | Responsibility |
|---|---|---|
| Presentation | `Controllers/`, `Views/`, `Models/ViewModels/` | HTTP handling, UI, DTOs |
| Application | `Services/` | Business logic, caching, orchestration |
| Data Access | `Repositories/` | LINQ queries, persistence |
| Persistence | `Data/` | DbContexts, migrations, seeders |
| Domain | `Models/Northwind/`, `Models/Identity/` | Entities and identity |
| Infrastructure | `Infrastructure/` | Middleware, filters, extensions, helpers |

All service and repository interfaces sit next to their implementations.

## 3.1 Package diagram

```mermaid
graph LR
  P["Presentation<br/>8 Controllers → Razor Views ↔ ViewModels"]
  App["Application<br/>CartSvc, OrderSvc, ProductSvc, InventorySvc, Cache"]
  DAL["Data Access<br/>ProductRepo, OrderRepo — LINQ, persistence"]
  Per["Persistence<br/>NorthwindCtx, AppDbContext — EF Core, seeding"]
  Dom["Domain<br/>Product, Order, Customer, OrderDetail, AppUser"]
  XCut["Cross-Cutting<br/>SingleSession MW, ValidateCart, DI, helpers"]

  P --> App --> DAL --> Per --> Dom
  XCut -.-> P
  XCut -.-> App
  XCut -.-> DAL
  XCut -.-> Per
```

# 4. Installation

## 4.1 Prerequisites

### 4.1.1 Git

a) **Debian** — `sudo apt update && sudo apt install -y git`

b) **Windows** — Download the installer from [https://git-scm.com/download/win](https://git-scm.com/download/win) and run it with default options.

### 4.1.2 .NET SDK

a) **Debian** — Register the Microsoft feed and install:

```bash
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo tee /usr/share/keyrings/microsoft-prod.gpg > /dev/null
sudo wget -qO /etc/apt/sources.list.d/microsoft-prod.list https://packages.microsoft.com/config/debian/13/prod.list
sudo apt update && sudo apt install -y dotnet-sdk-10.0
```

For other Debian versions, follow the [official instructions](https://learn.microsoft.com/dotnet/core/install/linux-debian).

b) **Windows** — Download the **.NET SDK 10.0** installer from [https://dotnet.microsoft.com/en-us/download/dotnet/10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and run it.

### 4.1.3 PostgreSQL

a) **Debian**
```bash
sudo apt update
sudo apt install -y postgresql postgresql-client
sudo systemctl enable --now postgresql
sudo -u postgres psql -c "ALTER USER postgres PASSWORD 'postgres';"
```

b) **Windows** — Download the installer from [https://www.postgresql.org/download/windows/](https://www.postgresql.org/download/windows/). Run it, set the superuser password to `postgres`, keep port `5432`. After installation, add `C:\Program Files\PostgreSQL\17\bin` to `PATH`.

## 4.2 Clone the repository

```bash
git clone https://github.com/jeffersonmejia/north-taks1-p3
cd north-taks1-p3
```

## 4.3 Restore NuGet packages

```bash
dotnet restore
```

## 4.4 Set up the database

### 4.4.1 Create the databases

```bash
createdb northwind
createdb northwind_identity
```

### 4.4.2 Create the application database user

Run the credentials script against both databases:

```bash
psql -d northwind          -f db/credentials.sql
psql -d northwind_identity -f db/credentials.sql
```

This creates the `jef` role with a hashed password and grants the required privileges.

### 4.4.3 Load the schema, seed data, and indexes

```bash
psql -d northwind -f db/schema.sql
psql -d northwind -f db/seed.sql
psql -d northwind -f db/index.sql
```

### 4.4.4 Database indexes

Performance indexes are defined in `db/index.sql` (run separately after the schema). They include:

| Index | Column(s) | Purpose |
|---|---|---|
| `ix_products_available` | `discontinued`, `units_in_stock` (partial, `is_deleted = false`) | Speeds up the available-products listing |
| `ix_products_name_trgm` | `product_name` (GIN trigram, partial `is_deleted = false`) | Accelerates `ILIKE` search by product name |
| `ix_products_low_stock` | `units_in_stock`, `discontinued` (partial, in-stock only) | Fast low-stock reports |
| `ix_products_discontinued` | `discontinued` (partial, `discontinued = true`) | Fast discontinued-products query |
| `ix_order_details_product` | `product_id` | Speeds up the most-purchased-products aggregation |
| `ix_orders_customer_date` | `customer_id`, `order_date` (partial, `is_deleted = false`) | Fast customer order history and sorting |

The trigram index requires the `pg_trgm` extension, which is enabled by `db/index.sql`.

### 4.4.5 Scaffold the models (Database First)

Generate the Northwind models from the live PostgreSQL database:

```bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=northwind;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Idle Lifetime=300" Npgsql.EntityFrameworkCore.PostgreSQL --context NorthwindContext --context-dir Data --output-dir Models/Northwind --force --use-database-names
```

After scaffolding, keep the soft-delete columns from `db/schema.sql`. The app uses query filters for `is_deleted`, `deleted_at`, and `deleted_by`.

## 4.5 Configure secrets

The application reads PostgreSQL credentials from `Secrets/secrets.json`. Create or edit the file with your database password:

```json
{
  "ConnectionStrings": {
    "NorthwindConnection": "Host=localhost;Port=5432;Database=northwind;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Idle Lifetime=300",
    "IdentityConnection": "Host=localhost;Port=5432;Database=northwind_identity;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=50;Connection Idle Lifetime=300"
  }
}
```

Replace `<your-password>` with the password set by `db/credentials.sql`.

`Secrets/secrets.json` is listed in `.gitignore` so it stays local and is never committed. `appsettings.json` does not store database passwords.

## 4.6 Run the project

```bash
dotnet run
```

For watch mode:

```bash
dotnet watch run
```

When the app starts, `IdentitySeeder` creates the `Admin`, `Customer`, and `Employee` roles. `Employee` is kept for compatibility with previous academic work; this project protects the required routes with `Admin` and `Customer`.

## 4.7 Default admin user

The identity seed creates a default academic admin account:

| Field | Value |
|---|---|
| Email | `admin@northwind.local` |
| Password | `Admin123!` |
| Role | `Admin` |

The password is stored as an ASP.NET Core Identity password hash, not as plain text in the database.

To apply or repair this admin user in an existing `northwind_identity` database without recreating the databases, run:

```bash
psql -d postgres -f db/identity_admin_seed.sql
```

## 4.8 Protected routes

The root route `/` is a neutral home page and does not expose product, inventory, order, or customer data.

Protected MVC routes:

| Area | Controller | Access |
|---|---|---|
| Home | `HomeController.Index` | Public |
| Authentication | `AccountController.Login`, `Register`, `AccessDenied` | Public |
| Products | `ProductsController.Index`, `Details` | `Customer` only |
| Cart | `CartController` | `Customer` only |
| Customer orders | `OrdersController` | `Customer` only |
| Admin inventory | `AdminInventoryController` | `Admin` only |
| Admin orders | `AdminOrdersController` | `Admin` only |
| Status pages | `StatusController` | Public/internal error flow |

The navbar follows the same rules: product/cart/order links appear only for `Customer`, while inventory/order administration links appear only for `Admin`.

## 4.9 Publish in Release mode

```bash
dotnet publish -c Release -o ./publish
```

The `publish` folder contains the compiled application. Run it with:

```bash
dotnet ./publish/NorthwindStore.dll
```

# 5. Logging

Logging is configured with **Serilog** in `Infrastructure/Extensions/LoggingExtensions.cs`.

Detailed request/database logs are enabled only when `ASPNETCORE_ENVIRONMENT=Development`.
In non-development environments, the app writes only `Warning` and higher events to the console and does not create `logs/app-*.json` files. This avoids filling disk and slowing down production with high-volume EF query logs.

Development sinks:

| Output | Configuration |
|---|---|
| Console | `.WriteTo.Console()` |
| Rolling file (JSON) | `.WriteTo.File(new CompactJsonFormatter(), "logs/app-.json", ...)` |

Development file rolling policy:

| Setting | Value | Description |
|---|---|---|
| `rollingInterval` | `Day` | Creates a new file each day (`app-20260704.log`) |
| `fileSizeLimitBytes` | 10 MB | Rotates early if a file exceeds this size |
| `rollOnFileSizeLimit` | `true` | Enables size-based rotation in addition to daily |
| `retainedFileCountLimit` | 7 | Keeps only the last 7 files, older ones are deleted automatically |

File format (compact JSON, one event per line):

| Property | Description |
|---|---|
| `@t` | Timestamp (ISO 8601) |
| `@mt` | Message template |
| `@l` | Level (`INF`, `WRN`, `ERR`, `FTL`) |
| `@x` | Exception details (present on errors) |
| `@m` | Rendered message |

Example line:
```json
{"@t":"2026-07-04T15:29:46.316Z","@mt":"Database operation failed","@l":"ERROR","@x":"Npgsql.PostgresException: 42501: permission denied..."}
```

Query examples:
```bash
# Filter errors only
jq 'select(.["@l"] == "ERR")' logs/app-*.json

# Search for a specific message
jq 'select(.["@mt"] | test("product"))' logs/app-*.json
```
