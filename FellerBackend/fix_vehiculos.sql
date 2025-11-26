-- Script de corrección para vehículos existentes
-- Ejecutar este archivo con: psql -U postgres -h localhost -d feller_db -f fix_vehiculos.sql

-- Ver estado actual
SELECT 'Estado actual de vehículos:' AS mensaje;
SELECT "Id", "TipoVehiculo", "Marca", "Modelo", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos";

-- Actualizar AUTOS con valores por defecto
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Puertas" = COALESCE("Puertas", 4),
    "TipoCombustible" = COALESCE("TipoCombustible", 'Nafta')
WHERE "TipoVehiculo" = 'Auto';

SELECT 'Autos actualizados' AS mensaje;

-- Actualizar MOTOS con valores por defecto
UPDATE "Vehiculos"
SET 
    "Estado" = COALESCE("Estado", 'Usado'),
    "Cilindrada" = COALESCE("Cilindrada", 150)
WHERE "TipoVehiculo" = 'Moto';

SELECT 'Motos actualizadas' AS mensaje;

-- Verificar resultado
SELECT 'Estado después de la actualización:' AS mensaje;
SELECT "Id", "TipoVehiculo", "Marca", "Modelo", "Estado", "Puertas", "TipoCombustible", "Cilindrada"
FROM "Vehiculos";

-- Verificar si quedan valores NULL críticos
SELECT 'Verificando valores NULL críticos:' AS mensaje;
SELECT COUNT(*) as vehiculos_con_problemas
FROM "Vehiculos"
WHERE 
    ("TipoVehiculo" = 'Auto' AND ("Puertas" IS NULL OR "TipoCombustible" IS NULL OR "Estado" IS NULL OR "Estado" = ''))
    OR
    ("TipoVehiculo" = 'Moto' AND ("Cilindrada" IS NULL OR "Estado" IS NULL OR "Estado" = ''));
