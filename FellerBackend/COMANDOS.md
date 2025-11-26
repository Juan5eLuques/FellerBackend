# ??? COMANDOS ÚTILES - Feller Automotores

Referencia rápida de todos los comandos útiles para el proyecto.

---

## ?? COMANDOS BÁSICOS

### Compilar el proyecto
```bash
dotnet build
```

### Ejecutar el proyecto
```bash
dotnet run

# Con hot reload (recompila automáticamente)
dotnet watch run
```

### Limpiar archivos de compilación
```bash
dotnet clean
```

### Restaurar paquetes NuGet
```bash
dotnet restore
```

---

## ?? GESTIÓN DE PAQUETES

### Ver paquetes instalados
```bash
dotnet list package
```

### Agregar un nuevo paquete
```bash
dotnet add package NombreDelPaquete

# Con versión específica
dotnet add package NombreDelPaquete --version 1.2.3
```

### Actualizar un paquete
```bash
dotnet add package NombreDelPaquete

# Actualizar todos
dotnet list package --outdated
```

### Eliminar un paquete
```bash
dotnet remove package NombreDelPaquete
```

---

## ??? ENTITY FRAMEWORK - MIGRACIONES

### Instalar herramientas EF (solo primera vez)
```bash
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

### Crear primera migración
```bash
dotnet ef migrations add InitialCreate
```

### Crear migraciones adicionales
```bash
dotnet ef migrations add NombreDeLaMigracion

# Ejemplos:
dotnet ef migrations add AddPhotoToUsuario
dotnet ef migrations add UpdateTurnoTable
```

### Aplicar migraciones
```bash
# Aplicar todas las pendientes
dotnet ef database update

# Aplicar hasta una migración específica
dotnet ef database update NombreMigracion

# Aplicar la última migración
dotnet ef database update --verbose
```

### Ver migraciones
```bash
# Listar todas
dotnet ef migrations list

# Ver detalles de la última
dotnet ef migrations list --verbose
```

### Generar script SQL
```bash
# Script completo
dotnet ef migrations script

# Script desde una migración específica
dotnet ef migrations script MigracionInicial MigracionFinal

# Guardar en archivo
dotnet ef migrations script > migration.sql
```

### Revertir migraciones
```bash
# Revertir a la migración anterior
dotnet ef database update NombreMigracionAnterior

# Revertir todas (base de datos vacía)
dotnet ef database update 0
```

### Eliminar última migración
```bash
# Solo si NO fue aplicada a la BD
dotnet ef migrations remove

# Forzar eliminación
dotnet ef migrations remove --force
```

### Eliminar base de datos
```bash
dotnet ef database drop

# Sin confirmación
dotnet ef database drop --force
```

---

## ?? POSTGRESQL - COMANDOS

### Conectar a PostgreSQL
```bash
# Desde terminal
psql -U postgres -h localhost -p 5432

# Conectar a base de datos específica
psql -U postgres -d feller_db
```

### Comandos dentro de psql
```sql
-- Listar bases de datos
\l

-- Conectar a una base de datos
\c feller_db

-- Listar tablas
\dt

-- Ver estructura de una tabla
\d "Usuarios"
\d "Vehiculos"

-- Ver todas las columnas
\d+ "Usuarios"

-- Listar esquemas
\dn

-- Salir
\q
```

### Consultas SQL útiles
```sql
-- Ver todos los usuarios
SELECT * FROM "Usuarios";

-- Ver autos disponibles
SELECT * FROM "Vehiculos" WHERE "TipoVehiculo" = 'Auto' AND "Disponible" = true;

-- Ver turnos pendientes
SELECT * FROM "Turnos" WHERE "Estado" = 'Pendiente';

-- Contar registros
SELECT COUNT(*) FROM "Usuarios";
SELECT COUNT(*) FROM "Vehiculos";
SELECT COUNT(*) FROM "Turnos";

-- Ver turnos con datos de usuario
SELECT t.*, u."Nombre", u."Email" 
FROM "Turnos" t 
INNER JOIN "Usuarios" u ON t."UsuarioId" = u."Id";

-- Eliminar todos los datos (cuidado!)
TRUNCATE TABLE "Usuarios" CASCADE;
```

### Backup y Restore
```bash
# Backup
pg_dump -U postgres feller_db > backup.sql

# Restore
psql -U postgres feller_db < backup.sql
```

---

## ?? DOCKER - POSTGRESQL

### Crear contenedor PostgreSQL
```bash
docker run --name postgres-feller \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres
```

### Gestión del contenedor
```bash
# Ver contenedores corriendo
docker ps

# Ver todos los contenedores
docker ps -a

# Iniciar contenedor
docker start postgres-feller

# Detener contenedor
docker stop postgres-feller

# Reiniciar contenedor
docker restart postgres-feller

# Eliminar contenedor
docker rm postgres-feller

