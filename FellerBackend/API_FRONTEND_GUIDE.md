# ?? GUÍA COMPLETA DE API PARA FRONTEND - Feller Automotores

## ?? Base URL

```
http://localhost:5000/api
```

---

## ?? AUTENTICACIÓN

Todos los endpoints que requieren autenticación necesitan el header:
```
Authorization: Bearer {token}
```

---

## ?? TABLA DE CONTENIDOS

1. [Autenticación](#1-autenticación)
2. [Usuarios](#2-usuarios-admin)
3. [Autos](#3-autos)
4. [Motos](#4-motos)
5. [Turnos](#5-turnos)
6. [Notificaciones](#6-notificaciones-admin)
7. [Dashboard](#7-dashboard-admin)
8. [Seed (Solo Desarrollo)](#8-seed-solo-desarrollo)

---

## 1. ?? AUTENTICACIÓN

### 1.1 Registro de Usuario

**Endpoint**: `POST /api/auth/register`  
**Auth**: No requerida  
**Descripción**: Registrar un nuevo usuario (rol Cliente por defecto)

**Request Body**:
```json
{
  "nombre": "Juan Pérez",
  "email": "juan@example.com",
  "password": "Password123!",
  "telefono": "+54 9 11 1234-5678"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "juan@example.com",
    "nombre": "Juan Pérez",
    "rol": "Cliente"
  },
  "errors": null
}
```

**Response Error (400)**:
```json
{
  "success": false,
  "message": "El email ya está registrado",
  "data": null,
  "errors": null
}
```

**Validaciones Frontend**:
- Nombre: requerido, min 2 caracteres
- Email: requerido, formato válido
- Password: requerido, min 6 caracteres
- Teléfono: opcional, formato +54 9 11 1234-5678

---

### 1.2 Login

**Endpoint**: `POST /api/auth/login`  
**Auth**: No requerida  
**Descripción**: Iniciar sesión

**Request Body**:
```json
{
  "email": "juan@example.com",
  "password": "Password123!"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "juan@example.com",
    "nombre": "Juan Pérez",
    "rol": "Cliente"
  },
  "errors": null
}
```

**Response Error (401)**:
```json
{
  "success": false,
  "message": "Email o contraseña incorrectos",
  "data": null,
  "errors": null
}
```

**Qué hacer con el token**:
```javascript
// Guardar en localStorage
localStorage.setItem('token', response.data.token);
localStorage.setItem('userRole', response.data.rol);
localStorage.setItem('userName', response.data.nombre);

// Usar en requests posteriores
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
```

---

### 1.3 Obtener Usuario Actual

**Endpoint**: `GET /api/auth/me`  
**Auth**: ? Requerida  
**Descripción**: Obtener datos del usuario logueado

**Request Headers**:
```
Authorization: Bearer {token}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": {
  "id": 1,
    "nombre": "Juan Pérez",
    "email": "juan@example.com",
    "telefono": "+54 9 11 1234-5678",
    "rol": "Cliente",
    "fechaRegistro": "2024-11-19T10:30:00Z"
  },
  "errors": null
}
```

**Uso Frontend**:
```javascript
// Al cargar la app, verificar si hay sesión
const token = localStorage.getItem('token');
if (token) {
  const userData = await axios.get('/api/auth/me');
  // Actualizar estado global con userData
}
```

---

## 2. ?? USUARIOS (ADMIN)

### 2.1 Listar Todos los Usuarios

**Endpoint**: `GET /api/usuarios`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Obtener lista de todos los usuarios

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "nombre": "Juan Pérez",
"email": "juan@example.com",
      "telefono": "+54 9 11 1234-5678",
      "rol": "Cliente",
  "fechaRegistro": "2024-11-19T10:30:00Z"
    },
    {
      "id": 2,
      "nombre": "Admin",
      "email": "admin@feller.com",
      "telefono": "+54 9 11 9999-9999",
  "rol": "Admin",
      "fechaRegistro": "2024-11-18T08:00:00Z"
    }
  ],
  "errors": null
}
```

**Componente Frontend Sugerido**:
- Tabla con columnas: Nombre, Email, Teléfono, Rol, Fecha Registro
- Botones: Ver, Editar, Eliminar
- Badge para mostrar el rol (color diferente para Admin)

---

### 2.2 Obtener Usuario por ID

**Endpoint**: `GET /api/usuarios/{id}`  
**Auth**: ? Requerida  
**Descripción**: Obtener detalles de un usuario específico

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 1,
    "nombre": "Juan Pérez",
    "email": "juan@example.com",
    "telefono": "+54 9 11 1234-5678",
    "rol": "Cliente",
    "fechaRegistro": "2024-11-19T10:30:00Z"
  },
  "errors": null
}
```

---

### 2.3 Actualizar Usuario

**Endpoint**: `PUT /api/usuarios/{id}`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Actualizar datos de un usuario (incluyendo rol)

**Request Body** (todos los campos opcionales):
```json
{
  "nombre": "Juan Carlos Pérez",
  "email": "juancarlos@example.com",
  "telefono": "+54 9 11 8888-8888",
  "rol": "Admin"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuario actualizado exitosamente",
  "data": {
    "id": 1,
    "nombre": "Juan Carlos Pérez",
 "email": "juancarlos@example.com",
 "telefono": "+54 9 11 8888-8888",
    "rol": "Admin",
    "fechaRegistro": "2024-11-19T10:30:00Z"
  },
  "errors": null
}
```

**Nota**: Para promover a Admin, enviar `"rol": "Admin"`

---

### 2.4 Eliminar Usuario

**Endpoint**: `DELETE /api/usuarios/{id}`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Eliminar un usuario

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuario eliminado exitosamente",
  "data": null,
  "errors": null
}
```

**Confirmación Frontend**:
```javascript
if (confirm('¿Estás seguro de eliminar este usuario?')) {
  await axios.delete(`/api/usuarios/${userId}`);
}
```

---

### 2.5 Ver Turnos de un Usuario

**Endpoint**: `GET /api/usuarios/{id}/turnos`  
**Auth**: ? Requerida  
**Descripción**: Obtener todos los turnos de un usuario específico

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
    "usuarioId": 1,
  "nombreUsuario": "Juan Pérez",
      "emailUsuario": "juan@example.com",
      "fecha": "2024-11-25T00:00:00Z",
      "hora": "10:00:00",
      "tipoLavado": "Completo",
      "estado": "Pendiente",
      "fechaFinalizacion": null,
      "fechaCreacion": "2024-11-19T14:20:00Z"
    }
  ],
  "errors": null
}
```

---

## 3. ?? AUTOS

### 3.1 Listar Todos los Autos

**Endpoint**: `GET /api/autos`  
**Auth**: No requerida  
**Descripción**: Obtener lista de todos los autos con imágenes

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "marca": "Toyota",
    "modelo": "Corolla",
      "año": 2022,
      "precio": 25000.00,
      "descripcion": "Toyota Corolla 2022 en excelente estado",
      "disponible": true,
      "fechaPublicacion": "2024-11-10T08:00:00Z",
      "imagenes": [
     {
       "id": 1,
   "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/abc123.jpg"
    },
        {
     "id": 2,
          "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/def456.jpg"
        }
      ]
    }
  ],
  "errors": null
}
```

**Componente Frontend Sugerido**:
- Cards con imagen principal (primera del array)
- Título: `${marca} ${modelo} ${año}`
- Precio con formato: `$25,000`
- Badge de "Disponible" / "No disponible"
- Botón "Ver detalles"
- Carrusel de imágenes en modal

---

### 3.2 Obtener Auto por ID

**Endpoint**: `GET /api/autos/{id}`  
**Auth**: No requerida  
**Descripción**: Obtener detalles de un auto específico

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 1,
    "marca": "Toyota",
    "modelo": "Corolla",
    "año": 2022,
  "precio": 25000.00,
    "descripcion": "Toyota Corolla 2022 en excelente estado, único dueño",
    "disponible": true,
    "fechaPublicacion": "2024-11-10T08:00:00Z",
    "imagenes": [
      {
        "id": 1,
  "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/abc123.jpg"
    }
]
  },
  "errors": null
}
```

---

### 3.3 Crear Auto

**Endpoint**: `POST /api/autos`  
**Auth**: ? Requerida (Admin)
**Descripción**: Crear un nuevo auto

**Request Body**:
```json
{
  "marca": "Ford",
  "modelo": "Focus",
  "año": 2023,
  "precio": 22000.00,
  "descripcion": "Ford Focus 2023, full equipo",
  "disponible": true
}
```

**Response Success (201)**:
```json
{
  "success": true,
  "message": "Auto creado exitosamente",
  "data": {
    "id": 2,
    "marca": "Ford",
    "modelo": "Focus",
    "año": 2023,
    "precio": 22000.00,
    "descripcion": "Ford Focus 2023, full equipo",
    "disponible": true,
    "fechaPublicacion": "2024-11-19T15:30:00Z",
    "imagenes": []
  },
  "errors": null
}
```

**Validaciones Frontend**:
- Marca: requerida, min 2 caracteres
- Modelo: requerido, min 2 caracteres
- Año: requerido, entre 1900 y año actual + 1
- Precio: requerido, mayor a 0
- Descripción: opcional
- Disponible: requerido, boolean

---

### 3.4 Actualizar Auto

**Endpoint**: `PUT /api/autos/{id}`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Actualizar datos de un auto

**Request Body** (todos los campos opcionales):
```json
{
  "marca": "Ford",
  "modelo": "Focus Titanium",
  "año": 2023,
  "precio": 21000.00,
  "descripcion": "Ford Focus 2023, full equipo, impecable",
  "disponible": false
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Auto actualizado exitosamente",
  "data": {
    "id": 2,
    "marca": "Ford",
    "modelo": "Focus Titanium",
    "año": 2023,
    "precio": 21000.00,
    "descripcion": "Ford Focus 2023, full equipo, impecable",
    "disponible": false,
    "fechaPublicacion": "2024-11-19T15:30:00Z",
    "imagenes": []
  },
  "errors": null
}
```

---

### 3.5 Eliminar Auto

**Endpoint**: `DELETE /api/autos/{id}`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Eliminar un auto y todas sus imágenes

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Auto eliminado exitosamente",
  "data": null,
  "errors": null
}
```

---

### 3.6 Subir Imagen de Auto

**Endpoint**: `POST /api/autos/{id}/imagenes`  
**Auth**: ? Requerida (Admin)  
**Content-Type**: `multipart/form-data`  
**Descripción**: Subir una imagen para un auto

**Request Body (FormData)**:
```javascript
const formData = new FormData();
formData.append('file', imageFile); // File object from input

