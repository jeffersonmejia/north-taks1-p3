#!/usr/bin/env bash
clear
set -uo pipefail

sudo -v
trap 'kill 0' EXIT
(sudo -v &>/dev/null &)

HAS_DATA=$(sudo -u postgres psql -d northwind -tAc "SELECT COUNT(*) FROM products;" 2>/dev/null || echo "0")

if [ "${HAS_DATA:-0}" = "0" ]; then
    echo "Database missing or empty — running setup..."
    bash db/scripts/setup.sh || echo "Setup failed (non-fatal), starting app anyway..."
fi

echo "=== Starting NorthwindStore ==="
dotnet watch run
