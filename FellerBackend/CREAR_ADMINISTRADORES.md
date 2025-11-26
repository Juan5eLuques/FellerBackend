# ?? CREAR USUARIOS ADMINISTRADORES

## Opción 1: Script SQL Directo (Recomendado para el primer admin)

### 1. Conectar a PostgreSQL
```bash
psql -U postgres -h localhost -d feller_db
```

### 2. Crear usuario Admin manualmente
```sql
-- Ver el hash de password para "admin123"
-- Este es un hash de ejemplo, deberías generar uno nuevo

-- Insertar usuario administrador
INSERT INTO "Usuarios" ("Nombre", "Email", "PasswordHash", "Telefono", "Rol", "FechaRegistro")
VALUES (
    'Administrador Principal',
    'admin@feller.com',
    'HASH_AQUI',  -- Reemplazar con hash real
    '+54 9 11 9999-9999',
    'Admin',
    NOW()
);

-- Verificar que se creó
SELECT "Id", "Nombre", "Email", "Rol" FROM "Usuarios" WHERE "Rol" = 'Admin';
```

### 3. Obtener el hash de password

Para obtener el hash de una contraseña específica, ejecuta el proyecto y usa el endpoint de registro primero, luego copia el hash de la base de datos:

```sql
-- Opción A: Crear usuario temporal como Cliente
-- 1. Registrarse con POST /api/auth/register
-- 2. Obtener el hash de la BD:
SELECT "PasswordHash" FROM "Usuarios" WHERE "Email" = 'temporal@test.com';

-- 3. Usar ese hash para crear el admin:
INSERT INTO "Usuarios" ("Nombre", "Email", "PasswordHash", "Telefono", "Rol", "FechaRegistro")
VALUES (
    'Admin Sistema',
    'admin@feller.com',
    'COPIAR_HASH_AQUI',
    '+54 9 11 0000-0000',
    'Admin',
    NOW()
);

-- 4. Eliminar el usuario temporal
DELETE FROM "Usuarios" WHERE "Email" = 'temporal@test.com';
```

---

## Opción 2: Actualizar un usuario existente a Admin

Si ya tienes un usuario registrado, simplemente actualízalo:

```sql
-- Ver todos los usuarios
SELECT "Id", "Nombre", "Email", "Rol" FROM "Usuarios";

-- Actualizar un usuario a Admin
UPDATE "Usuarios" 
SET "Rol" = 'Admin' 
WHERE "Email" = 'tu_email@example.com';

-- Verificar el cambio
SELECT "Id", "Nombre", "Email", "Rol" FROM "Usuarios" WHERE "Rol" = 'Admin';
```

---

## Opción 3: Endpoint especial para crear el primer Admin (Ver siguiente sección)

---

## ?? Proceso Completo Recomendado

### Paso 1: Registrar un usuario normal
```bash
# Desde Swagger o Postman
POST http://localhost:5000/api/auth/register
{
  "nombre": "Admin Principal",
  "email": "admin@feller.com",
  "password": "Admin123!Seguro",
  "telefono": "+54 9 11 1234-5678"
}
```

### Paso 2: Convertir a Admin en PostgreSQL
```bash
# Conectar a PostgreSQL
psql -U postgres -h localhost -d feller_db

# Ejecutar
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'admin@feller.com';
```

### Paso 3: Hacer logout y login nuevamente
```bash
POST http://localhost:5000/api/auth/login
{
  "email": "admin@feller.com",
  "password": "Admin123!Seguro"
}
```

El nuevo token JWT ahora tendrá el rol "Admin" y podrás acceder a todos los endpoints protegidos.

---

## ?? Seguridad

### Contraseñas recomendadas para Admin:
- Mínimo 12 caracteres
- Mayúsculas y minúsculas
- Números y símbolos
- Ejemplo: `Admin2025!Feller#Secure`

### Emails recomendados:
- `admin@feller.com`
- `backoffice@feller.com`
- `gerencia@feller.com`

---

## ?? Gestión de Usuarios desde el Backoffice

Una vez que tengas un usuario Admin, puedes usar estos endpoints para gestionar usuarios:

### Ver todos los usuarios
```http
GET /api/usuarios
Authorization: Bearer {admin_token}
```

### Ver usuario específico
```http
GET /api/usuarios/{id}
Authorization: Bearer {admin_token}
```

### Actualizar rol de usuario
```http
PUT /api/usuarios/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "rol": "Admin"
}
```

### Eliminar usuario
```http
DELETE /api/usuarios/{id}
Authorization: Bearer {admin_token}
```

---

## ?? Verificar si eres Admin

Una vez que hagas login:

```http
GET /api/auth/me
Authorization: Bearer {token}
```

Deberías ver:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "nombre": "Admin Principal",
 "email": "admin@feller.com",
    "rol": "Admin",  // <-- Verificar que diga "Admin"
    "fechaRegistro": "2024-11-19T..."
  }
}
```

---

## ? Script de Inicialización Rápida

```sql
-- Script completo para crear primer admin
-- EJECUTAR EN PostgreSQL

-- 1. Verificar que la tabla existe
SELECT COUNT(*) FROM "Usuarios";

-- 2. Si no hay usuarios, necesitas crear uno normal primero desde la API
-- Luego ejecutar:

-- 3. Convertir el primer usuario en Admin
UPDATE "Usuarios" 
SET "Rol" = 'Admin' 
WHERE "Id" = 1;

-- 4. O crear múltiples admins
UPDATE "Usuarios" 
SET "Rol" = 'Admin' 
WHERE "Email" IN ('admin@feller.com', 'gerencia@feller.com');

-- 5. Ver todos los admins
SELECT "Id", "Nombre", "Email", "Rol", "FechaRegistro" 
FROM "Usuarios" 
WHERE "Rol" = 'Admin';
```

---

## ?? Orden de Operaciones Recomendado

1. ? Ejecutar el proyecto: `dotnet run`
2. ? Registrar primer usuario: `POST /api/auth/register`
3. ? Conectar a PostgreSQL: `psql -U postgres -d feller_db`
4. ? Actualizar a Admin: `UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Id" = 1;`
5. ? Hacer logout
6. ? Login nuevamente: `POST /api/auth/login`
7. ? Verificar rol: `GET /api/auth/me`
8. ? Probar endpoints de admin: `GET /api/usuarios`

---

## ?? Roles Disponibles

El sistema actualmente soporta dos roles:

| Rol | Descripción | Acceso |
|-----|-------------|--------|
| **Cliente** | Usuario normal | - Ver vehículos<br>- Crear turnos<br>- Ver sus propios turnos |
| **Admin** | Administrador | - Todo lo de Cliente<br>- Gestionar usuarios<br>- Gestionar vehículos<br>- Ver todos los turnos<br>- Dashboard<br>- Enviar notificaciones |

---

## ?? Contacto de Administradores

Puedes crear múltiples admins para diferentes personas:

```sql
-- Admin principal
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'admin@feller.com';

-- Gerente
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'gerente@feller.com';

-- Backoffice
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'backoffice@feller.com';
```

---

**Consejo**: Siempre ten al menos 2 usuarios Admin para evitar quedarte sin acceso si uno olvida su contraseña.
