# ?? Prompt de Actualización - Sistema de Vehículos con Destacados

## ?? Resumen de Cambios Implementados

Se han realizado dos actualizaciones mayores en el sistema de gestión de vehículos de Feller Automotores:

1. **Propiedades de Concesionaria** - Información detallada de vehículos (usados/0km)
2. **Sistema de Destacados** - Gestión de vehículos destacados para la home

---

## ?? ACTUALIZACIÓN 1: Propiedades de Concesionaria

### Nuevos Atributos en Vehículos

#### VehiculoBase (Común para Autos y Motos)
```csharp
public string Estado { get; set; } = "Usado"; // "Usado" o "0km"
public bool EsDestacado { get; set; } = false;
public int? OrdenDestacado { get; set; }
```

#### Auto (Propiedades Específicas)
```csharp
public int Puertas { get; set; } = 4;   // 2-5 puertas
public string TipoCombustible { get; set; } = "Nafta";         // Nafta, Gasoil, GNC, Híbrido, Eléctrico
public string? Transmision { get; set; }              // Manual, Automática
public int? Kilometraje { get; set; }       // null para 0km
```

#### Moto (Propiedades Específicas)
```csharp
public int Cilindrada { get; set; }           // 50-2000 cc
public string? TipoMoto { get; set; } // Deportiva, Cruiser, Touring, Naked, Enduro
public int? Kilometraje { get; set; }               // null para 0km
```

### Endpoints Actualizados

#### POST /api/autos (Crear Auto)
```json
{
  "marca": "Toyota",
  "modelo": "Corolla",
  "año": 2024,
  "precio": 35000,
  "descripcion": "Toyota Corolla 2024, 0km",
  "disponible": true,
  
  // NUEVOS CAMPOS
  "estado": "0km",          // REQUERIDO: "Usado" o "0km"
  "puertas": 4,      // REQUERIDO: 2-5
  "tipoCombustible": "Nafta",       // REQUERIDO: Nafta, Gasoil, GNC, Híbrido, Eléctrico
  "transmision": "Automática",      // OPCIONAL: Manual, Automática
  "kilometraje": null,    // OPCIONAL: null para 0km, número para usados
  
  // DESTACADOS (Opcionales al crear)
  "esDestacado": false,  // OPCIONAL: Por defecto false
  "ordenDestacado": null      // OPCIONAL: Orden de aparición (1, 2, 3...)
}
```

#### POST /api/motos (Crear Moto)
```json
{
  "marca": "Honda",
  "modelo": "CB 500X",
  "año": 2024,
  "precio": 10000,
  "descripcion": "Honda CB 500X 2024, 0km",
  "disponible": true,
  
  // NUEVOS CAMPOS
  "estado": "0km",  // REQUERIDO: "Usado" o "0km"
  "cilindrada": 500,        // REQUERIDO: 50-2000
  "tipoMoto": "Touring",       // OPCIONAL: Deportiva, Cruiser, Touring, Naked, Enduro
  "kilometraje": null,                // OPCIONAL: null para 0km
  
  // DESTACADOS (Opcionales al crear)
  "esDestacado": false,
  "ordenDestacado": null
}
```

#### PUT /api/autos/{id} (Actualizar Auto)
```json
{
  // Todos los campos son opcionales, incluyendo los nuevos
  "estado": "Usado",
  "puertas": 5,
  "tipoCombustible": "Gasoil",
  "transmision": "Manual",
  "kilometraje": 45000,
  "esDestacado": true,
  "ordenDestacado": 1
}
```

#### GET /api/autos (Response Actualizado)
```json
{
  "success": true,
  "data": [
  {
      "id": 1,
  "marca": "Toyota",
      "modelo": "Corolla",
  "año": 2024,
      "precio": 35000,
      "descripcion": "...",
  "disponible": true,
      "fechaPublicacion": "2024-11-19T10:00:00Z",
      
  // NUEVAS PROPIEDADES EN RESPONSE
      "estado": "0km",
      "puertas": 4,
      "tipoCombustible": "Nafta",
      "transmision": "Automática",
      "kilometraje": null,
      "esDestacado": true,
      "ordenDestacado": 1,
  
      "imagenes": [...]
    }
  ]
}
```

### Validaciones Implementadas

#### Autos
- **Estado**: Solo "Usado" o "0km" (requerido)
- **Puertas**: Entre 2 y 5 (requerido)
- **TipoCombustible**: Nafta, Gasoil, GNC, Híbrido, Eléctrico (requerido)
- **Transmision**: Manual, Automática (opcional)
- **Kilometraje**: null para 0km, número positivo para usados (opcional)

