# 1. Summary

NorthwindStore is an ASP.NET Core MVC academic project for a complete Northwind purchasing and inventory flow. It includes product browsing, a temporary shopping cart, order creation, order details, automatic stock updates, administrative inventory reports, authentication, role-based authorization, server-side validations, EF Core transactions, soft delete, single active session control, LINQ queries, PostgreSQL connection pooling, and IMemoryCache invalidation after purchases or inventory changes.

The solution follows this responsibility flow:

Controller -> Service -> Repository -> DbContext -> PostgreSQL

Business logic is kept in services and repositories instead of controllers. The project also includes `db/schema.sql` with the complete PostgreSQL Northwind database adapted for the app, plus `db/seed.sql` for academic seed helpers.

# 2. Technologies Used

* ASP.NET Core MVC
* C#
* PostgreSQL
* Northwind Database
* Entity Framework Core
* Database First
* Npgsql
* ASP.NET Core Identity
* LINQ
* IMemoryCache
* HTML
* CSS
* JavaScript
* Git
* GitHub

# 3. Installation

## 3.1 Clone the repository

```bash
git clone <repository-url>
cd <repository-folder>
```

## 3.2 Create the application database user

Run the credentials script against both databases:

```bash
psql -d northwind          -f db/credentials.sql
psql -d northwind_identity -f db/credentials.sql
```

This creates the `jef` role with a hashed password and grants the required privileges.

## 3.3 Configure database credentials

The application reads PostgreSQL credentials from `Secrets/secrets.json`. The file is already prepared for the local setup:

```json
{
  "ConnectionStrings": {
    "NorthwindConnection": "Host=localhost;Port=5432;Database=northwind;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Idle Lifetime=300",
    "IdentityConnection": "Host=localhost;Port=5432;Database=northwind_identity;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=50;Connection Idle Lifetime=300"
  }
}
```

`Secrets/secrets.json` is listed in `.gitignore` so it stays local and is never committed. `appsettings.json` does not store database passwords. Connection pooling is enabled in the connection strings with `Pooling=true`, `Minimum Pool Size`, `Maximum Pool Size`, and `Connection Idle Lifetime`.

## 3.4 Restore packages

```bash
dotnet restore
```

## 3.5 Prepare PostgreSQL

### 3.5.1 Install PostgreSQL on Debian

```bash
sudo apt update
sudo apt install -y postgresql postgresql-client
sudo systemctl enable --now postgresql
```

Switch to the `postgres` user and set a password for the initial superuser:

```bash
sudo -u postgres psql -c "ALTER USER postgres PASSWORD 'postgres';"
```

### 3.5.2 Install PostgreSQL on Windows

1. Download the installer from [https://www.postgresql.org/download/windows/](https://www.postgresql.org/download/windows/).
2. Run the installer and follow the wizard. When prompted:
   - Set the superuser (`postgres`) password to `postgres`.
   - Keep the default port `5432`.
3. After installation, open **pgAdmin** or **SQL Shell (psql)** and verify the server is running.

Enable `psql` in the terminal by adding PostgreSQL's `bin` directory to your `PATH` (typically `C:\Program Files\PostgreSQL\17\bin`).

### 3.5.3 Create the databases and load the schema

Create the Northwind and Identity databases:

```bash
createdb northwind
createdb northwind_identity
```

Then run the schema, seed and index files:

```bash
psql -d northwind -f db/schema.sql
psql -d northwind -f db/seed.sql
psql -d northwind -f db/index.sql
```

### 3.5.4 Scaffold the models (Database First)

Generate the Northwind models from the live PostgreSQL database:

```bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=northwind;Username=jef;Password=<your-password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Idle Lifetime=300" Npgsql.EntityFrameworkCore.PostgreSQL --context NorthwindContext --context-dir Data --output-dir Models/Northwind --force --use-database-names
```

After scaffolding, keep the soft-delete columns from `db/schema.sql`. The app uses query filters for `is_deleted`, `deleted_at`, and `deleted_by`.

### 3.5.5 Database indexes

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

## 3.6 Run the project

```bash
dotnet run
```

For watch mode:

```bash
dotnet watch run
```

When the app starts, `IdentitySeeder` creates the `Admin`, `Customer`, and `Employee` roles. `Employee` is kept for compatibility with previous academic work; this project protects the required routes with `Admin` and `Customer`.

## 3.7 Publish in Release mode

```bash
dotnet publish -c Release -o ./publish
```

The `publish` folder contains the compiled application. Run it with:

```bash
dotnet ./publish/NorthwindStore.dll
```
