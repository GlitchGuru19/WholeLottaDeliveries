# Rebuild script for Delivery App
# Run this after stopping the application

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Delivery App - Clean Rebuild" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if app is running
$process = Get-Process -Name "DeliveryApp" -ErrorAction SilentlyContinue
if ($process) {
    Write-Host "ERROR: Application is still running!" -ForegroundColor Red
    Write-Host "Please stop the app first (Press Ctrl+C in the terminal)" -ForegroundColor Yellow
    exit 1
}

Write-Host "[1/5] Cleaning old database files..." -ForegroundColor Yellow
Remove-Item -Path "DeliveryApp.db" -ErrorAction SilentlyContinue
Remove-Item -Path "Data\DeliveryApp.db" -ErrorAction SilentlyContinue
Write-Host "  ✓ Database files removed" -ForegroundColor Green

Write-Host ""
Write-Host "[2/5] Removing old migrations..." -ForegroundColor Yellow
Remove-Item -Path "Migrations" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "  ✓ Old migrations removed" -ForegroundColor Green

Write-Host ""
Write-Host "[3/5] Creating new migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Migration creation failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Migration created" -ForegroundColor Green

Write-Host ""
Write-Host "[4/5] Applying migration to database..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Database update failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Database created in Data/ folder" -ForegroundColor Green

Write-Host ""
Write-Host "[5/5] Building application..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Build successful" -ForegroundColor Green

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "✓ Rebuild Complete!" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Run the app:" -ForegroundColor White
Write-Host "  dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "Admin Login:" -ForegroundColor White
Write-Host "  Email: admin@delivery.com" -ForegroundColor Cyan
Write-Host "  Password: Admin123" -ForegroundColor Cyan
Write-Host ""