# Ver logs
docker logs postgres-feller

# Conectar al contenedor
docker exec -it postgres-feller psql -U postgres
```

### Crear contenedor con volumen persistente
```bash
docker run --name postgres-feller \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -v postgres-feller-data:/var/lib/postgresql/data \
  -d postgres
```

---

## ?? GIT - CONTROL DE VERSIONES

### Inicializar repositorio
```bash
git init
```

### Configuración inicial
```bash
git config user.name "Tu Nombre"
git config user.email "tu@email.com"
```

### Comandos básicos
```bash
# Ver estado
git status

# Agregar archivos
git add .
git add archivo.cs

# Commit
git commit -m "Mensaje del commit"

# Ver historial
git log
git log --oneline

# Ver diferencias
git diff
```

### Branches
```bash
# Crear branch
git branch nombre-feature

# Cambiar de branch
git checkout nombre-feature

# Crear y cambiar
git checkout -b nombre-feature

# Listar branches
git branch

# Eliminar branch
git branch -d nombre-feature
```

### Remote
```bash
# Agregar remote
git remote add origin https://github.com/usuario/repo.git

# Subir cambios
git push origin main

# Bajar cambios
git pull origin main

# Ver remotes
git remote -v
```

---

## ?? TESTING (Para cuando agregues tests)

### Crear proyecto de tests
```bash
dotnet new xunit -n FellerBackend.Tests
dotnet add FellerBackend.Tests reference FellerBackend
```

### Ejecutar tests
```bash
# Todos los tests
dotnet test

# Con detalles
dotnet test --verbosity detailed

# Con cobertura
dotnet test /p:CollectCoverage=true
```

---

## ?? DEBUGGING

### Ver información detallada
```bash
# Ver versión de .NET
dotnet --version

# Ver SDKs instalados
dotnet --list-sdks

# Ver runtimes instalados
dotnet --list-runtimes

# Información del proyecto
dotnet --info
```

### Limpiar caché de NuGet
```bash
dotnet nuget locals all --clear
```

### Compilar en Release
```bash
dotnet build --configuration Release
dotnet run --configuration Release
```

---

## ?? SWAGGER / API

### Abrir Swagger
```bash
# Iniciar proyecto
dotnet run

# Abrir navegador en:
http://localhost:5000
```

### Probar endpoints con curl
```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Test","email":"test@test.com","password":"Pass123!"}'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Pass123!"}'

# Get con token
curl -X GET http://localhost:5000/api/autos \
  -H "Authorization: Bearer TU_TOKEN_AQUI"
```

---

## ?? SECRETS (Producción)

### User Secrets (Desarrollo)
```bash
# Inicializar secrets
dotnet user-secrets init

# Agregar secret
dotnet user-secrets set "JwtSettings:Secret" "TU_CLAVE_SECRETA"
dotnet user-secrets set "AWS:AccessKey" "TU_ACCESS_KEY"

# Listar secrets
dotnet user-secrets list

# Eliminar secret
dotnet user-secrets remove "JwtSettings:Secret"

# Limpiar todos
dotnet user-secrets clear
```

---

## ?? PUBLICACIÓN

### Publicar aplicación
```bash
# Publicar para producción
dotnet publish -c Release -o ./publish

# Publicar como ejecutable único
dotnet publish -c Release -r win-x64 --self-contained

# Para Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## ?? TROUBLESHOOTING

### Error: Puerto en uso
```bash
# Windows - Liberar puerto 5000
netstat -ano | findstr :5000
taskkill /PID [numero] /F

# Linux/Mac
lsof -i :5000
kill -9 [PID]
```

### Error: Cannot find dotnet ef
```bash
dotnet tool install --global dotnet-ef
# o
dotnet tool update --global dotnet-ef
```

### Error: Build failed
```bash
dotnet clean
dotnet restore
dotnet build
```

### Error: PostgreSQL connection refused
```bash
# Verificar que PostgreSQL está corriendo
docker ps
# o
pg_ctl status

# Verificar puerto
netstat -an | findstr 5432
```

---

## ? ATAJOS ÚTILES

### Reinicio completo del proyecto
```bash
# Limpiar todo y recompilar
dotnet clean
dotnet restore
dotnet build
dotnet run
```

### Recrear base de datos desde cero
```bash
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Ver información completa del proyecto
```bash
dotnet build --verbosity detailed
```

---

## ?? NOTAS IMPORTANTES

1. **Siempre** hacer backup antes de `database drop`
2. **Nunca** commitear archivos con secrets (appsettings.Production.json)
3. **Verificar** que PostgreSQL esté corriendo antes de migraciones
4. **Compilar** después de agregar nuevos paquetes
5. **Usar** `dotnet watch run` durante desarrollo para hot reload

---

**Tip**: Guarda este archivo en favoritos para acceso rápido a comandos ??
