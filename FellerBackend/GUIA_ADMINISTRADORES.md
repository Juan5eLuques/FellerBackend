# ?? GUÍA RÁPIDA: CREAR ADMINISTRADORES

## ?? 3 FORMAS DE CREAR ADMINISTRADORES

---

## ? OPCIÓN 1: Endpoint Automático (MÁS FÁCIL)

**Ventaja**: No necesitas tocar PostgreSQL

### 1. Detener el proyecto actual
```bash
# En la terminal donde está corriendo
Ctrl + C
```

### 2. Ejecutar el proyecto
```bash
dotnet run
```

### 3. Abrir Swagger
```
http://localhost:5000
```

### 4. Usar el endpoint especial
```http
POST /api/seed/create-first-admin
```

**Respuesta**:
```json
{
  "success": true,
  "message": "Usuario administrador creado exitosamente",
  "data": {
    "email": "admin@feller.com",
    "password": "Admin123!",
    "instrucciones": "Usa estas credenciales para hacer login..."
  }
}
```

### 5. Login con las credenciales
```http
POST /api/auth/login
{
  "email": "admin@feller.com",
  "password": "Admin123!"
}
```

### 6. ¡Listo! Ya tienes acceso completo

**?? IMPORTANTE**: Este endpoint solo funciona si NO hay ningún admin. Una vez creado el primero, no funcionará más (por seguridad).

---

## ? OPCIÓN 2: Convertir Usuario Existente (RÁPIDA)

### 1. Registrar un usuario normal
```http
POST /api/auth/register
{
  "nombre": "Tu Nombre",
  "email": "tuadmin@feller.com",
  "password": "TuPassword123!",
  "telefono": "+54 9 11 1234-5678"
}
```

### 2. Conectar a PostgreSQL
```bash
psql -U postgres -h localhost -d feller_db
```

### 3. Actualizar el rol
```sql
UPDATE "Usuarios" 
SET "Rol" = 'Admin' 
WHERE "Email" = 'tuadmin@feller.com';
```

### 4. Verificar
```sql
SELECT "Id", "Nombre", "Email", "Rol" 
FROM "Usuarios" 
WHERE "Rol" = 'Admin';
```

### 5. Logout y Login nuevamente
El nuevo token tendrá el rol Admin.

---

## ? OPCIÓN 3: Datos de Prueba Completos

Si quieres crear datos de ejemplo para probar:

```http
POST /api/seed/seed-test-data
```

Esto crea:
- ? 1 Admin: `admin@feller.com` / `Admin123!`
- ? 1 Cliente: `juan@test.com` / `Password123!`
- ? 2 Autos de ejemplo
- ? 2 Motos de ejemplo

---

## ?? VERIFICAR QUE ERES ADMIN

Después de hacer login:

```http
GET /api/auth/me
Authorization: Bearer {tu_token}
```

Debes ver:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "nombre": "Admin Feller",
    "email": "admin@feller.com",
    "rol": "Admin",  // ? Debe decir "Admin"
    "fechaRegistro": "2024-11-19..."
  }
}
```

---

## ?? ENDPOINTS DE ADMIN DISPONIBLES

Una vez que tengas el token de Admin, puedes acceder a:

### Gestión de Usuarios
```http
GET    /api/usuarios     # Ver todos
GET    /api/usuarios/{id}    # Ver uno
PUT /api/usuarios/{id}      # Actualizar (incluso el rol)
DELETE /api/usuarios/{id}      # Eliminar
GET    /api/usuarios/{id}/turnos  # Ver turnos de un usuario
```

### Gestión de Vehículos
```http
POST   /api/autos   # Crear auto
PUT    /api/autos/{id}         # Actualizar auto
DELETE /api/autos/{id}         # Eliminar auto
POST   /api/autos/{id}/imagenes  # Subir imagen
DELETE /api/autos/{autoId}/imagenes/{imagenId}

