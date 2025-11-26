# ?? Feller Automotores - Backend API

Backend desarrollado en **.NET 8 Web API** para la gestión de una agencia de autos y motos con servicio de lavado.

## ??? Arquitectura del Proyecto

```
FellerBackend/
??? Controllers/          # Controladores de la API
?   ??? AuthController.cs
?   ??? UsuariosController.cs
?   ??? AutosController.cs
?   ??? MotosController.cs
?   ??? TurnosController.cs
?   ??? NotificacionesController.cs
?   ??? DashboardController.cs
??? Models/    # Modelos de entidades
?   ??? Usuario.cs
?   ??? VehiculoBase.cs  (clase abstracta)
?   ??? Auto.cs
?   ??? Moto.cs
?   ??? Turno.cs
?   ??? Notificacion.cs
? ??? ImagenVehiculo.cs
??? DTOs/         # Data Transfer Objects
?   ??? Auth/
?   ??? Autos/
?   ??? Motos/
?   ??? Turnos/
?   ??? Usuarios/
?   ??? Dashboard/
??? Data/    # Entity Framework DbContext
?   ??? FellerDbContext.cs
??? Services/     # Lógica de negocio
?   ??? JwtTokenHelper.cs
?   ??? ImagenService.cs  (AWS S3)
?   ??? WhatsAppService.cs
?   ??? EmailService.cs
?   ??? Interfaces/
??? Helpers/    # Utilidades
?   ??? PasswordHasher.cs
?   ??? ResponseWrapper.cs
??? Program.cs            # Configuración principal
```

## ?? Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Base de datos
- **JWT** - Autenticación y autorización
- **AWS S3** - Almacenamiento de imágenes
- **Swagger/OpenAPI** - Documentación de API

## ?? Paquetes NuGet Instalados

- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)
- `Microsoft.EntityFrameworkCore.Design` (8.0.11)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11)
- `AWSSDK.S3` (4.0.11.3)

## ?? Configuración Inicial

### 1. Configurar appsettings.json

Edita el archivo `appsettings.json` y configura:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=feller_db;Username=TU_USUARIO;Password=TU_PASSWORD"
  },
  "JwtSettings": {
    "Secret": "TU_CLAVE_SECRETA_MUY_SEGURA_DE_AL_MENOS_32_CARACTERES_AQUI",
    "Issuer": "FellerAutomotores",
    "Audience": "FellerAutomotoresApp",
    "ExpirationHours": 24
  },
  "AWS": {
    "Region": "us-east-1",
    "AccessKey": "TU_AWS_ACCESS_KEY",
    "SecretKey": "TU_AWS_SECRET_KEY",
    "BucketName": "feller-automotores"
  }
}
```

### 2. Configurar PostgreSQL

Asegúrate de tener PostgreSQL instalado y corriendo:

```bash
# Linux/Mac con Docker
docker run --name postgres-feller -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres

# O instala PostgreSQL localmente
```

### 3. Ejecutar Migraciones de Entity Framework

```bash
# Crear la primera migración
dotnet ef migrations add InitialCreate

# Aplicar las migraciones a la base de datos
dotnet ef database update
```

### 4. Ejecutar el Proyecto

```bash
dotnet run
```

La API estará disponible en:
- **HTTPS**: `https://localhost:7000`
- **HTTP**: `http://localhost:5000`
- **Swagger**: `http://localhost:5000` (raíz)

## ??? Estructura de la Base de Datos

### Tablas Principales

1. **Usuarios**
   - Gestión de clientes y administradores
   - Autenticación con hash de password
   - Roles: Cliente / Admin

2. **Vehiculos** (TPH - Table Per Hierarchy)
   - Tabla base para autos y motos
   - Discriminador: `TipoVehiculo` (Auto/Moto)
   - Extensible para futuros tipos de vehículos

3. **ImagenesVehiculos**
   - URLs de imágenes almacenadas en AWS S3
   - Relación 1:N con vehículos