axios.post(`/api/autos/${autoId}/imagenes`, formData, {
headers: {
    'Content-Type': 'multipart/form-data',
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Imagen subida exitosamente",
  "data": {
    "id": 3,
    "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/xyz789.jpg"
  },
  "errors": null
}
```

**Validaciones Frontend**:
- Tipo de archivo: jpg, jpeg, png, webp
- Tamaño máximo: 5MB
- Preview antes de subir

**Ejemplo React**:
```javascript
const handleImageUpload = async (e, autoId) => {
  const file = e.target.files[0];
  
  // Validar tipo
  if (!['image/jpeg', 'image/jpg', 'image/png', 'image/webp'].includes(file.type)) {
    alert('Formato no permitido');
    return;
  }
  
  // Validar tamaño
  if (file.size > 5 * 1024 * 1024) {
    alert('Archivo muy grande. Máximo 5MB');
    return;
  }
  
  const formData = new FormData();
  formData.append('file', file);
  
  try {
    const response = await axios.post(
      `/api/autos/${autoId}/imagenes`,
      formData,
      {
        headers: {
  'Content-Type': 'multipart/form-data'
   }
      }
    );
  
    // Actualizar lista de imágenes
    setImagenes([...imagenes, response.data.data]);
  } catch (error) {
    alert(error.response.data.message);
  }
};
```

---

### 3.7 Eliminar Imagen de Auto

**Endpoint**: `DELETE /api/autos/{autoId}/imagenes/{imagenId}`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Eliminar una imagen específica de un auto

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Imagen eliminada exitosamente",
  "data": null,
  "errors": null
}
```

---

## 4. ??? MOTOS

**Nota**: Los endpoints de motos son idénticos a los de autos, solo cambia la ruta base de `/api/autos` a `/api/motos`.

### Endpoints Disponibles

- `GET /api/motos` - Listar todas las motos
- `GET /api/motos/{id}` - Obtener moto por ID
- `POST /api/motos` - Crear moto (Admin)
- `PUT /api/motos/{id}` - Actualizar moto (Admin)
- `DELETE /api/motos/{id}` - Eliminar moto (Admin)
- `POST /api/motos/{id}/imagenes` - Subir imagen (Admin)
- `DELETE /api/motos/{motoId}/imagenes/{imagenId}` - Eliminar imagen (Admin)

**Estructura de respuesta**: Idéntica a Autos

---

## 5. ?? TURNOS

### 5.1 Obtener Mis Turnos (Cliente)

**Endpoint**: `GET /api/turnos/mios`  
**Auth**: ? Requerida  
**Descripción**: Obtener lista de turnos del usuario logueado

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
    "id": 1,
      "usuarioId": 1,
      "nombreUsuario": "Juan Pérez",
      "emailUsuario": "juan@example.com",
      "fecha": "2024-11-25T00:00:00Z",
      "hora": "10:00:00",
    "tipoLavado": "Completo",
      "estado": "Pendiente",
      "fechaFinalizacion": null,
      "fechaCreacion": "2024-11-19T14:20:00Z"
    }
  ],
  "errors": null
}
```

**Estados Posibles**:
- `Pendiente` - Turno reservado, pendiente de atención
- `EnProceso` - Turno en proceso de lavado
- `Finalizado` - Turno completado
- `Cancelado` - Turno cancelado

**Componente Frontend**:
```javascript
// Badge de estado con colores
const getEstadoColor = (estado) => {
  switch(estado) {
    case 'Pendiente': return 'yellow';
    case 'EnProceso': return 'blue';
    case 'Finalizado': return 'green';
    case 'Cancelado': return 'red';
  }
};
```

---

### 5.2 Crear Turno

**Endpoint**: `POST /api/turnos`  
**Auth**: ? Requerida  
**Descripción**: Crear un nuevo turno de lavado

**Request Body**:
```json
{
  "fecha": "2024-11-25",
  "hora": "10:00:00",
  "tipoLavado": "Completo"
}
```

**Tipos de Lavado Disponibles**:
- `Básico`
- `Completo`
- `Premium`

**Response Success (201)**:
```json
{
  "success": true,
  "message": "Turno creado exitosamente",
  "data": {
    "id": 2,
    "usuarioId": 1,
    "nombreUsuario": "Juan Pérez",
    "emailUsuario": "juan@example.com",
  "fecha": "2024-11-25T00:00:00Z",
    "hora": "10:00:00",
    "tipoLavado": "Completo",
    "estado": "Pendiente",
    "fechaFinalizacion": null,
    "fechaCreacion": "2024-11-19T15:45:00Z"
  },
  "errors": null
}
```

**Response Error (400)**:
```json
{
  "success": false,
  "message": "El horario seleccionado no está disponible",
  "data": null,
  "errors": null
}
```

**Validaciones Frontend**:
- Fecha: requerida, no puede ser pasada
- Hora: requerida, formato "HH:mm:ss"
- Tipo de Lavado: requerido, uno de los 3 valores válidos
- Verificar disponibilidad antes de enviar

---

### 5.3 Verificar Disponibilidad

**Endpoint**: `GET /api/turnos/disponibilidad?fecha=2024-11-25`  
**Auth**: ? Requerida  
**Descripción**: Consultar horarios disponibles para una fecha

**Query Params**:
- `fecha` (requerido): Formato "YYYY-MM-DD"

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": {
    "fecha": "2024-11-25T00:00:00Z",
    "horariosDisponibles": [
      "09:00",
      "10:00",
      "11:00",
      "14:00",
      "15:00",
      "16:00",
      "17:00",
      "18:00"
    ]
  },
  "errors": null
}
```

