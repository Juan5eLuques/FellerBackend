# ?? FIX: Actualizar Datos Existentes

## Problema
Error al crear/listar vehículos: "Nullable object must have a value"

## Causa
La migración agregó columnas nuevas pero los registros existentes tienen valores NULL en campos requeridos.

## Solución

### 1. Actualizar Vehículos Existentes en PostgreSQL

Conecta a PostgreSQL y ejecuta estos comandos:

```bash
psql -U postgres -h localhost -d feller_db
```

### 2. Script SQL para Actualizar Datos

```sql
-- Ver estado actual de los vehículos
SELECT "Id", "TipoVehiculo", "Marca", "Modelo", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos";

-- Actualizar AUTOS existentes con valores por defecto
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Puertas" = COALESCE("Puertas", 4),
    "TipoCombustible" = COALESCE("TipoCombustible", 'Nafta')
WHERE "TipoVehiculo" = 'Auto';

-- Actualizar MOTOS existentes con valores por defecto
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Cilindrada" = COALESCE("Cilindrada", 150)
WHERE "TipoVehiculo" = 'Moto';

-- Verificar que se aplicaron los cambios
SELECT "Id", "TipoVehiculo", "Marca", "Modelo", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos";
```

### 3. Verificar Columnas NULL

```sql
-- Ver si hay valores NULL en columnas críticas
SELECT "Id", "TipoVehiculo", "Marca", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos"
WHERE 
    ("TipoVehiculo" = 'Auto' AND ("Puertas" IS NULL OR "TipoCombustible" IS NULL OR "Estado" IS NULL))
    OR
    ("TipoVehiculo" = 'Moto' AND ("Cilindrada" IS NULL OR "Estado" IS NULL));
```

---

## Alternativa: Migración de Corrección

Si prefieres hacer una migración formal:

### Crear nueva migración:

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef migrations add CorregirValoresPorDefecto
```

El contenido debería ser:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Actualizar autos existentes
    migrationBuilder.Sql(@"
        UPDATE ""Vehiculos""
        SET 
            ""Estado"" = COALESCE(""Estado"", 'Usado'),
      ""Puertas"" = COALESCE(""Puertas"", 4),
 ""TipoCombustible"" = COALESCE(""TipoCombustible"", 'Nafta')
     WHERE ""TipoVehiculo"" = 'Auto';
    ");

    // Actualizar motos existentes
migrationBuilder.Sql(@"
    UPDATE ""Vehiculos""
        SET 
        ""Estado"" = COALESCE(""Estado"", 'Usado'),
""Cilindrada"" = COALESCE(""Cilindrada"", 150)
        WHERE ""TipoVehiculo"" = 'Moto';
    ");
}
```

### Aplicar migración:

```bash
dotnet ef database update
```

---

## Opción Rápida: Eliminar y Recrear Datos

Si NO tienes datos importantes:

```sql
-- Eliminar todos los vehículos
DELETE FROM "ImagenesVehiculos";
DELETE FROM "Vehiculos";

-- Resetear secuencias
ALTER SEQUENCE "Vehiculos_Id_seq" RESTART WITH 1;
ALTER SEQUENCE "ImagenesVehiculos_Id_seq" RESTART WITH 1;
```

Luego usa el endpoint de seed para crear datos de prueba:

```http
POST /api/seed/seed-test-data
```

---

## Verificación Post-Fix

Después de aplicar la solución, verifica que funciona:

### 1. Listar Autos
```http
GET /api/autos
```

### 2. Listar Motos
```http
GET /api/motos
```

### 3. Crear Auto Nuevo
```http
POST /api/autos
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "marca": "Toyota",
  "modelo": "Corolla",
  "año": 2024,
  "precio": 35000,
  "estado": "0km",
  "puertas": 4,
  "tipoCombustible": "Nafta",
  "transmision": "Automática",
  "kilometraje": null,
  "descripcion": "Toyota Corolla 2024",
  "disponible": true
}
```

### 4. Crear Moto Nueva
```http
POST /api/motos
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "marca": "Honda",
  "modelo": "CB 500X",
  "año": 2024,
  "precio": 10000,
  "estado": "0km",
  "cilindrada": 500,
  "tipoMoto": "Touring",
  "kilometraje": null,
  "descripcion": "Honda CB 500X 2024",
  "disponible": true
}
```

---

## Estado de Columnas Después del Fix

| Tabla | Columna | Tipo | Nullable | Valor Por Defecto |
|-------|---------|------|----------|-------------------|
| Vehiculos | Estado | text | NO | 'Usado' |
| Vehiculos | Puertas | int | SI | 4 (para autos) |
| Vehiculos | TipoCombustible | text | SI | 'Nafta' (para autos) |
| Vehiculos | Cilindrada | int | SI | 150 (para motos) |
| Vehiculos | Transmision | text | SI | NULL |
| Vehiculos | TipoMoto | text | SI | NULL |
| Vehiculos | Kilometraje | int | SI | NULL |

---

## Resumen de Comandos

```bash
# 1. Conectar a PostgreSQL
psql -U postgres -h localhost -d feller_db

# 2. Ejecutar en PostgreSQL
UPDATE "Vehiculos" SET "Estado" = COALESCE("Estado", 'Usado'), "Puertas" = COALESCE("Puertas", 4), "TipoCombustible" = COALESCE("TipoCombustible", 'Nafta') WHERE "TipoVehiculo" = 'Auto';
UPDATE "Vehiculos" SET "Estado" = COALESCE("Estado", 'Usado'), "Cilindrada" = COALESCE("Cilindrada", 150) WHERE "TipoVehiculo" = 'Moto';

# 3. Salir de PostgreSQL
\q

# 4. Reiniciar el proyecto
dotnet run
```

---

**Estado**: ?? Requiere acción  
**Prioridad**: Alta  
**Tiempo estimado**: 2 minutos
