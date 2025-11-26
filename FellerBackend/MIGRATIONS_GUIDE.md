# ??? Guía de Migraciones Entity Framework Core

## ?? Requisitos Previos

1. Tener instalado el CLI de Entity Framework:
```bash
dotnet tool install --global dotnet-ef
# O actualizar si ya lo tienes:
dotnet tool update --global dotnet-ef
```

2. PostgreSQL instalado y corriendo (o usar Docker):
```bash
docker run --name postgres-feller -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

3. Configurar `appsettings.json` con tu cadena de conexión a PostgreSQL

## ?? Crear la Primera Migración

Desde la carpeta `FellerBackend` (donde está el .csproj):

```bash
dotnet ef migrations add InitialCreate
```

Esto creará una carpeta `Migrations/` con:
- `{timestamp}_InitialCreate.cs` - Código de la migración
- `{timestamp}_InitialCreate.Designer.cs` - Metadata
- `FellerDbContextModelSnapshot.cs` - Estado actual del modelo

## ? Aplicar la Migración

```bash
dotnet ef database update
```

Esto creará todas las tablas en PostgreSQL:
- Usuarios
- Vehiculos (con discriminador para Auto/Moto)
- ImagenesVehiculos
- Turnos
- Notificaciones

## ?? Verificar las Tablas Creadas

Conéctate a PostgreSQL y verifica:

```sql
-- Listar todas las tablas
\dt

-- Ver estructura de una tabla
\d "Usuarios"
\d "Vehiculos"
\d "Turnos"

-- Verificar datos
SELECT * FROM "Usuarios";
```

## ?? Estructura de Tablas Esperada

### Usuarios
```sql
CREATE TABLE "Usuarios" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "Telefono" VARCHAR(20),
    "Rol" VARCHAR(20),
    "FechaRegistro" TIMESTAMP NOT NULL
);
```

### Vehiculos (Table Per Hierarchy)
```sql
CREATE TABLE "Vehiculos" (
    "Id" SERIAL PRIMARY KEY,
    "TipoVehiculo" VARCHAR(50) NOT NULL, -- 'Auto' o 'Moto'
    "Marca" VARCHAR(50) NOT NULL,
    "Modelo" VARCHAR(50) NOT NULL,
    "Año" INTEGER NOT NULL,
    "Precio" DECIMAL(18,2) NOT NULL,
    "Descripcion" TEXT,
    "Disponible" BOOLEAN NOT NULL DEFAULT TRUE,
    "FechaPublicacion" TIMESTAMP NOT NULL
);
```

### ImagenesVehiculos
```sql
CREATE TABLE "ImagenesVehiculos" (
    "Id" SERIAL PRIMARY KEY,
    "VehiculoId" INTEGER NOT NULL,
    "Url" TEXT NOT NULL,
    "Key" TEXT NOT NULL,
    "FechaSubida" TIMESTAMP NOT NULL,
    FOREIGN KEY ("VehiculoId") REFERENCES "Vehiculos"("Id") ON DELETE CASCADE
);
```

### Turnos
```sql
CREATE TABLE "Turnos" (
    "Id" SERIAL PRIMARY KEY,
    "UsuarioId" INTEGER NOT NULL,
    "Fecha" DATE NOT NULL,
    "Hora" TIME NOT NULL,
    "TipoLavado" VARCHAR(50) NOT NULL,
    "Estado" VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    "FechaFinalizacion" TIMESTAMP,
    "FechaCreacion" TIMESTAMP NOT NULL,
    FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios"("Id") ON DELETE CASCADE
);
```

### Notificaciones
```sql
CREATE TABLE "Notificaciones" (
    "Id" SERIAL PRIMARY KEY,
    "UsuarioId" INTEGER NOT NULL,
    "Mensaje" TEXT NOT NULL,
    "Tipo" VARCHAR(20) NOT NULL,
    "FechaEnvio" TIMESTAMP NOT NULL,
    "Enviada" BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios"("Id") ON DELETE CASCADE
);
```

## ?? Comandos Útiles

### Ver migraciones aplicadas
```bash
dotnet ef migrations list
```

### Generar script SQL (sin aplicar)
```bash
dotnet ef migrations script
```

### Revertir a una migración específica
```bash
dotnet ef database update NombreMigracionAnterior
```

### Eliminar la última migración (si no fue aplicada)
```bash
dotnet ef migrations remove
```

### Eliminar toda la base de datos
```bash
dotnet ef database drop
```

### Recrear la base de datos desde cero
```bash
dotnet ef database drop --force
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## ?? Solución de Problemas Comunes

### Error: "No se puede conectar a PostgreSQL"
```bash
# Verificar que PostgreSQL está corriendo
docker ps
# O en Windows:
pg_ctl status

# Verificar la cadena de conexión en appsettings.json
```

### Error: "Build failed"
```bash
# Asegúrate de estar en la carpeta correcta
cd FellerBackend

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build
```

### Error: "The Entity Framework tools version is older"
```bash
dotnet tool update --global dotnet-ef
```

### Error: "Could not load assembly 'FellerBackend'"
```bash
# Asegúrate de que el proyecto compila
dotnet build

# Luego intenta la migración nuevamente
dotnet ef migrations add InitialCreate
```

## ?? Agregar Datos de Prueba (Opcional)

Después de crear las tablas, puedes agregar datos de prueba:

```sql
-- Crear usuario admin de prueba
INSERT INTO "Usuarios" ("Nombre", "Email", "PasswordHash", "Rol", "FechaRegistro")
VALUES (
    'Admin',
    'admin@feller.com',
    'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', -- Password: admin123
    'Admin',
    NOW()
);

-- Crear usuario cliente de prueba
INSERT INTO "Usuarios" ("Nombre", "Email", "PasswordHash", "Telefono", "Rol", "FechaRegistro")
VALUES (
    'Juan Pérez',
    'juan@example.com',
    'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', -- Password: admin123
    '+54 9 11 1234-5678',
 'Cliente',
 NOW()
);

-- Crear un auto de prueba
INSERT INTO "Vehiculos" ("TipoVehiculo", "Marca", "Modelo", "Año", "Precio", "Descripcion", "Disponible", "FechaPublicacion")
VALUES (
    'Auto',
    'Toyota',
    'Corolla',
 2022,
    25000.00,
    'Toyota Corolla 2022 en excelente estado',
    TRUE,
    NOW()
);

-- Crear una moto de prueba
INSERT INTO "Vehiculos" ("TipoVehiculo", "Marca", "Modelo", "Año", "Precio", "Descripcion", "Disponible", "FechaPublicacion")
VALUES (
    'Moto',
    'Honda',
    'CB 500X',
    2023,
  8000.00,
    'Honda CB 500X 2023, única dueña',
    TRUE,
    NOW()
);
```

## ? Verificación Final

Después de aplicar las migraciones, verifica que todo funciona:

```bash
# 1. Ejecutar el proyecto
dotnet run

# 2. Abrir Swagger en el navegador
# http://localhost:5000

# 3. Probar el endpoint de registro
# POST /api/auth/register

# 4. Verificar en la base de datos
# SELECT * FROM "Usuarios";
```

## ?? Próximos Pasos

Una vez que las migraciones estén aplicadas:
1. ? Implementar la lógica de los controladores
2. ? Probar todos los endpoints con Swagger
3. ? Configurar AWS S3 para las imágenes
4. ? Integrar con la API de WhatsApp

---

**Nota**: Recuerda que las migraciones se deben ejecutar cada vez que modifiques los modelos.
