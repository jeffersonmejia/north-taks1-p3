#!/usr/bin/env bash
set -euo pipefail

PASS=1125
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
CONTEXT_NORTHWIND="NorthwindContext"
CONTEXT_IDENTITY="ApplicationDbContext"
PROJECT="$SCRIPT_DIR/NorthwindStore.csproj"

MIGRATIONS_DIR_NORTHWIND="$SCRIPT_DIR/Migrations/Northwind"
MIGRATIONS_DIR_IDENTITY="$SCRIPT_DIR/Migrations/Identity"

echo "$PASS" | sudo -S -v 2>/dev/null
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

action="${1:-help}"

scaffold() {
    local conn="$1" context="$2" output="$3"
    echo "=== Scaffolding $context from database ==="
    dotnet ef dbcontext scaffold "$conn" Npgsql.EntityFrameworkCore.PostgreSQL \
        --context "$context" \
        --output-dir "$output" \
        --project "$PROJECT" \
        --force \
        --no-build 2>/dev/null || {
        dotnet ef dbcontext scaffold "$conn" Npgsql.EntityFrameworkCore.PostgreSQL \
            --context "$context" \
            --output-dir "$output" \
            --project "$PROJECT" \
            --force
    }
}

get_conn() {
    local name="$1"
    dotnet run --project "$PROJECT" --no-build 2>/dev/null &
    local pid=$!
    sleep 2
    kill $pid 2>/dev/null || true
    grep -A2 "\"$name\"" "$SCRIPT_DIR/Secrets/secrets.json" 2>/dev/null \
        | sed -n 's/.*"\(Host.*\)".*/\1/p' || {
        echo "ERROR: Connection string '$name' not found. Check Secrets/secrets.json" >&2
        exit 1
    }
}

case "$action" in
    scaffold)
        echo "=== DB First: Scaffolding models from existing database ==="
        conn_nw=$(jq -r '.ConnectionStrings.NorthwindConnection' "$SCRIPT_DIR/Secrets/secrets.json")
        conn_id=$(jq -r '.ConnectionStrings.IdentityConnection' "$SCRIPT_DIR/Secrets/secrets.json")
        scaffold "$conn_nw" "$CONTEXT_NORTHWIND" "Models/Northwind"
        scaffold "$conn_id" "$CONTEXT_IDENTITY" "Models/Identity"
        echo "=== Scaffold complete ==="
        ;;

    add)
        name="${2:-Initial}"
        echo "=== Creating migration '$name' ==="
        dotnet ef migrations add "$name" \
            --context "$CONTEXT_NORTHWIND" \
            --output-dir "$MIGRATIONS_DIR_NORTHWIND" \
            --project "$PROJECT"
        dotnet ef migrations add "${name}Identity" \
            --context "$CONTEXT_IDENTITY" \
            --output-dir "$MIGRATIONS_DIR_IDENTITY" \
            --project "$PROJECT"
        echo "=== Migration '$name' created ==="
        ;;

    apply)
        echo "=== Applying pending migrations ==="
        dotnet ef database update \
            --context "$CONTEXT_NORTHWIND" \
            --project "$PROJECT"
        dotnet ef database update \
            --context "$CONTEXT_IDENTITY" \
            --project "$PROJECT"
        echo "=== Migrations applied ==="
        ;;

    remove)
        echo "=== Removing last migration ==="
        dotnet ef migrations remove \
            --context "$CONTEXT_NORTHWIND" \
            --project "$PROJECT"
        dotnet ef migrations remove \
            --context "$CONTEXT_IDENTITY" \
            --project "$PROJECT"
        echo "=== Last migration removed ==="
        ;;

    list)
        echo "=== NorthwindContext migrations ==="
        dotnet ef migrations list \
            --context "$CONTEXT_NORTHWIND" \
            --project "$PROJECT"
        echo ""
        echo "=== ApplicationDbContext migrations ==="
        dotnet ef migrations list \
            --context "$CONTEXT_IDENTITY" \
            --project "$PROJECT"
        ;;

    reset)
        echo "=== WARNING: This will delete all migrations and regenerate ==="
        echo "=== Dropping existing migrations ==="
        rm -rf "$MIGRATIONS_DIR_NORTHWIND" "$MIGRATIONS_DIR_IDENTITY"
        echo "=== Creating fresh initial migration ==="
        "$0" add Initial
        echo "=== Reset complete ==="
        ;;

    full)
        echo "=== Full DB First pipeline ==="
        "$0" scaffold
        "$0" add "${2:-Initial}"
        "$0" apply
        ;;

    help|*)
        echo "Usage: ./migration.sh <action> [args]"
        echo ""
        echo "Actions:"
        echo "  scaffold          Generate models from DB (DB First)"
        echo "  add [name]        Create new migration (default: Initial)"
        echo "  apply             Apply pending migrations"
        echo "  remove            Remove last migration"
        echo "  list              List all migrations"
        echo "  reset             Delete all migrations and start fresh"
        echo "  full [name]       Run scaffold → add → apply pipeline"
        echo ""
        echo "Examples:"
        echo "  ./migration.sh scaffold"
        echo "  ./migration.sh add AddProductCategory"
        echo "  ./migration.sh apply"
        echo "  ./migration.sh full"
        ;;
esac
