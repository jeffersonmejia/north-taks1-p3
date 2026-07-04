#!/usr/bin/env bash
set -euo pipefail

sudo -v
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "=== Dropping and recreating databases ==="
sudo -u postgres psql -d postgres < "$ROOT/credentials.sql"

echo "=== Loading schema ==="
sudo -u postgres psql -d northwind < "$ROOT/schema.sql"

echo "=== Loading seed data ==="
sudo -u postgres psql -d northwind < "$ROOT/seed.sql"

echo "=== Creating indexes ==="
sudo -u postgres psql -d northwind < "$ROOT/index.sql"

echo "=== Granting application permissions ==="
sudo -u postgres psql -d postgres < "$ROOT/grants.sql"

echo "=== Done ==="