#### Motos
- **Estado**: Solo "Usado" o "0km" (requerido)
- **Cilindrada**: Entre 50 y 2000 cc (requerido)
- **TipoMoto**: Deportiva, Cruiser, Touring, Naked, Enduro (opcional)
- **Kilometraje**: null para 0km, número positivo para usadas (opcional)

---

## ?? ACTUALIZACIÓN 2: Sistema de Destacados

### Nuevo Controlador: DestacadosController

#### Endpoints Públicos (Sin Autenticación)

##### 1. GET /api/destacados
Obtener todos los vehículos destacados (autos y motos mezclados)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "tipo": "Auto",
      "vehiculo": {
        "id": 1,
"marca": "Toyota",
  "modelo": "Corolla",
        "precio": 35000,
        "estado": "0km",
        "esDestacado": true,
        "ordenDestacado": 1,
        "imagenes": [...]
      }
    },
    {
      "tipo": "Moto",
"vehiculo": {
        "id": 2,
        "marca": "Honda",
      "modelo": "CB 500X",
        "precio": 10000,
        "estado": "0km",
        "esDestacado": true,
     "ordenDestacado": 2,
     "imagenes": [...]
      }
    }
  ]
}
```

**Orden**: Por `ordenDestacado` ascendente (1, 2, 3...), luego por fecha de publicación

##### 2. GET /api/destacados/autos
Obtener solo autos destacados

**Response:** Array de `AutoDto` con `esDestacado: true`

##### 3. GET /api/destacados/motos
Obtener solo motos destacadas

**Response:** Array de `MotoDto` con `esDestacado: true`

---

#### Endpoints de Administración (Requieren Token Admin)

##### 4. POST /api/destacados/autos/{id}?orden={numero}
Marcar un auto como destacado

**Headers:**
```
Authorization: Bearer {admin_token}
```

**Query Params:**
- `orden` (opcional): Número del orden (1, 2, 3...)
  - Si no se proporciona, se auto-asigna el siguiente número disponible

**Response:**
```json
{
  "success": true,
  "message": "Auto marcado como destacado con orden 3",
  "data": {
  "id": 1,
    "marca": "Toyota",
    "modelo": "Corolla",
    "esDestacado": true,
    "ordenDestacado": 3,
    ...
  }
}
```

**Ejemplo de uso:**
```javascript
// Con orden específico
POST /api/destacados/autos/1?orden=1

// Sin orden (auto-asigna el siguiente)
POST /api/destacados/autos/1
```

##### 5. POST /api/destacados/motos/{id}?orden={numero}
Marcar una moto como destacada

Funciona igual que autos.

##### 6. DELETE /api/destacados/autos/{id}
Desmarcar un auto como destacado

**Headers:**
```
Authorization: Bearer {admin_token}
```

**Response:**
```json
{
  "success": true,
  "message": "Auto desmarcado como destacado",
  "data": {
    "id": 1,
    "esDestacado": false,
    "ordenDestacado": null,
    ...
  }
}
```

##### 7. DELETE /api/destacados/motos/{id}
Desmarcar una moto como destacada

Funciona igual que autos.

---

## ?? Configuración de Base de Datos

### Migraciones Requeridas

```bash
# 1. Crear migración para propiedades de concesionaria
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef migrations add AgregarPropiedadesConcesionaria
dotnet ef database update

# 2. Crear migración para destacados
dotnet ef migrations add AgregarVehiculosDestacados
dotnet ef database update
```

### Columnas Agregadas en BD

**Tabla Vehiculos:**
- `Estado` (text, not null, default 'Usado')
- `EsDestacado` (boolean, default false)
- `OrdenDestacado` (integer, nullable)
- `Puertas` (integer, default 4 para autos)
- `TipoCombustible` (text, default 'Nafta' para autos)
- `Transmision` (text, nullable)
- `Kilometraje` (integer, nullable)
- `Cilindrada` (integer, default 150 para motos)
- `TipoMoto` (text, nullable)

**Índices:**
- `IX_Vehiculos_EsDestacado_OrdenDestacado` (Para búsquedas rápidas de destacados)

---

## ?? Ejemplos de Integración Frontend

### React - Obtener Destacados para Home

```javascript
import { useState, useEffect } from 'react';
import axios from 'axios';

