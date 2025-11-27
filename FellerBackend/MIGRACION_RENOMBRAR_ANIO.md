# Migración Manual: Renombrar columna Año a Anio

## Problema

Railway rechaza el build porque la columna se llama "Año" (con eñe) y causa problemas de encoding UTF-8 durante el build en Docker.

## Solución

Renombrar la columna de `Año` a `Anio` en la base de datos.

## SQL para PostgreSQL

```sql
-- Renombrar columna en tabla Vehiculos
ALTER TABLE "Vehiculos" 
RENAME COLUMN "Año" TO "Anio";
```

## Aplicar en Railway

### Opción 1: Railway CLI

```bash
railway connect postgres

# Una vez conectado
ALTER TABLE "Vehiculos" RENAME COLUMN "Año" TO "Anio";
```

### Opción 2: Desde DBeaver o pgAdmin

1. Conectarse a la base de datos de Railway:
   - Host: centerbeam.proxy.rlwy.net
   - Port: 22293
   - Database: railway
   - Username: postgres
   - Password: YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt
   - SSL Mode: Require

2. Ejecutar el SQL:
```sql
ALTER TABLE "Vehiculos" RENAME COLUMN "Año" TO "Anio";
```

### Opción 3: psql

```bash
PGPASSWORD='YubMlpAtULwvnDQDLCIFMUGbLBnzWVpt' psql -h centerbeam.proxy.rlwy.net -p 22293 -U postgres -d railway -c 'ALTER TABLE "Vehiculos" RENAME COLUMN "Año" TO "Anio";'
```

## Verificar

```sql
-- Ver estructura de tabla
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'Vehiculos' 
ORDER BY ordinal_position;
```

Deberías ver `Anio` en lugar de `Año`.

## Nota

Después de aplicar esta migración, el próximo deploy en Railway debería funcionar sin problemas de encoding.