**Horario de Atención**: 9:00 AM - 6:00 PM

**Ejemplo Frontend** (Selector de horario):
```javascript
const HorarioSelector = ({ fecha }) => {
  const [disponibles, setDisponibles] = useState([]);
  
  useEffect(() => {
    const fetchDisponibilidad = async () => {
      const response = await axios.get(`/api/turnos/disponibilidad?fecha=${fecha}`);
      setDisponibles(response.data.data.horariosDisponibles);
  };
    
if (fecha) {
      fetchDisponibilidad();
    }
  }, [fecha]);
  
  return (
    <select>
      {disponibles.map(hora => (
  <option key={hora} value={hora}>{hora}</option>
      ))}
    </select>
  );
};
```

---

### 5.4 Listar Todos los Turnos (Admin)

**Endpoint**: `GET /api/turnos`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Obtener lista de todos los turnos

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "usuarioId": 1,
      "nombreUsuario": "Juan Pérez",
    "emailUsuario": "juan@example.com",
      "fecha": "2024-11-25T00:00:00Z",
      "hora": "10:00:00",
  "tipoLavado": "Completo",
      "estado": "Pendiente",
      "fechaFinalizacion": null,
  "fechaCreacion": "2024-11-19T14:20:00Z"
    }
  ],
  "errors": null
}
```

**Componente Frontend Sugerido**:
- Tabla con filtros por estado, fecha
- Ordenar por fecha/hora
- Botones de acción según estado
- Color coding por estado

---

### 5.5 Actualizar Estado de Turno (Admin)

**Endpoint**: `PUT /api/turnos/{id}/estado`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Cambiar el estado de un turno

**Request Body**:
```json
{
  "estado": "EnProceso"
}
```

**Estados Permitidos**:
- `Pendiente`
- `EnProceso`
- `Finalizado` ?? **Al pasar a Finalizado, se envía WhatsApp automáticamente**
- `Cancelado`

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Estado del turno actualizado exitosamente",
  "data": {
    "id": 1,
    "usuarioId": 1,
    "nombreUsuario": "Juan Pérez",
    "emailUsuario": "juan@example.com",
    "fecha": "2024-11-25T00:00:00Z",
    "hora": "10:00:00",
  "tipoLavado": "Completo",
    "estado": "Finalizado",
 "fechaFinalizacion": "2024-11-25T11:30:00Z",
    "fechaCreacion": "2024-11-19T14:20:00Z"
  },
  "errors": null
}
```

