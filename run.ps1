Write-Host "=== Checking database ==="
try {
    $count = & psql -d northwind -tAc "SELECT COUNT(*) FROM products;" 2>$null
    if ([string]::IsNullOrWhiteSpace($count)) { $count = "0" }
} catch {
    $count = "0"
}

if ($count -eq "0") {
    Write-Host "Database missing or empty — running setup..."
    & .\db\scripts\setup.ps1
} else {
    Write-Host "Database is ready."
}

Write-Host "=== Starting NorthwindStore ==="
dotnet run
