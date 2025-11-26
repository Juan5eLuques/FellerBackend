# ?? API Endpoints - Feller Automotores

Documentación completa de todos los endpoints disponibles en la API.

## ?? Base URL
```
http://localhost:5000/api
https://localhost:7000/api
```

## ?? Tabla de Contenidos
- [Autenticación](#-autenticación)
- [Usuarios](#-usuarios)
- [Autos](#-autos)
- [Motos](#-motos)
- [Turnos](#-turnos)
- [Notificaciones](#-notificaciones)
- [Dashboard](#-dashboard)

---

## ?? Autenticación

### Registrar Usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "nombre": "Juan Pérez",
  "email": "juan@example.com",
  "password": "Password123!",
  "telefono": "+54 9 11 1234-5678"
}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "juan@example.com",
    "nombre": "Juan Pérez",
    "rol": "Cliente"
  }
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "Password123!"
}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "juan@example.com",
    "nombre": "Juan Pérez",
    "rol": "Cliente"
  }
}
```

### Obtener Usuario Actual
```http
GET /api/auth/me
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "nombre": "Juan Pérez",
    "email": "juan@example.com",
 "telefono": "+54 9 11 1234-5678",
    "rol": "Cliente",
    "fechaRegistro": "2025-01-15T10:30:00Z"
  }
}
```

---

## ?? Usuarios

**Nota**: Todos los endpoints requieren rol `Admin` excepto `GET /api/usuarios/{id}`

### Listar Todos los Usuarios
```http
GET /api/usuarios
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "nombre": "Juan Pérez",
      "email": "juan@example.com",
  "telefono": "+54 9 11 1234-5678",
      "rol": "Cliente",
      "fechaRegistro": "2025-01-15T10:30:00Z"
  },
    {
      "id": 2,
      "nombre": "Admin",
      "email": "admin@feller.com",
      "telefono": null,
      "rol": "Admin",
      "fechaRegistro": "2025-01-10T08:00:00Z"
    }
  ]
}
```

### Obtener Usuario por ID
```http
GET /api/usuarios/{id}
Authorization: Bearer {token}
```

### Actualizar Usuario
```http
PUT /api/usuarios/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "nombre": "Juan Carlos Pérez",
  "email": "juancarlos@example.com",
  "telefono": "+54 9 11 9999-8888",
  "rol": "Admin"
}
```

### Eliminar Usuario
```http
DELETE /api/usuarios/{id}
Authorization: Bearer {token}
```

### Obtener Turnos de un Usuario
```http
GET /api/usuarios/{id}/turnos
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": [
 {
    "id": 1,
      "usuarioId": 1,
      "nombreUsuario": "Juan Pérez",
      "emailUsuario": "juan@example.com",
      "fecha": "2025-01-20",
      "hora": "10:00:00",
      "tipoLavado": "Completo",
      "estado": "Pendiente",
      "fechaFinalizacion": null,
      "fechaCreacion": "2025-01-15T14:20:00Z"
    }
  ]
}
```

---

## ?? Autos

### Listar Todos los Autos
```http
GET /api/autos
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "marca": "Toyota",
      "modelo": "Corolla",
      "año": 2022,
      "precio": 25000.00,
      "descripcion": "Toyota Corolla 2022 en excelente estado",
   "disponible": true,
      "fechaPublicacion": "2025-01-10T08:00:00Z",
      "imagenes": [
        {
    "id": 1,
          "url": "https://feller-automotores.s3.amazonaws.com/autos/1/abc123.jpg"
   },
   {
    "id": 2,
  "url": "https://feller-automotores.s3.amazonaws.com/autos/1/def456.jpg"
  }
      ]
    }
  ]
}
```

### Obtener Auto por ID
```http
GET /api/autos/{id}
```

### Crear Auto
```http
POST /api/autos
Authorization: Bearer {token}
Content-Type: application/json

{
  "marca": "Ford",
  "modelo": "Focus",
  "año": 2023,
  "precio": 22000.00,
  "descripcion": "Ford Focus 2023, full equipo",
  "disponible": true
}
```

### Actualizar Auto
```http
PUT /api/autos/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "precio": 21000.00,
  "disponible": false
}
```

### Eliminar Auto
```http
DELETE /api/autos/{id}
Authorization: Bearer {token}
```

### Subir Imagen de Auto
```http
POST /api/autos/{id}/imagenes
Authorization: Bearer {token}
Content-Type: multipart/form-data

file: [archivo de imagen]
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "message": "Imagen subida exitosamente",
  "data": {
    "id": 3,
    "url": "https://feller-automotores.s3.amazonaws.com/autos/1/xyz789.jpg"
  }
}
```

### Eliminar Imagen de Auto
```http
DELETE /api/autos/{autoId}/imagenes/{imagenId}
Authorization: Bearer {token}
```

---

## ??? Motos

**Nota**: Los endpoints de motos son idénticos a los de autos, pero con la ruta `/api/motos`

### Listar Todas las Motos
```http
GET /api/motos
```

### Obtener Moto por ID
```http
GET /api/motos/{id}
```

### Crear Moto
```http
POST /api/motos
Authorization: Bearer {token}
Content-Type: application/json