**Importante**: Cuando cambias a "Finalizado":
1. Se registra la `fechaFinalizacion`
2. Se envía WhatsApp automáticamente al cliente
3. Se guarda en historial de notificaciones

**Ejemplo Frontend**:
```javascript
const cambiarEstado = async (turnoId, nuevoEstado) => {
  if (nuevoEstado === 'Finalizado') {
    if (!confirm('¿Finalizar turno? Se enviará notificación al cliente')) {
  return;
    }
  }
  
  await axios.put(`/api/turnos/${turnoId}/estado`, {
    estado: nuevoEstado
  });
};
```

---

### 5.6 Cancelar Turno

**Endpoint**: `DELETE /api/turnos/{id}`  
**Auth**: ? Requerida  
**Descripción**: Cancelar un turno (Cliente puede cancelar sus propios turnos)

**Response Success (200)**:
```json
{
  "success": true,
"message": "Turno cancelado exitosamente",
  "data": null,
  "errors": null
}
```

---

## 6. ?? NOTIFICACIONES (ADMIN)

### 6.1 Ver Historial de Notificaciones

**Endpoint**: `GET /api/notificaciones`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Obtener historial de todas las notificaciones enviadas

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "usuarioId": 1,
      "usuarioNombre": "Juan Pérez",
 "usuarioEmail": "juan@example.com",
      "mensaje": "Su turno de lavado ha finalizado",
  "tipo": "WhatsApp",
      "fechaEnvio": "2024-11-25T11:30:00Z",
      "enviada": true
    }
  ],
  "errors": null
}
```

**Tipos de Notificación**:
- `WhatsApp`
- `Email`
- `SMS`

---

### 6.2 Enviar WhatsApp Manual

**Endpoint**: `POST /api/notificaciones/whatsapp`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Enviar mensaje de WhatsApp manual a un usuario

**Request Body**:
```json
{
  "usuarioId": 1,
  "mensaje": "Tenemos una promoción especial para ti. Visitanos!"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Mensaje enviado exitosamente",
  "data": {
    "id": 2,
    "usuarioId": 1,
    "usuarioNombre": "Juan Pérez",
 "usuarioTelefono": "+54 9 11 1234-5678",
    "mensaje": "Tenemos una promoción especial para ti. Visitanos!",
    "tipo": "WhatsApp",
    "fechaEnvio": "2024-11-19T16:00:00Z",
    "enviada": true
  },
  "errors": null
}
```

**Nota**: El usuario debe tener teléfono registrado

---

## 7. ?? DASHBOARD (ADMIN)

### 7.1 Obtener Resumen de Estadísticas

**Endpoint**: `GET /api/dashboard/resumen`  
**Auth**: ? Requerida (Admin)  
**Descripción**: Obtener estadísticas generales del sistema

**Response Success (200)**:
```json
{
  "success": true,
  "message": null,
  "data": {
    "autosPublicados": 15,
    "motosPublicadas": 8,
    "turnosDelDia": 5,
    "usuariosRegistrados": 42,
    "turnosPendientes": 3,
    "turnosEnProceso": 2
  },
  "errors": null
}
```

**Componente Frontend Sugerido**:
- Cards con íconos para cada métrica
- Números grandes y destacados
- Actualización automática cada X minutos
- Gráficos opcionales

**Ejemplo React**:
```javascript
const Dashboard = () => {
  const [stats, setStats] = useState(null);
  
  useEffect(() => {
    const fetchStats = async () => {
      const response = await axios.get('/api/dashboard/resumen');
      setStats(response.data.data);
    };
    
    fetchStats();
    
    // Actualizar cada 5 minutos
    const interval = setInterval(fetchStats, 5 * 60 * 1000);
    return () => clearInterval(interval);
  }, []);
  
  return (
    <div className="grid grid-cols-3 gap-4">
  <StatCard 
   title="Autos Publicados" 
 value={stats?.autosPublicados}
        icon={<CarIcon />}
      />
      <StatCard 
        title="Motos Publicadas" 
        value={stats?.motosPublicadas}
     icon={<BikeIcon />}
      />
      <StatCard 
title="Turnos del Día" 
  value={stats?.turnosDelDia}
icon={<CalendarIcon />}
      />
    </div>
  );
};
```

---

## 8. ?? SEED (SOLO DESARROLLO)

**?? IMPORTANTE**: Estos endpoints NO deben estar en producción. Eliminar `SeedController.cs` antes de deploy.

### 8.1 Crear Primer Admin

**Endpoint**: `POST /api/seed/create-first-admin`  
**Auth**: No requerida  
**Descripción**: Crear el primer usuario administrador

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuario administrador creado exitosamente",
  "data": {
    "email": "admin@feller.com",
    "password": "Admin123!",
    "instrucciones": "Usa estas credenciales para hacer login. CAMBIA LA CONTRASEÑA inmediatamente."
  }
}
```

