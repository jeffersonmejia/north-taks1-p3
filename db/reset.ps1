Write-Host "=== Dropping and recreating databases ==="
& psql -d postgres -f "$PSScriptRoot/credentials.sql"

Write-Host "=== Loading schema ==="
& psql -d northwind -f "$PSScriptRoot/schema.sql"

Write-Host "=== Loading seed data ==="
& psql -d northwind -f "$PSScriptRoot/seed.sql"

Write-Host "=== Loading indexes ==="
& psql -d northwind -f "$PSScriptRoot/index.sql"

Write-Host "=== Granting application permissions ==="
& psql -d postgres -f "$PSScriptRoot/grants.sql"

Write-Host "=== Reset complete ==="
