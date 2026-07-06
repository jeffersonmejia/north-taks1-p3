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

# ─── helpers ──────────────────────────────────────────

run_scaffold() {
    local conn="$1" context="$2" output="$3"
    echo "=== Scaffolding $context from database ==="
    dotnet ef dbcontext scaffold "$conn" Npgsql.EntityFrameworkCore.PostgreSQL \
        --context "$context" \
        --output-dir "$output" \
        --project "$PROJECT" \
        --force 2>/dev/null \
    || dotnet ef dbcontext scaffold "$conn" Npgsql.EntityFrameworkCore.PostgreSQL \
        --context "$context" \
        --output-dir "$output" \
        --project "$PROJECT" \
        --force
}

# ─── actions ──────────────────────────────────────────

do_scaffold() {
    echo "=== DB First: Scaffolding models from existing database ==="
    conn_nw=$(jq -r '.ConnectionStrings.NorthwindConnection' "$SCRIPT_DIR/Secrets/secrets.json")
    conn_id=$(jq -r '.ConnectionStrings.IdentityConnection' "$SCRIPT_DIR/Secrets/secrets.json")
    run_scaffold "$conn_nw" "$CONTEXT_NORTHWIND" "Models/Northwind"
    run_scaffold "$conn_id" "$CONTEXT_IDENTITY" "Models/Identity"
    echo "=== Scaffold complete ==="
}

do_add() {
    local name="${1:-Initial}"
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
}

do_apply() {
    echo "=== Applying pending migrations ==="
    dotnet ef database update \
        --context "$CONTEXT_NORTHWIND" \
        --project "$PROJECT"
    dotnet ef database update \
        --context "$CONTEXT_IDENTITY" \
        --project "$PROJECT"
    echo "=== Migrations applied ==="
}

do_remove() {
    echo "=== Removing last migration ==="
    dotnet ef migrations remove \
        --context "$CONTEXT_NORTHWIND" \
        --project "$PROJECT"
    dotnet ef migrations remove \
        --context "$CONTEXT_IDENTITY" \
        --project "$PROJECT"
    echo "=== Last migration removed ==="
}

do_list() {
    echo "=== NorthwindContext migrations ==="
    dotnet ef migrations list \
        --context "$CONTEXT_NORTHWIND" \
        --project "$PROJECT"
    echo ""
    echo "=== ApplicationDbContext migrations ==="
    dotnet ef migrations list \
        --context "$CONTEXT_IDENTITY" \
        --project "$PROJECT"
}

do_reset() {
    echo "=== WARNING: This will delete all migrations and regenerate ==="
    echo "=== Dropping existing migrations ==="
    rm -rf "$MIGRATIONS_DIR_NORTHWIND" "$MIGRATIONS_DIR_IDENTITY"
    echo "=== Creating fresh initial migration ==="
    do_add "$@"
    echo "=== Reset complete ==="
}

do_full() {
    echo "=== Full DB First pipeline ==="
    do_scaffold
    do_add "${1:-Initial}"
    do_apply
    echo "=== Full pipeline complete ==="
}

do_help() {
    echo "Usage: ./migration.sh <action> [args]"
    echo ""
    echo "Actions:"
    echo "  scaffold          Generate models from DB (DB First)"
    echo "  add [name]        Create new migration (default: Initial)"
    echo "  apply             Apply pending migrations"
    echo "  remove            Remove last migration"
    echo "  list              List all migrations"
    echo "  reset [name]      Delete all migrations and start fresh"
    echo "  full [name]       Run scaffold → add → apply pipeline"
    echo ""
    echo "Examples:"
    echo "  ./migration.sh scaffold"
    echo "  ./migration.sh add AddProductCategory"
    echo "  ./migration.sh apply"
    echo "  ./migration.sh full"
}

# ─── dispatch ─────────────────────────────────────────

action="${1:-help}"
shift 2>/dev/null || true

case "$action" in
    scaffold) do_scaffold ;;
    add)      do_add "$@" ;;
    apply)    do_apply ;;
    remove)   do_remove ;;
    list)     do_list ;;
    reset)    do_reset "$@" ;;
    full)     do_full "$@" ;;
    help|*)   do_help ;;
esac