**Nota**: Solo funciona si NO hay ningún admin en el sistema.

---

### 8.2 Crear Datos de Prueba

**Endpoint**: `POST /api/seed/seed-test-data`  
**Auth**: No requerida  
**Descripción**: Crear datos de prueba (usuarios, autos, motos)

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Datos de prueba creados exitosamente",
  "data": {
    "admin": {
      "email": "admin@feller.com",
      "password": "Admin123!"
  },
    "cliente": {
      "email": "juan@test.com",
      "password": "Password123!"
    },
    "autos": 2,
    "motos": 2
  }
}
```

---

### 8.3 Eliminar Todos los Datos

**Endpoint**: `DELETE /api/seed/delete-all-data?confirmacion=CONFIRMO_ELIMINAR_TODO`  
**Auth**: No requerida  
**Descripción**: Eliminar TODOS los datos de la base de datos

**Query Params**:
- `confirmacion` (requerido): Debe ser exactamente "CONFIRMO_ELIMINAR_TODO"

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Todos los datos han sido eliminados"
}
```

**?? PELIGRO**: Esta acción es irreversible.

---

## ?? CÓDIGOS DE ESTADO HTTP

| Código | Significado | Cuándo se Usa |
|--------|-------------|---------------|
| 200 | OK | Operación exitosa |
| 201 | Created | Recurso creado (POST exitoso) |
| 400 | Bad Request | Datos inválidos o faltantes |
| 401 | Unauthorized | Token inválido o ausente |
| 403 | Forbidden | Sin permisos (no es Admin) |
| 404 | Not Found | Recurso no encontrado |
| 500 | Internal Server Error | Error del servidor |

