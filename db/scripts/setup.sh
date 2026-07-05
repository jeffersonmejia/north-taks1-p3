#!/usr/bin/env bash
set -euo pipefail

sudo -v
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "=== Dropping databases (if they exist) ==="
sudo -u postgres psql -c "DROP DATABASE IF EXISTS northwind;"
sudo -u postgres psql -c "DROP DATABASE IF EXISTS northwind_identity;"

echo "=== Creating databases ==="
sudo -u postgres createdb northwind
sudo -u postgres createdb northwind_identity

echo "=== Creating application user ==="
sudo -u postgres psql -d northwind          < "$ROOT/credentials.sql"
sudo -u postgres psql -d northwind_identity < "$ROOT/credentials.sql"

echo "=== Loading schema ==="
sudo -u postgres psql -d northwind < "$ROOT/schema.sql"

echo "=== Loading seed data ==="
sudo -u postgres psql -d northwind < "$ROOT/seed.sql"

echo "=== Creating indexes ==="
sudo -u postgres psql -d northwind < "$ROOT/index.sql"

echo "=== Done ==="