4. **Turnos**
   - Gestión de turnos de lavado
   - Estados: Pendiente ? EnProceso ? Finalizado
   - Notificación automática al finalizar

5. **Notificaciones**
   - Historial de notificaciones enviadas
   - Tipos: WhatsApp, Email, SMS

## ?? Autenticación

La API usa **JWT Bearer Tokens**. Para autenticarte:

1. **Registrarse**: `POST /api/auth/register`
2. **Login**: `POST /api/auth/login`
3. Usar el token en el header: `Authorization: Bearer {token}`

## ?? Endpoints Principales

### Auth
- `POST /api/auth/register` - Registro de usuarios
- `POST /api/auth/login` - Login y obtención de token
- `GET /api/auth/me` - Obtener usuario actual (requiere auth)

### Usuarios (Admin)
- `GET /api/usuarios` - Listar todos
- `GET /api/usuarios/{id}` - Obtener por ID
- `PUT /api/usuarios/{id}` - Actualizar
- `DELETE /api/usuarios/{id}` - Eliminar
- `GET /api/usuarios/{id}/turnos` - Turnos de un usuario

### Autos
- `GET /api/autos` - Listar autos
- `GET /api/autos/{id}` - Obtener auto por ID
- `POST /api/autos` - Crear auto (Admin)
- `PUT /api/autos/{id}` - Actualizar auto (Admin)
- `DELETE /api/autos/{id}` - Eliminar auto (Admin)
- `POST /api/autos/{id}/imagenes` - Subir imagen (Admin)
- `DELETE /api/autos/{autoId}/imagenes/{imagenId}` - Borrar imagen (Admin)

### Motos
- Endpoints idénticos a Autos, ruta `/api/motos`

### Turnos
- `GET /api/turnos/mios` - Mis turnos (Cliente)
- `POST /api/turnos` - Crear turno (Cliente)
- `GET /api/turnos` - Listar todos (Admin)
- `PUT /api/turnos/{id}/estado` - Cambiar estado (Admin)
- `DELETE /api/turnos/{id}` - Cancelar turno
- `GET /api/turnos/disponibilidad` - Ver disponibilidad

### Dashboard (Admin)
- `GET /api/dashboard/resumen` - Estadísticas generales

## ??? Gestión de Imágenes con AWS S3

Las imágenes se almacenan en S3 con la siguiente estructura:

```
feller-automotores/
??? autos/
?   ??? 1/
?   ?   ??? {guid}.jpg
?   ? ??? {guid}.jpg
?   ??? 2/
??? motos/
    ??? 1/
```

## ?? Próximos Pasos (Para Implementación)

Actualmente la arquitectura está completa con:
? Estructura de carpetas
? Modelos y DTOs
? DbContext configurado
? Controladores con endpoints definidos
? Servicios (JWT, S3, WhatsApp, Email)
? Program.cs configurado
? Migraciones listas para ejecutar

**Pendiente de implementación:**
- [ ] Lógica de negocio en los controladores (marcados con TODO)
- [ ] Validaciones de datos
- [ ] Manejo de errores global
- [ ] Tests unitarios
- [ ] Integración real con WhatsApp API

## ?? Comandos Útiles de Entity Framework

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update NombreMigracionAnterior

# Eliminar última migración (si no fue aplicada)
dotnet ef migrations remove

# Ver estado de migraciones
dotnet ef migrations list

# Generar script SQL
dotnet ef migrations script

# Eliminar la base de datos
dotnet ef database drop
```

## ?? Seguridad

- Passwords hasheados con SHA256
- JWT con expiración configurable
- CORS configurado para frontend específico
- Autorización por roles (Admin/Cliente)
- URLs presignadas de S3 para mayor seguridad (opcional)

## ?? Contacto

Para más información sobre el proyecto Feller Automotores.

---

**Versión**: 1.0.0  
**Framework**: .NET 8  
**Última actualización**: 2025