const Home = () => {
  const [destacados, setDestacados] = useState([]);
  
  useEffect(() => {
    const fetchDestacados = async () => {
      const response = await axios.get('http://localhost:5000/api/destacados');
      setDestacados(response.data.data);
    };
    fetchDestacados();
  }, []);

  return (
    <section className="destacados">
      <h2>Vehículos Destacados</h2>
    <div className="grid">
        {destacados.map((item, index) => (
    <VehiculoCard 
     key={index}
    tipo={item.tipo}
       vehiculo={item.vehiculo}
   />
    ))}
      </div>
    </section>
  );
};
```

### React - Admin: Marcar como Destacado

```javascript
const marcarDestacado = async (vehiculoId, tipo, orden = null) => {
  const token = localStorage.getItem('token');
  const url = `/api/destacados/${tipo.toLowerCase()}s/${vehiculoId}`;
  const params = orden ? `?orden=${orden}` : '';
  
  try {
    const response = await axios.post(
      url + params,
      {},
      { headers: { Authorization: `Bearer ${token}` }}
    );
alert(response.data.message);
  } catch (error) {
    alert('Error: ' + error.response?.data?.message);
  }
};

// Uso
<button onClick={() => marcarDestacado(1, 'Auto', 1)}>
  Marcar como Destacado (Orden 1)
</button>
```

### React - Admin: Desmarcar Destacado

```javascript
const desmarcarDestacado = async (vehiculoId, tipo) => {
  const token = localStorage.getItem('token');
  
  if (!confirm('¿Desmarcar como destacado?')) return;
  
  try {
    await axios.delete(
      `/api/destacados/${tipo.toLowerCase()}s/${vehiculoId}`,
      { headers: { Authorization: `Bearer ${token}` }}
    );
    alert('Vehículo desmarcado');
  } catch (error) {
    alert('Error: ' + error.response?.data?.message);
  }
};
```

---

## ?? Casos de Uso

### 1. Crear Auto 0km Destacado
```json
POST /api/autos
Authorization: Bearer {admin_token}

{
  "marca": "Toyota",
  "modelo": "Corolla XEI",
"año": 2024,
  "precio": 35000,
  "estado": "0km",
  "puertas": 4,
  "tipoCombustible": "Nafta",
  "transmision": "Automática",
  "kilometraje": null,
  "descripcion": "Toyota Corolla XEI 2024, 0km, full equipo",
  "disponible": true,
  "esDestacado": true,
  "ordenDestacado": 1
}
```

### 2. Crear Auto Usado
```json
POST /api/autos
Authorization: Bearer {admin_token}

{
  "marca": "Ford",
  "modelo": "Focus",
  "año": 2020,
  "precio": 18000,
  "estado": "Usado",
  "puertas": 5,
  "tipoCombustible": "Gasoil",
  "transmision": "Manual",
  "kilometraje": 45000,
  "descripcion": "Ford Focus 2020, excelente estado",
  "disponible": true
}
```

### 3. Crear Moto 0km Destacada
```json
POST /api/motos
Authorization: Bearer {admin_token}

{
  "marca": "Honda",
  "modelo": "CB 500X",
  "año": 2024,
  "precio": 10000,
  "estado": "0km",
  "cilindrada": 500,
  "tipoMoto": "Touring",
  "kilometraje": null,
  "descripcion": "Honda CB 500X 2024, 0km",
  "disponible": true,
"esDestacado": true,
  "ordenDestacado": 2
}
```

### 4. Gestionar Destacados

```bash
# Marcar auto existente como destacado
POST /api/destacados/autos/5?orden=3
Authorization: Bearer {admin_token}

# Obtener todos los destacados (público)
GET /api/destacados

