$ROOT = Join-Path (Split-Path $PSScriptRoot -Parent)

Write-Host "=== Creating databases ==="
& createdb northwind
& createdb northwind_identity

Write-Host "=== Creating application user ==="
& psql -d northwind          -f "$ROOT\credentials.sql"
& psql -d northwind_identity -f "$ROOT\credentials.sql"

Write-Host "=== Loading schema ==="
& psql -d northwind -f "$ROOT\schema.sql"

Write-Host "=== Loading seed data ==="
& psql -d northwind -f "$ROOT\seed.sql"

Write-Host "=== Creating indexes ==="
& psql -d northwind -f "$ROOT\index.sql"

Write-Host "=== Done ==="
