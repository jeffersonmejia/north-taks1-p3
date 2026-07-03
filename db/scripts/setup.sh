#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "=== Creating databases ==="
sudo -u postgres createdb northwind
sudo -u postgres createdb northwind_identity

echo "=== Creating application user ==="
sudo -u postgres psql -d northwind          -f "$ROOT/credentials.sql"
sudo -u postgres psql -d northwind_identity -f "$ROOT/credentials.sql"

echo "=== Loading schema ==="
sudo -u postgres psql -d northwind -f "$ROOT/schema.sql"

echo "=== Loading seed data ==="
sudo -u postgres psql -d northwind -f "$ROOT/seed.sql"

echo "=== Creating indexes ==="
sudo -u postgres psql -d northwind -f "$ROOT/index.sql"

echo "=== Done ==="
