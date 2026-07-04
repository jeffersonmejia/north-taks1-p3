$ROOT = Split-Path (Split-Path $PSScriptRoot -Parent)

Write-Host "=== Dropping and recreating databases ==="
& psql -d postgres -f "$ROOT\credentials.sql"

Write-Host "=== Loading schema ==="
& psql -d northwind -f "$ROOT\schema.sql"

Write-Host "=== Loading seed data ==="
& psql -d northwind -f "$ROOT\seed.sql"

Write-Host "=== Creating indexes ==="
& psql -d northwind -f "$ROOT\index.sql"

Write-Host "=== Granting application permissions ==="
& psql -d postgres -f "$ROOT\grants.sql"

Write-Host "=== Done ==="
