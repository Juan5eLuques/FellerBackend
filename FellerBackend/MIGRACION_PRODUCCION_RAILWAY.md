# ?? MIGRACIÓN A PRODUCCIÓN - Railway

## ?? Información de Conexión

**Base de datos**: PostgreSQL en Railway  
**Host**: centerbeam.proxy.rlwy.net  
**Puerto**: 22293  
**Database**: railway  
**Usuario**: postgres  

---

## ?? PASO 1: Aplicar Migraciones desde Entity Framework

### Opción A: Usar Connection String Directamente (Recomendado)

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend

dotnet ef database update --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

### Opción B: Usar Variable de Entorno

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend

$env:ConnectionStrings__DefaultConnection="Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"

dotnet ef database update
```

---

## ?? PASO 2: Verificar Estado Actual

### Ver qué migraciones existen

```bash
dotnet ef migrations list
```

**Resultado esperado**:
```
20251119043803_InitialCreate
20251119060840_AgregarPropiedadesConcesionaria
20251120051939_AgregarVehiculosDestacados
```

### Ver qué migraciones están aplicadas en producción

```bash
dotnet ef migrations list --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

---

## ?? PASO 3: Aplicar Migraciones Específicas

### Si necesitas aplicar solo una migración específica

```bash
# Aplicar hasta una migración específica
dotnet ef database update AgregarPropiedadesConcesionaria --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

### Aplicar todas las migraciones pendientes

```bash
dotnet ef database update --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

---

## ?? SI HAY ERROR "Tabla ya existe"

Si obtienes el error `relation "Usuarios" already exists`, significa que algunas tablas ya existen.

### Solución 1: Marcar Migración como Aplicada (Sin ejecutar)

```bash
# Marcar InitialCreate como aplicada (si ya tienes las tablas)
dotnet ef database update 20251119043803_InitialCreate --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

### Solución 2: SQL Manual para Registrar Migración

Conéctate a la BD y ejecuta:

```sql
-- Verificar tabla de migraciones
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- Si no existe, crearla
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Registrar InitialCreate como aplicada (si ya tienes las tablas)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119043803_InitialCreate', '8.0.0')
ON CONFLICT DO NOTHING;
```

Luego ejecuta las migraciones restantes:

```bash
dotnet ef database update --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

---

## ?? PASO 4: Verificar Columnas Agregadas

Después de aplicar las migraciones, verifica que se agregaron las columnas:

```sql
-- Ver estructura de tabla Vehiculos
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'Vehiculos'
ORDER BY ordinal_position;
```

**Deberías ver**:
- `Estado` (text)
- `EsDestacado` (boolean)
- `OrdenDestacado` (integer, nullable)
- `Puertas` (integer)
- `TipoCombustible` (text)
- `Transmision` (text, nullable)
- `Kilometraje` (integer, nullable)
- `Cilindrada` (integer)
- `TipoMoto` (text, nullable)

---

## ? PASO 5: Probar la Aplicación

### 1. Actualizar appsettings.Production.json

Crea el archivo `appsettings.Production.json` (NO lo commitees):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### 2. Ejecutar la API en modo Producción

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend

$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

### 3. Probar Endpoints

```bash
# GET Autos (debería incluir nuevas propiedades)
curl http://localhost:5000/api/autos

# GET Destacados
curl http://localhost:5000/api/destacados
```

---

## ?? ROLLBACK (Si algo sale mal)

### Revertir a una migración anterior

```bash
# Volver a InitialCreate
dotnet ef database update InitialCreate --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"
```

### Eliminar última migración (solo si NO está aplicada)

```bash
dotnet ef migrations remove
```

---

## ?? COMANDOS ÚTILES

### Ver historial de migraciones aplicadas

```sql
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
```

### Ver todas las tablas

```sql
SELECT tablename FROM pg_tables WHERE schemaname = 'public';
```

### Ver columnas de una tabla

```sql
\d "Vehiculos"
```

---

## ?? DEPLOY AUTOMÁTICO (Para el futuro)

### GitHub Actions

Crea `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Railway

on:
  push:
    branches: [main, master]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
     uses: actions/setup-dotnet@v3
      with:
 dotnet-version: 8.0.x
      
    - name: Install EF Tools
  run: dotnet tool install --global dotnet-ef
      
      - name: Run Migrations
        run: |
          cd FellerBackend
          dotnet ef database update --connection "${{ secrets.RAILWAY_CONNECTION_STRING }}"
      
 - name: Deploy to Railway
        run: railway up
```

---

## ?? IMPORTANTE

### Antes de aplicar migraciones en producción

1. ? **Backup de la base de datos**
   ```bash
   # Railway hace backups automáticos, pero verifica
   ```

2. ? **Probar en local primero**
   ```bash
   dotnet ef database update
   ```

3. ? **Revisar los scripts SQL generados**
 ```bash
   dotnet ef migrations script --idempotent
   ```

4. ? **Tener plan de rollback**
   - Saber a qué migración volver
   - Tener backup reciente

---

## ?? RESUMEN EJECUTIVO

```bash
# 1. Aplicar todas las migraciones
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef database update --connection "Host=centerbeam.proxy.rlwy.net;Port=22293;Database=railway;Username=postgres;Password=YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt;SSL Mode=Require;Trust Server Certificate=true"

# 2. Verificar resultado
# Debería mostrar: "Done." sin errores

# 3. Probar API
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run

# 4. En navegador
http://localhost:5000/swagger
```

---

**Próximo paso**: Ejecutar el comando de migración
