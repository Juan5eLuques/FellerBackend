-- Script para registrar InitialCreate como aplicada y permitir migraciones posteriores

-- 1. Verificar si existe la tabla de historial de migraciones
SELECT EXISTS (
    SELECT 1 FROM information_schema.tables 
    WHERE table_schema = 'public' 
    AND table_name = '__EFMigrationsHistory'
) AS tabla_existe;

-- 2. Crear tabla de historial si no existe
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- 3. Ver migraciones actuales
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- 4. Registrar InitialCreate como aplicada (si no está registrada)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119043803_InitialCreate', '8.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- 5. Verificar resultado
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- 6. Ver estructura actual de tabla Vehiculos
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'Vehiculos'
ORDER BY ordinal_position;