# Desmarcar auto destacado
DELETE /api/destacados/autos/5
Authorization: Bearer {admin_token}
```

---

## ?? Lógica de Ordenamiento de Destacados

1. **Por `ordenDestacado`** (ascendente): 1, 2, 3...
2. **Sin orden (`null`)**: Van al final
3. **Entre vehículos sin orden**: Los más recientes primero (por `fechaPublicacion`)

**Ejemplo:**
- Auto 1: `ordenDestacado: 1` ? Aparece primero
- Moto 1: `ordenDestacado: 2` ? Aparece segundo
- Auto 2: `ordenDestacado: 3` ? Aparece tercero
- Auto 3: `ordenDestacado: null` ? Aparece al final (más reciente)
- Moto 2: `ordenDestacado: null` ? Aparece al final (más antiguo)

---

## ? Checklist de Implementación Frontend

### Endpoints a Integrar

#### Públicos
- [ ] `GET /api/autos` - Incluye nuevos campos en response
- [ ] `GET /api/motos` - Incluye nuevos campos en response
- [ ] `GET /api/destacados` - Para home/landing
- [ ] `GET /api/destacados/autos` - Filtrado por tipo
- [ ] `GET /api/destacados/motos` - Filtrado por tipo

#### Admin
- [ ] `POST /api/autos` - Actualizar form con nuevos campos
- [ ] `PUT /api/autos/{id}` - Actualizar form con nuevos campos
- [ ] `POST /api/motos` - Actualizar form con nuevos campos
- [ ] `PUT /api/motos/{id}` - Actualizar form con nuevos campos
- [ ] `POST /api/destacados/autos/{id}` - Gestión de destacados
- [ ] `POST /api/destacados/motos/{id}` - Gestión de destacados
- [ ] `DELETE /api/destacados/autos/{id}` - Gestión de destacados
- [ ] `DELETE /api/destacados/motos/{id}` - Gestión de destacados

### Componentes a Crear/Actualizar

- [ ] **Home/Landing**: Sección de vehículos destacados
- [ ] **Card de Vehículo**: Mostrar badge "0km"/"Usado" y specs
- [ ] **Detalle de Vehículo**: Grid completo de especificaciones
- [ ] **Formulario Admin (Autos)**: Campos nuevos con validaciones
- [ ] **Formulario Admin (Motos)**: Campos nuevos con validaciones
- [ ] **Panel de Destacados**: Gestionar destacados (marcar/desmarcar/orden)

### Validaciones Frontend

#### Autos
```javascript
const validateAuto = (data) => {
  const errors = [];
  
  // Estado
  if (!['Usado', '0km'].includes(data.estado)) {
    errors.push('Estado debe ser "Usado" o "0km"');
  }
  
  // Puertas
  if (data.puertas < 2 || data.puertas > 5) {
    errors.push('Puertas debe estar entre 2 y 5');
  }
  
  // Combustible
  const combustiblesValidos = ['Nafta', 'Gasoil', 'GNC', 'Híbrido', 'Eléctrico'];
  if (!combustiblesValidos.includes(data.tipoCombustible)) {
    errors.push('Tipo de combustible inválido');
  }
  
  // Kilometraje según estado
  if (data.estado === '0km' && data.kilometraje !== null) {
    errors.push('Un vehículo 0km no puede tener kilometraje');
  }
  
  return errors;
};
```

#### Motos
```javascript
const validateMoto = (data) => {
  const errors = [];
  
  // Estado
  if (!['Usado', '0km'].includes(data.estado)) {
    errors.push('Estado debe ser "Usado" o "0km"');
  }
  
  // Cilindrada
  if (data.cilindrada < 50 || data.cilindrada > 2000) {
    errors.push('Cilindrada debe estar entre 50 y 2000 cc');
  }
  
  // Kilometraje según estado
  if (data.estado === '0km' && data.kilometraje !== null) {
  errors.push('Una moto 0km no puede tener kilometraje');
  }
  
  return errors;
};
```

---

## ?? Documentación Completa

**Archivos de referencia en el backend:**
- `VEHICULOS_DESTACADOS.md` - Documentación completa de destacados
- `DESTACADOS_RESUMEN.md` - Resumen ejecutivo
- `INSTRUCCIONES_DESTACADOS.md` - Guía paso a paso
- `API_FRONTEND_GUIDE_ACTUALIZACION.md` - Guía completa para frontend
- `ACTUALIZACION_CONCESIONARIA_COMPLETA.md` - Resumen de propiedades

---

## ?? Resumen

### Nuevos Campos Requeridos al Crear Vehículos

**Autos:**
- `estado` (string): "Usado" o "0km"
- `puertas` (int): 2-5
- `tipoCombustible` (string): Nafta, Gasoil, GNC, Híbrido, Eléctrico

**Motos:**
- `estado` (string): "Usado" o "0km"
- `cilindrada` (int): 50-2000

### Nuevos Endpoints Destacados

- `GET /api/destacados` - Público
- `GET /api/destacados/autos` - Público
- `GET /api/destacados/motos` - Público
- `POST /api/destacados/{tipo}s/{id}?orden={num}` - Admin
- `DELETE /api/destacados/{tipo}s/{id}` - Admin

### Features Implementadas

? Diferenciación entre 0km y Usados  
? Especificaciones técnicas completas  
? Sistema de destacados con orden  
? Auto-asignación de orden  
? Consultas optimizadas con índices  
? API RESTful completa  
? Validaciones de backend  

---

**Versión API**: 1.3.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Completado - Listo para integrar