# Lo mismo para /api/motos
```

### Gestión de Turnos
```http
GET    /api/turnos       # Ver todos los turnos
PUT    /api/turnos/{id}/estado # Cambiar estado
```

### Dashboard
```http
GET    /api/dashboard/resumen  # Estadísticas
```

### Notificaciones
```http
GET    /api/notificaciones  # Historial
POST   /api/notificaciones/whatsapp  # Enviar manual
```

---

## ??? GESTIONAR ROLES DE USUARIOS

### Ver todos los usuarios y sus roles
```http
GET /api/usuarios
Authorization: Bearer {admin_token}
```

### Actualizar el rol de un usuario
```http
PUT /api/usuarios/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "rol": "Admin"
}
```

### Quitar rol de Admin (volver a Cliente)
```http
PUT /api/usuarios/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "rol": "Cliente"
}
```

---

## ?? MEJORES PRÁCTICAS

### 1. Contraseñas Seguras para Admins
- Mínimo 12 caracteres
- Mayúsculas y minúsculas
- Números y símbolos
- ? Ejemplo: `Admin2025!Feller#`

### 2. Emails Profesionales
- ? `admin@feller.com`
- ? `gerencia@feller.com`
- ? `backoffice@feller.com`
- ? `test@test.com`

### 3. Múltiples Admins
Crea al menos 2 admins para evitar quedar sin acceso:

```sql
-- Admin principal
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'admin@feller.com';

-- Admin secundario
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = 'gerente@feller.com';
```

### 4. Cambiar Contraseña Después del Primer Login
Si usas el endpoint automático, cambia la contraseña inmediatamente.

---

## ?? TESTING RÁPIDO

### 1. Crear datos de prueba
```http
POST /api/seed/seed-test-data
```

### 2. Login como admin
```http
POST /api/auth/login
{
  "email": "admin@feller.com",
  "password": "Admin123!"
}
```

### 3. Probar endpoints de admin
```http
GET /api/usuarios
GET /api/dashboard/resumen
POST /api/autos
```

### 4. Limpiar cuando termines
```http
DELETE /api/seed/delete-all-data?confirmacion=CONFIRMO_ELIMINAR_TODO
```

---

## ?? IMPORTANTE PARA PRODUCCIÓN

### Antes de ir a producción:

1. ? **ELIMINAR** el `SeedController.cs` completo
2. ? Cambiar todas las contraseñas por defecto
3. ? Usar variables de entorno para secrets
4. ? Verificar que solo hay usuarios reales
5. ? Configurar backup automático de PostgreSQL

---

## ?? CHECKLIST DE SEGURIDAD

- [ ] Crear al menos 2 usuarios Admin
- [ ] Usar contraseñas seguras (12+ caracteres)
- [ ] Cambiar contraseñas por defecto
- [ ] Usar emails profesionales
- [ ] Guardar credenciales en lugar seguro
- [ ] Eliminar SeedController en producción
- [ ] Configurar backup de base de datos

---

## ?? SOLUCIÓN DE PROBLEMAS

### "Ya existe un usuario administrador"
- Es normal si ya creaste uno
- Usa la Opción 2 para crear más admins

### "Token inválido"
- Haz logout y login nuevamente
- El rol se actualiza solo con un nuevo token

### "No autorizado" en endpoints de admin
- Verifica con `GET /api/auth/me` que tu rol sea "Admin"
- Verifica que el token esté en el header `Authorization: Bearer {token}`

### Olvidé la contraseña del admin
```sql
-- Opción 1: Resetear a password conocido
-- 1. Crear usuario temporal y copiar su hash
-- 2. Actualizar el admin con ese hash

-- Opción 2: Promover otro usuario a admin
UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Id" = 2;
```

---

## ?? TIP PRO

**Guarda estas credenciales en un lugar seguro:**

```
Proyecto: Feller Automotores Backend
Admin Principal:
- Email: admin@feller.com
- Password: [TU_PASSWORD_SEGURO]
- Rol: Admin

Admin Secundario:
- Email: gerente@feller.com
- Password: [PASSWORD_BACKUP]
- Rol: Admin
```

---

## ?? Resumen Rápido

**Para crear el PRIMER admin:**
1. `POST /api/seed/create-first-admin`
2. Login con `admin@feller.com` / `Admin123!`
3. ¡Listo!

**Para crear MÁS admins:**
1. Registrar usuario normal
2. SQL: `UPDATE "Usuarios" SET "Rol" = 'Admin' WHERE "Email" = '...'`
3. Login nuevamente

**Para gestionar usuarios desde el backoffice:**
- `GET /api/usuarios` - Ver todos
- `PUT /api/usuarios/{id}` con `{"rol": "Admin"}` - Promover
- `PUT /api/usuarios/{id}` con `{"rol": "Cliente"}` - Despromover

---

?? **¡Ya puedes gestionar tu backoffice completamente!**
