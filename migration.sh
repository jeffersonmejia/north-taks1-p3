#!/usr/bin/env bash
set -euo pipefail

PASS=1125
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT="$SCRIPT_DIR/NorthwindStore.csproj"
MIGRATIONS_NW="$SCRIPT_DIR/Migrations/Northwind"
MIGRATIONS_ID="$SCRIPT_DIR/Migrations/Identity"
SECRETS="$SCRIPT_DIR/Secrets/secrets.json"

echo "$PASS" | sudo -S -v 2>/dev/null
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

# Clean any leftover scaffolded DbContexts from previous runs
rm -f "$SCRIPT_DIR/Models/Northwind/NorthwindContext.cs" \
      "$SCRIPT_DIR/Models/Identity/ApplicationDbContext.cs"

echo "=== 1/5: Scaffolding models from database (DB First) ==="
conn_nw=$(jq -r '.ConnectionStrings.NorthwindConnection' "$SECRETS")
conn_id=$(jq -r '.ConnectionStrings.IdentityConnection' "$SECRETS")

dotnet ef dbcontext scaffold "$conn_nw" Npgsql.EntityFrameworkCore.PostgreSQL \
    --context NorthwindContext --output-dir "Models/Northwind" \
    --project "$PROJECT" --force 2>/dev/null \
|| dotnet ef dbcontext scaffold "$conn_nw" Npgsql.EntityFrameworkCore.PostgreSQL \
    --context NorthwindContext --output-dir "Models/Northwind" \
    --project "$PROJECT" --force
rm -f "$SCRIPT_DIR/Models/Northwind/NorthwindContext.cs"

dotnet ef dbcontext scaffold "$conn_id" Npgsql.EntityFrameworkCore.PostgreSQL \
    --context ApplicationDbContext --output-dir "Models/Identity" \
    --project "$PROJECT" --force 2>/dev/null \
|| dotnet ef dbcontext scaffold "$conn_id" Npgsql.EntityFrameworkCore.PostgreSQL \
    --context ApplicationDbContext --output-dir "Models/Identity" \
    --project "$PROJECT" --force
rm -f "$SCRIPT_DIR/Models/Identity/ApplicationDbContext.cs"

echo "=== 2/5: Deleting old migrations ==="
rm -rf "$MIGRATIONS_NW" "$MIGRATIONS_ID"

echo "=== 3/5: Creating fresh migration ==="
dotnet ef migrations add Initial \
    --context NorthwindContext --output-dir "$MIGRATIONS_NW" \
    --project "$PROJECT"
dotnet ef migrations add InitialIdentity \
    --context ApplicationDbContext --output-dir "$MIGRATIONS_ID" \
    --project "$PROJECT"

echo "=== 4/5: Applying migrations ==="
dotnet ef database update --context NorthwindContext --project "$PROJECT"
dotnet ef database update --context ApplicationDbContext --project "$PROJECT"

echo "=== 5/5: Verifying build ==="
dotnet build --project "$PROJECT" --no-restore 2>&1 | tail -3

echo "=== All done ==="