---

## ?? MANEJO DE AUTENTICACIÓN

### Setup Inicial (React/Angular/Vue)

```javascript
import axios from 'axios';

// Configurar base URL
axios.defaults.baseURL = 'http://localhost:5000/api';

// Interceptor para agregar token automáticamente
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor para manejar errores de autenticación
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
 // Token inválido o expirado
      localStorage.removeItem('token');
    window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
```

### Guardar Token Después de Login

```javascript
const handleLogin = async (email, password) => {
  try {
    const response = await axios.post('/auth/login', {
    email,
      password
    });
    
    const { token, rol, nombre } = response.data.data;
    
    // Guardar en localStorage
    localStorage.setItem('token', token);
    localStorage.setItem('userRole', rol);
    localStorage.setItem('userName', nombre);
    
    // Redirigir según rol
    if (rol === 'Admin') {
      navigate('/admin/dashboard');
    } else {
    navigate('/cliente/mis-turnos');
    }
  } catch (error) {
    alert(error.response.data.message);
  }
};
```

### Verificar Rol (Rutas Protegidas)

```javascript
const ProtectedRoute = ({ children, requireAdmin }) => {
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('userRole');
  
  if (!token) {
    return <Navigate to="/login" />;
  }
  
  if (requireAdmin && userRole !== 'Admin') {
    return <Navigate to="/unauthorized" />;
  }
  
  return children;
};

// Uso
<Route 
  path="/admin/usuarios" 
  element={
    <ProtectedRoute requireAdmin>
      <UsuariosPage />
    </ProtectedRoute>
  } 
/>
```

---

## ?? ESTRUCTURA RECOMENDADA FRONTEND

### Páginas Públicas
- `/` - Home con autos y motos destacados
- `/autos` - Listado de autos
- `/autos/:id` - Detalle de auto
- `/motos` - Listado de motos
- `/motos/:id` - Detalle de moto
- `/login` - Login
- `/register` - Registro

### Páginas Cliente (Autenticado)
- `/cliente/mis-turnos` - Ver mis turnos
- `/cliente/nuevo-turno` - Crear turno
- `/cliente/perfil` - Ver/editar perfil

### Páginas Admin (Rol Admin)
- `/admin/dashboard` - Dashboard con estadísticas
- `/admin/usuarios` - Gestión de usuarios
- `/admin/autos` - Gestión de autos
- `/admin/motos` - Gestión de motos
- `/admin/turnos` - Ver y gestionar todos los turnos
- `/admin/notificaciones` - Historial y envío manual

---

## ??? UTILIDADES FRONTEND

### Formateo de Fechas

