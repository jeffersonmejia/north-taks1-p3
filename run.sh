#!/usr/bin/env bash
set -euo pipefail

sudo -v
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

echo "=== Checking database ==="

HAS_DATA=$(sudo -u postgres psql -d northwind -tAc "SELECT COUNT(*) FROM products;" 2>/dev/null || echo "0")

if [ "${HAS_DATA:-0}" = "0" ]; then
    echo "Database missing or empty — running setup..."
    bash db/scripts/setup.sh
else
    echo "Database is ready."
fi

echo "=== Starting NorthwindStore ==="
dotnet run