{
  "marca": "Honda",
  "modelo": "CB 500X",
  "año": 2023,
  "precio": 8000.00,
  "descripcion": "Honda CB 500X 2023, única dueña",
  "disponible": true
}
```

### Actualizar Moto
```http
PUT /api/motos/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

### Eliminar Moto
```http
DELETE /api/motos/{id}
Authorization: Bearer {token}
```

### Subir Imagen de Moto
```http
POST /api/motos/{id}/imagenes
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

### Eliminar Imagen de Moto
```http
DELETE /api/motos/{motoId}/imagenes/{imagenId}
Authorization: Bearer {token}
```

---

## ?? Turnos

### Obtener Mis Turnos (Cliente)
```http
GET /api/turnos/mios
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "usuarioId": 1,
      "nombreUsuario": "Juan Pérez",
  "emailUsuario": "juan@example.com",
      "fecha": "2025-01-20",
 "hora": "10:00:00",
      "tipoLavado": "Completo",
      "estado": "Pendiente",
      "fechaFinalizacion": null,
      "fechaCreacion": "2025-01-15T14:20:00Z"
 }
  ]
}
```

### Crear Turno
```http
POST /api/turnos
Authorization: Bearer {token}
Content-Type: application/json

{
  "fecha": "2025-01-25",
  "hora": "14:00:00",
  "tipoLavado": "Básico"
}
```

**Tipos de lavado disponibles**:
- `Básico`
- `Completo`
- `Premium`

### Listar Todos los Turnos (Admin)
```http
GET /api/turnos
Authorization: Bearer {token}
```

### Actualizar Estado de Turno (Admin)
```http
PUT /api/turnos/{id}/estado
Authorization: Bearer {token}
Content-Type: application/json

{
  "estado": "EnProceso"
}
```

**Estados disponibles**:
- `Pendiente`
- `EnProceso`
- `Finalizado`
- `Cancelado`

**Nota**: Cuando el estado cambia a `Finalizado`, se envía automáticamente un mensaje de WhatsApp al cliente.

### Cancelar Turno
```http
DELETE /api/turnos/{id}
Authorization: Bearer {token}
```

### Verificar Disponibilidad
```http
GET /api/turnos/disponibilidad?fecha=2025-01-25
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": {
    "fecha": "2025-01-25",
    "horariosDisponibles": [
    "09:00:00",
      "10:00:00",
      "11:00:00",
      "14:00:00",
      "15:00:00",
   "16:00:00"
    ],
  "horariosOcupados": [
      "12:00:00",
      "13:00:00"
    ]
  }
}
```

---

## ?? Notificaciones

**Nota**: Todos los endpoints requieren rol `Admin`

### Listar Todas las Notificaciones
```http
GET /api/notificaciones
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "usuarioId": 1,
      "mensaje": "Su turno de lavado ha finalizado",
      "tipo": "WhatsApp",
      "fechaEnvio": "2025-01-15T16:30:00Z",
      "enviada": true
    }
  ]
}
```

### Enviar WhatsApp Manual
```http
POST /api/notificaciones/whatsapp
Authorization: Bearer {token}
Content-Type: application/json

{
  "usuarioId": 1,
  "mensaje": "Tenemos una promoción especial para ti"
}
```

---

## ?? Dashboard

**Nota**: Todos los endpoints requieren rol `Admin`

### Obtener Resumen del Dashboard
```http
GET /api/dashboard/resumen
Authorization: Bearer {token}
```

**Respuesta exitosa (200)**:
```json
{
  "success": true,
  "data": {
    "autosPublicados": 15,
    "motosPublicadas": 8,
    "turnosDelDia": 5,
    "usuariosRegistrados": 42,
    "turnosPendientes": 3,
    "turnosEnProceso": 2
  }
}
```

---

## ?? Códigos de Estado HTTP

| Código | Descripción |
|--------|-------------|
| 200 | Operación exitosa |
| 201 | Recurso creado exitosamente |
| 400 | Solicitud incorrecta (datos inválidos) |
| 401 | No autenticado (token inválido o ausente) |
| 403 | No autorizado (sin permisos) |
| 404 | Recurso no encontrado |
| 500 | Error interno del servidor |

## ??? Autenticación

Todos los endpoints marcados con ?? requieren un token JWT en el header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Para obtener un token:
1. Registrarse con `POST /api/auth/register`
2. O hacer login con `POST /api/auth/login`

## ?? Roles

- **Cliente**: Puede ver vehículos, crear turnos, ver sus propios turnos
- **Admin**: Acceso completo a todos los endpoints

---

**Swagger UI**: Para probar los endpoints interactivamente, visita `http://localhost:5000`
