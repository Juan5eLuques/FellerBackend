# Script para registrar migración inicial y aplicar migraciones restantes

Write-Host "?? MIGRACIÓN A PRODUCCIÓN - Railway" -ForegroundColor Cyan
Write-Host "====================================`n" -ForegroundColor Cyan

$connectionString = "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"

Write-Host "?? Paso 1: Registrar InitialCreate como aplicada" -ForegroundColor Yellow

# SQL para registrar la migración inicial
$registerSql = @"
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119043803_InitialCreate', '8.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
"@

# Guardar SQL en archivo temporal
$registerSql | Out-File -FilePath "temp_register.sql" -Encoding UTF8

# Ejecutar usando psql si está disponible
try {
    Write-Host "Intentando ejecutar con psql..." -ForegroundColor Gray
    $env:PGPASSWORD = "YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt"
    psql -h centerbeam.proxy.rlwy.net -p 22293 -U postgres -d railway -f temp_register.sql
    
    Write-Host "? Migración inicial registrada" -ForegroundColor Green
} catch {
    Write-Host "??  No se pudo ejecutar con psql. Ejecuta manualmente:" -ForegroundColor Yellow
    Write-Host $registerSql -ForegroundColor White
    Write-Host "`nPresiona Enter después de ejecutar el SQL manualmente..."
    Read-Host
}

# Limpiar archivo temporal
if (Test-Path "temp_register.sql") {
    Remove-Item "temp_register.sql"
}

Write-Host "`n?? Paso 2: Aplicar migraciones restantes con Entity Framework" -ForegroundColor Yellow

# Establecer variable de entorno con connection string
$env:ConnectionStrings__DefaultConnection = $connectionString

# Ir al directorio del proyecto
Set-Location "D:\repos\feller\backend\FellerBackend\FellerBackend"

Write-Host "Aplicando migraciones..." -ForegroundColor Gray

# Aplicar migraciones
try {
  dotnet ef database update
    Write-Host "`n? Migraciones aplicadas exitosamente" -ForegroundColor Green
} catch {
    Write-Host "`n? Error al aplicar migraciones" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "`n?? Paso 3: Verificar migraciones aplicadas" -ForegroundColor Yellow

# Ver lista de migraciones
dotnet ef migrations list

Write-Host "`n? PROCESO COMPLETADO" -ForegroundColor Green
Write-Host "`nVerifica que las siguientes migraciones estén aplicadas:" -ForegroundColor Cyan
Write-Host "  - 20251119043803_InitialCreate" -ForegroundColor White
Write-Host "  - 20251119060840_AgregarPropiedadesConcesionaria" -ForegroundColor White
Write-Host "  - 20251120051939_AgregarVehiculosDestacados" -ForegroundColor White