```javascript
const formatFecha = (fechaISO) => {
  return new Date(fechaISO).toLocaleDateString('es-AR', {
    day: '2-digit',
 month: '2-digit',
    year: 'numeric'
  });
};

// Uso: 25/11/2024
```

### Formateo de Precios

```javascript
const formatPrecio = (precio) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
    minimumFractionDigits: 0
  }).format(precio);
};

// Uso: $25.000
```

### Formateo de Hora

```javascript
const formatHora = (horaString) => {
  return horaString.substring(0, 5); // "10:00:00" -> "10:00"
};
```

### Badge de Estado

```javascript
const EstadoBadge = ({ estado }) => {
  const colors = {
    'Pendiente': 'yellow',
    'EnProceso': 'blue',
    'Finalizado': 'green',
    'Cancelado': 'red'
  };
  
  return (
    <span className={`badge badge-${colors[estado]}`}>
    {estado}
    </span>
  );
};
```

---

## ?? MANEJO DE ERRORES

### Estructura de Error

Todos los errores siguen esta estructura:

```json
{
  "success": false,
  "message": "Descripción del error",
  "data": null,
  "errors": ["Error detallado 1", "Error detallado 2"]
}
```

### Ejemplo de Manejo

```javascript
try {
  const response = await axios.post('/api/autos', autoData);
  alert('Auto creado exitosamente');
} catch (error) {
  if (error.response) {
    // Error del servidor
    const { message, errors } = error.response.data;
    
    if (errors && errors.length > 0) {
  alert(errors.join('\n'));
    } else {
      alert(message);
    }
  } else {
    // Error de red
    alert('Error de conexión. Verifica tu internet.');
  }
}
```

---

## ?? EJEMPLOS DE INTEGRACIÓN

### Listado de Autos (React)

```javascript
const AutosPage = () => {
  const [autos, setAutos] = useState([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    const fetchAutos = async () => {
      try {
        const response = await axios.get('/autos');
        setAutos(response.data.data);
      } catch (error) {
        console.error(error);
      } finally {
    setLoading(false);
      }
    };
    
    fetchAutos();
  }, []);
  
  if (loading) return <Spinner />;
  
  return (
    <div className="grid grid-cols-3 gap-4">
      {autos.map(auto => (
        <AutoCard key={auto.id} auto={auto} />
      ))}
    </div>
  );
};

const AutoCard = ({ auto }) => {
  const imagenPrincipal = auto.imagenes[0]?.url || '/placeholder.jpg';
  
  return (
    <div className="card">
      <img src={imagenPrincipal} alt={`${auto.marca} ${auto.modelo}`} />
      <h3>{auto.marca} {auto.modelo} {auto.año}</h3>
      <p className="price">{formatPrecio(auto.precio)}</p>
      <p className="description">{auto.descripcion}</p>
   <button onClick={() => navigate(`/autos/${auto.id}`)}>
        Ver Detalles
      </button>
    </div>
  );
};
```

### Crear Turno con Selección de Disponibilidad

```javascript
const NuevoTurnoForm = () => {
  const [fecha, setFecha] = useState('');
  const [hora, setHora] = useState('');
  const [tipoLavado, setTipoLavado] = useState('');
  const [horariosDisponibles, setHorariosDisponibles] = useState([]);
  
  useEffect(() => {
    if (fecha) {
      const fetchDisponibilidad = async () => {
     const response = await axios.get(`/turnos/disponibilidad?fecha=${fecha}`);
    setHorariosDisponibles(response.data.data.horariosDisponibles);
      };
    
      fetchDisponibilidad();
    }
  }, [fecha]);
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      await axios.post('/turnos', {
        fecha,
      hora: `${hora}:00`,
        tipoLavado
      });
      
      alert('Turno creado exitosamente');
  navigate('/cliente/mis-turnos');
    } catch (error) {
      alert(error.response.data.message);
    }
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input 
   type="date" 
 value={fecha} 
        onChange={(e) => setFecha(e.target.value)}
  min={new Date().toISOString().split('T')[0]}
        required
      />
      
      <select 
 value={hora} 
        onChange={(e) => setHora(e.target.value)}
        required
        disabled={!fecha}
      >
        <option value="">Seleccionar horario</option>
        {horariosDisponibles.map(h => (
        <option key={h} value={h}>{h}</option>
        ))}
      </select>
      
   <select 
        value={tipoLavado} 
   onChange={(e) => setTipoLavado(e.target.value)}
    required
      >
 <option value="">Seleccionar tipo</option>
        <option value="Básico">Básico</option>
        <option value="Completo">Completo</option>
 <option value="Premium">Premium</option>
      </select>
      
      <button type="submit">Reservar Turno</button>
    </form>
  );
};
```

