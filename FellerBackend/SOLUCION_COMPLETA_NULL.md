# ?? SOLUCIÓN COMPLETA - Error "Nullable object must have a value"

## ?? PROBLEMA

Al intentar crear o listar vehículos, obtienes el error:
```
System.InvalidOperationException: Nullable object must have a value
```

## ?? CAUSA

Las nuevas columnas (`Puertas`, `TipoCombustible`, `Cilindrada`, `Estado`) pueden tener valores NULL en registros existentes, pero el código espera valores no nulos.

---

## ? SOLUCIÓN EN 3 PASOS

### PASO 1: Detener el Proyecto

```bash
# Presiona Ctrl + C en la terminal donde está corriendo
```

### PASO 2: Actualizar Datos Existentes

#### Opción A: Usando psql (Recomendado)

```bash
# Ejecutar este comando desde PowerShell
cd D:\repos\feller\backend\FellerBackend\FellerBackend
$env:PGPASSWORD='Juanseluques.10'
psql -U postgres -h localhost -d feller_db -f fix_vehiculos.sql
```

#### Opción B: Manual en pgAdmin o DBeaver

1. Abrir pgAdmin o DBeaver
2. Conectar a la base de datos `feller_db`
3. Ejecutar este SQL:

```sql
-- Actualizar AUTOS
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Puertas" = COALESCE("Puertas", 4),
    "TipoCombustible" = COALESCE("TipoCombustible", 'Nafta')
WHERE "TipoVehiculo" = 'Auto';

-- Actualizar MOTOS
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Cilindrada" = COALESCE("Cilindrada", 150)
WHERE "TipoVehiculo" = 'Moto';

-- Verificar
SELECT "Id", "TipoVehiculo", "Marca", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos";
```

### PASO 3: Crear Nueva Migración

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend

# Crear migración
dotnet ef migrations add ConfigurarValoresPorDefecto

# Aplicar migración
dotnet ef database update
```

### PASO 4: Iniciar el Proyecto

```bash
dotnet run
```

---

## ?? VERIFICAR QUE FUNCIONA

### 1. Listar Autos
```http
GET http://localhost:5000/api/autos
```

**Esperado**: Lista de autos sin errores

### 2. Listar Motos
```http
GET http://localhost:5000/api/motos
```

**Esperado**: Lista de motos sin errores

### 3. Crear Auto Nuevo
```http
POST http://localhost:5000/api/autos
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

**Esperado**: Auto creado exitosamente

### 4. Crear Moto Nueva
```http
POST http://localhost:5000/api/motos
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

**Esperado**: Moto creada exitosamente

---

## ?? ALTERNATIVA: Limpiar y Empezar de Nuevo

Si NO tienes datos importantes en la base de datos:

### 1. Conectar a PostgreSQL
```bash
$env:PGPASSWORD='Juanseluques.10'
psql -U postgres -h localhost -d feller_db
```

### 2. Limpiar datos
```sql
-- Eliminar todos los datos
TRUNCATE TABLE "ImagenesVehiculos" CASCADE;
TRUNCATE TABLE "Vehiculos" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Notificaciones" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Turnos" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Usuarios" RESTART IDENTITY CASCADE;

-- Salir
\q
```

### 3. Recrear datos de prueba
```bash
# Iniciar proyecto
dotnet run

# En otra terminal, usar Swagger o Postman
POST http://localhost:5000/api/seed/seed-test-data
```

---

## ?? CAMBIOS REALIZADOS EN EL CÓDIGO

### FellerDbContext.cs - Configuraciones Agregadas

```csharp
// Configuración específica de Auto
modelBuilder.Entity<Auto>(entity =>
{
    entity.Property(e => e.Puertas).IsRequired().HasDefaultValue(4);
    entity.Property(e => e.TipoCombustible).IsRequired().HasMaxLength(20).HasDefaultValue("Nafta");
    entity.Property(e => e.Transmision).HasMaxLength(20);
});

// Configuración específica de Moto
modelBuilder.Entity<Moto>(entity =>
{
    entity.Property(e => e.Cilindrada).IsRequired().HasDefaultValue(150);
    entity.Property(e => e.TipoMoto).HasMaxLength(20);
});
```

Estos valores por defecto se aplicarán a nuevos registros automáticamente.

---

## ? SOLUCIÓN RÁPIDA (Sin migración)

Si quieres una solución inmediata sin migración:

### 1. Actualizar solo los datos existentes

```bash
# PowerShell
$env:PGPASSWORD='Juanseluques.10'
psql -U postgres -h localhost -d feller_db -c "UPDATE `"Vehiculos`" SET `"Estado`" = 'Usado', `"Puertas`" = 4, `"TipoCombustible`" = 'Nafta' WHERE `"TipoVehiculo`" = 'Auto' AND (`"Estado`" IS NULL OR `"Estado`" = '' OR `"Puertas`" IS NULL OR `"TipoCombustible`" IS NULL);"

psql -U postgres -h localhost -d feller_db -c "UPDATE `"Vehiculos`" SET `"Estado`" = 'Usado', `"Cilindrada`" = 150 WHERE `"TipoVehiculo`" = 'Moto' AND (`"Estado`" IS NULL OR `"Estado`" = '' OR `"Cilindrada`" IS NULL);"
```

### 2. Reiniciar proyecto

```bash
# Detener (Ctrl + C)
# Iniciar
dotnet run
```

---

## ?? ORDEN RECOMENDADO

1. ? Detener proyecto (`Ctrl + C`)
2. ? Ejecutar SQL de actualización (Opción A o B del Paso 2)
3. ? Crear y aplicar migración (Paso 3)
4. ? Iniciar proyecto (`dotnet run`)
5. ? Probar endpoints (Paso "Verificar")

---

## ?? CHECKLIST POST-SOLUCIÓN

- [ ] Proyecto inicia sin errores
- [ ] `GET /api/autos` funciona
- [ ] `GET /api/motos` funciona
- [ ] Puedo crear un auto nuevo
- [ ] Puedo crear una moto nueva
- [ ] Los vehículos existentes se listan correctamente
- [ ] Las imágenes se cargan correctamente

---

## ?? SI AÚN TIENES PROBLEMAS

### Verificar valores NULL restantes

```sql
SELECT "Id", "TipoVehiculo", "Marca", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos"
WHERE 
  ("Estado" IS NULL OR "Estado" = '')
    OR ("TipoVehiculo" = 'Auto' AND ("Puertas" IS NULL OR "TipoCombustible" IS NULL))
    OR ("TipoVehiculo" = 'Moto' AND "Cilindrada" IS NULL);
```

Si encuentra registros, aplicar:

```sql
DELETE FROM "Vehiculos" WHERE "Estado" IS NULL OR "Estado" = '';
```

O actualizar manualmente cada registro.

---

## ?? RESUMEN

**Problema**: Columnas nuevas con valores NULL  
**Solución**: Actualizar datos existentes + migración con valores por defecto  
**Tiempo**: 5 minutos  
**Dificultad**: Baja  

**Archivos creados**:
- `FIX_VALORES_NULL.md` - Guía detallada
- `fix_vehiculos.sql` - Script SQL
- `SOLUCION_COMPLETA_NULL.md` - Este archivo

**Archivos modificados**:
- `FellerDbContext.cs` - Agregadas configuraciones de valores por defecto

---

**Estado**: ?? Pendiente de aplicar  
**Próximo paso**: Ejecutar Paso 2 (Actualizar datos)
