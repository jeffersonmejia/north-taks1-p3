#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "=== Dropping and recreating databases ==="
sudo -u postgres psql -d postgres < "$SCRIPT_DIR/credentials.sql"

echo "=== Loading schema ==="
sudo -u postgres psql -d northwind < "$SCRIPT_DIR/schema.sql"

echo "=== Loading seed data ==="
sudo -u postgres psql -d northwind < "$SCRIPT_DIR/seed.sql"

echo "=== Loading indexes ==="
sudo -u postgres psql -d northwind < "$SCRIPT_DIR/index.sql"

echo "=== Granting application permissions ==="
sudo -u postgres psql -d postgres < "$SCRIPT_DIR/grants.sql"

echo "=== Reset complete ==="