### Gestión de Turnos (Admin)

```javascript
const AdminTurnosPage = () => {
  const [turnos, setTurnos] = useState([]);
  
  useEffect(() => {
    fetchTurnos();
  }, []);
  
  const fetchTurnos = async () => {
  const response = await axios.get('/turnos');
  setTurnos(response.data.data);
  };
  
  const cambiarEstado = async (turnoId, nuevoEstado) => {
    if (nuevoEstado === 'Finalizado') {
      if (!confirm('¿Finalizar turno? Se enviará WhatsApp al cliente')) {
        return;
      }
    }
  
    try {
      await axios.put(`/turnos/${turnoId}/estado`, {
        estado: nuevoEstado
      });
      
      alert('Estado actualizado');
      fetchTurnos(); // Recargar
    } catch (error) {
      alert(error.response.data.message);
    }
  };
  
  return (
    <table>
      <thead>
        <tr>
       <th>Cliente</th>
          <th>Fecha</th>
          <th>Hora</th>
          <th>Tipo</th>
          <th>Estado</th>
          <th>Acciones</th>
        </tr>
    </thead>
      <tbody>
        {turnos.map(turno => (
          <tr key={turno.id}>
         <td>{turno.nombreUsuario}</td>
  <td>{formatFecha(turno.fecha)}</td>
          <td>{formatHora(turno.hora)}</td>
          <td>{turno.tipoLavado}</td>
      <td><EstadoBadge estado={turno.estado} /></td>
      <td>
           {turno.estado === 'Pendiente' && (
          <button onClick={() => cambiarEstado(turno.id, 'EnProceso')}>
    Iniciar
          </button>
   )}
         {turno.estado === 'EnProceso' && (
        <button onClick={() => cambiarEstado(turno.id, 'Finalizado')}>
         Finalizar
     </button>
     )}
        </td>
          </tr>
      ))}
      </tbody>
    </table>
  );
};
```

---

## ? CHECKLIST DE INTEGRACIÓN

### Setup Inicial
- [ ] Configurar axios con baseURL
- [ ] Implementar interceptors para token
- [ ] Configurar manejo de errores global
- [ ] Crear utilidades de formato (fecha, precio, hora)

### Páginas Públicas
- [ ] Home / Landing
- [ ] Listado de autos
- [ ] Detalle de auto
- [ ] Listado de motos
- [ ] Detalle de moto
- [ ] Login
- [ ] Registro

### Área Cliente
- [ ] Mis turnos
- [ ] Crear nuevo turno (con disponibilidad)
- [ ] Ver perfil
- [ ] Editar perfil

### Área Admin
- [ ] Dashboard con estadísticas
- [ ] Gestión de usuarios (tabla + CRUD)
- [ ] Gestión de autos (tabla + CRUD + upload imágenes)
- [ ] Gestión de motos (tabla + CRUD + upload imágenes)
- [ ] Gestión de turnos (tabla + cambio de estado)
- [ ] Historial de notificaciones
- [ ] Envío manual de WhatsApp

### Seguridad
- [ ] Rutas protegidas por autenticación
- [ ] Rutas protegidas por rol (Admin)
- [ ] Logout funcional
- [ ] Redirección a login si token expira

### UX
- [ ] Loading states
- [ ] Error messages
- [ ] Success messages
- [ ] Confirmaciones antes de eliminar
- [ ] Validaciones de formularios
- [ ] Preview de imágenes antes de subir

---

## ?? RESUMEN

**Base URL**: `http://localhost:5000/api`

**Endpoints Totales**: 31

**Endpoints Públicos**: 6
- GET /autos
- GET /autos/{id}
- GET /motos
- GET /motos/{id}
- POST /auth/register
- POST /auth/login

**Endpoints Autenticados**: 25

**Endpoints Solo Admin**: 17

**Autenticación**: JWT Bearer Token

**Formato de Respuesta**: Estandarizado con `ResponseWrapper`

```json
{
  "success": true/false,
  "message": "Mensaje descriptivo",
  "data": { /* datos */ },
  "errors": [ /* lista de errores */ ]
}
```

---

## ?? CONTACTO Y SOPORTE

Para más información sobre la implementación del backend, consulta los siguientes archivos:

- `README.md` - Guía general del proyecto
- `ENDPOINTS.md` - Documentación detallada de endpoints
- `AWS_S3_CONFIGURACION.md` - Configuración de AWS S3
- `GUIA_ADMINISTRADORES.md` - Gestión de usuarios admin
- `IMPLEMENTACION_COMPLETA.md` - Detalles de implementación

---

**Versión**: 1.0.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Producción Ready
