# ?? ACTUALIZACIÓN API - Propiedades de Concesionaria

## ?? Nuevas Propiedades Agregadas

### VehiculoBase (Autos y Motos)
| Propiedad | Tipo | Descripción | Valores |
|-----------|------|-------------|---------|
| `estado` | string | Condición del vehículo | `"Usado"` o `"0km"` |

### Autos (Específicas)
| Propiedad | Tipo | Descripción | Valores | Requerido |
|-----------|------|-------------|---------|-----------|
| `puertas` | int | Cantidad de puertas | 2-5 | ? |
| `tipoCombustible` | string | Tipo de combustible | `"Nafta"`, `"Gasoil"`, `"GNC"`, `"Híbrido"`, `"Eléctrico"` | ? |
| `transmision` | string | Tipo de transmisión | `"Manual"`, `"Automática"` | ? |
| `kilometraje` | int | Kilómetros recorridos | Número o null | ? |

### Motos (Específicas)
| Propiedad | Tipo | Descripción | Valores | Requerido |
|-----------|------|-------------|---------|-----------|
| `cilindrada` | int | Cilindrada en CC | 50-2000 | ? |
| `tipoMoto` | string | Tipo de moto | `"Deportiva"`, `"Cruiser"`, `"Touring"`, `"Naked"`, `"Enduro"` | ? |
| `kilometraje` | int | Kilómetros recorridos | Número o null | ? |

---

## ?? CAMBIOS EN ENDPOINTS

### 3.1 GET /api/autos - Listar Todos los Autos

**Response Success (200)** - ACTUALIZADO:
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
      "estado": "Usado",
      "fechaPublicacion": "2024-11-10T08:00:00Z",
      
      // NUEVAS PROPIEDADES
      "puertas": 4,
      "tipoCombustible": "Nafta",
      "transmision": "Manual",
      "kilometraje": 45000,
      
  "imagenes": [
        {
 "id": 1,
          "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/abc123.jpg"
        }
    ]
    }
  ],
  "errors": null
}
```

---

### 3.3 POST /api/autos - Crear Auto

**Request Body** - ACTUALIZADO:
```json
{
  "marca": "Ford",
  "modelo": "Focus",
  "año": 2023,
  "precio": 22000.00,
  "descripcion": "Ford Focus 2023, full equipo",
  "disponible": true,
  
  // NUEVAS PROPIEDADES REQUERIDAS
  "estado": "0km",
  "puertas": 5,
  "tipoCombustible": "Gasoil",
  
  // NUEVAS PROPIEDADES OPCIONALES
  "transmision": "Automática",
  "kilometraje": null
}
```

**Validaciones Frontend**:
- `estado`: requerido, debe ser "Usado" o "0km"
- `puertas`: requerido, debe estar entre 2 y 5
- `tipoCombustible`: requerido, valores: Nafta, Gasoil, GNC, Híbrido, Eléctrico
- `transmision`: opcional, valores: Manual, Automática
- `kilometraje`: opcional, null para 0km, número positivo para usados

---

### 3.4 PUT /api/autos/{id} - Actualizar Auto

**Request Body** (todos opcionales) - ACTUALIZADO:
```json
{
  "marca": "Ford",
  "modelo": "Focus Titanium",
  "año": 2023,
  "precio": 21000.00,
  "descripcion": "Ford Focus 2023, full equipo, impecable",
  "disponible": false,
  
  // NUEVAS PROPIEDADES OPCIONALES
  "estado": "Usado",
  "puertas": 5,
  "tipoCombustible": "Gasoil",
  "transmision": "Automática",
  "kilometraje": 5000
}
```

---

### 4. MOTOS - Endpoints Actualizados

### 4.1 GET /api/motos - Listar Todas las Motos

**Response Success (200)** - ACTUALIZADO:
```json
{
  "success": true,
  "message": null,
  "data": [
    {
 "id": 1,
      "marca": "Honda",
      "modelo": "CB 500X",
      "año": 2023,
      "precio": 8000.00,
      "descripcion": "Honda CB 500X 2023, impecable",
 "disponible": true,
      "estado": "0km",
      "fechaPublicacion": "2024-11-10T08:00:00Z",
      
      // NUEVAS PROPIEDADES
      "cilindrada": 500,
      "tipoMoto": "Touring",
      "kilometraje": null,
      
"imagenes": [
  {
          "id": 1,
    "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/motos/1/xyz789.jpg"
    }
      ]
    }
  ],
  "errors": null
}
```

---

### 4.3 POST /api/motos - Crear Moto

**Request Body** - ACTUALIZADO:
```json
{
  "marca": "Yamaha",
  "modelo": "MT-07",
  "año": 2023,
"precio": 9500.00,
  "descripcion": "Yamaha MT-07 2023, muy cuidada",
  "disponible": true,
  
  // NUEVAS PROPIEDADES REQUERIDAS
  "estado": "Usado",
  "cilindrada": 689,
  
  // NUEVAS PROPIEDADES OPCIONALES
  "tipoMoto": "Naked",
  "kilometraje": 12000
}
```

**Validaciones Frontend**:
- `estado`: requerido, debe ser "Usado" o "0km"
- `cilindrada`: requerido, debe estar entre 50 y 2000
- `tipoMoto`: opcional, valores: Deportiva, Cruiser, Touring, Naked, Enduro
- `kilometraje`: opcional, null para 0km, número positivo para usadas

---

## ?? EJEMPLOS PRÁCTICOS

### Ejemplo 1: Card de Auto en Listado

```javascript
const AutoCard = ({ auto }) => {
  const imagenPrincipal = auto.imagenes[0]?.url || '/placeholder.jpg';
  
  return (
    <div className="card">
      <img src={imagenPrincipal} alt={`${auto.marca} ${auto.modelo}`} />
      
      {/* Badge de Estado */}
    <span className={`badge ${auto.estado === '0km' ? 'badge-new' : 'badge-used'}`}>
        {auto.estado}
      </span>
      
      <h3>{auto.marca} {auto.modelo} {auto.año}</h3>
      
      {/* Información técnica */}
      <div className="specs">
 <span>?? {auto.puertas} puertas</span>
   <span>? {auto.tipoCombustible}</span>
        {auto.transmision && <span>?? {auto.transmision}</span>}
  {auto.kilometraje && <span>??? {formatKm(auto.kilometraje)} km</span>}
      </div>
      
      <p className="price">{formatPrecio(auto.precio)}</p>
      <p className="description">{auto.descripcion}</p>
      
   <button onClick={() => navigate(`/autos/${auto.id}`)}>
      Ver Detalles
      </button>
    </div>
  );
};
```

---

### Ejemplo 2: Detalle de Auto

```javascript
const AutoDetalle = ({ auto }) => {
  return (
    <div className="detail-page">
      <div className="image-gallery">
        {/* Carrusel de imágenes */}
      </div>
      
      <div className="info">
        <h1>{auto.marca} {auto.modelo}</h1>
        <h2>{formatPrecio(auto.precio)}</h2>
    
   <div className="badges">
          <span className={`badge ${auto.estado === '0km' ? 'badge-new' : 'badge-used'}`}>
   {auto.estado}
  </span>
          {auto.disponible ? (
  <span className="badge badge-available">Disponible</span>
          ) : (
            <span className="badge badge-sold">Vendido</span>
          )}
        </div>
      
        <div className="specs-grid">
          <div className="spec-item">
        <strong>Año:</strong> {auto.año}
          </div>
          <div className="spec-item">
     <strong>Puertas:</strong> {auto.puertas}
          </div>
          <div className="spec-item">
     <strong>Combustible:</strong> {auto.tipoCombustible}
          </div>
  {auto.transmision && (
            <div className="spec-item">
    <strong>Transmisión:</strong> {auto.transmision}
        </div>
          )}
   {auto.kilometraje && (
          <div className="spec-item">
              <strong>Kilometraje:</strong> {formatKm(auto.kilometraje)} km
          </div>
     )}
   </div>
        
      <div className="description">
          <h3>Descripción</h3>
          <p>{auto.descripcion}</p>
      </div>
        
 <button className="btn-contact">
   Consultar por este vehículo
        </button>
      </div>
    </div>
  );
};
```

---

### Ejemplo 3: Formulario de Crear Auto

```javascript
const CreateAutoForm = () => {
  const [formData, setFormData] = useState({
    marca: '',
    modelo: '',
    año: new Date().getFullYear(),
    precio: '',
    descripcion: '',
    disponible: true,
estado: 'Usado',
    puertas: 4,
    tipoCombustible: 'Nafta',
    transmision: '',
    kilometraje: null
  });
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      // Si es 0km, kilometraje debe ser null
  const data = {
        ...formData,
        kilometraje: formData.estado === '0km' ? null : formData.kilometraje
      };
   
 const response = await axios.post('/api/autos', data);
      alert('Auto creado exitosamente');
      navigate('/admin/autos');
    } catch (error) {
      alert(error.response?.data?.message || 'Error al crear auto');
    }
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
      placeholder="Marca"
   value={formData.marca}
        onChange={(e) => setFormData({...formData, marca: e.target.value})}
        required
  />
      
      <input
    type="text"
        placeholder="Modelo"
        value={formData.modelo}
     onChange={(e) => setFormData({...formData, modelo: e.target.value})}
        required
      />
    
      <input
        type="number"
        placeholder="Año"
     value={formData.año}
        onChange={(e) => setFormData({...formData, año: parseInt(e.target.value)})}
      required
      />
      
 <input
      type="number"
        placeholder="Precio"
        value={formData.precio}
  onChange={(e) => setFormData({...formData, precio: parseFloat(e.target.value)})}
        required
      />
      
   {/* Estado */}
      <select
        value={formData.estado}
    onChange={(e) => setFormData({...formData, estado: e.target.value})}
        required
      >
        <option value="Usado">Usado</option>
        <option value="0km">0 Kilómetro</option>
      </select>
  
      {/* Puertas */}
      <select
        value={formData.puertas}
        onChange={(e) => setFormData({...formData, puertas: parseInt(e.target.value)})}
        required
      >
  <option value={2}>2 puertas</option>
        <option value={3}>3 puertas</option>
        <option value={4}>4 puertas</option>
    <option value={5}>5 puertas</option>
      </select>
      
      {/* Combustible */}
      <select
 value={formData.tipoCombustible}
     onChange={(e) => setFormData({...formData, tipoCombustible: e.target.value})}
 required
      >
        <option value="Nafta">Nafta</option>
     <option value="Gasoil">Gasoil</option>
        <option value="GNC">GNC</option>
   <option value="Híbrido">Híbrido</option>
        <option value="Eléctrico">Eléctrico</option>
      </select>
      
      {/* Transmisión */}
      <select
  value={formData.transmision}
        onChange={(e) => setFormData({...formData, transmision: e.target.value})}
      >
        <option value="">Seleccionar...</option>
        <option value="Manual">Manual</option>
  <option value="Automática">Automática</option>
      </select>

   {/* Kilometraje (solo si es Usado) */}
      {formData.estado === 'Usado' && (
        <input
      type="number"
        placeholder="Kilometraje"
   value={formData.kilometraje || ''}
          onChange={(e) => setFormData({...formData, kilometraje: parseInt(e.target.value)})}
        />
      )}
      
      <textarea
        placeholder="Descripción"
        value={formData.descripcion}
  onChange={(e) => setFormData({...formData, descripcion: e.target.value})}
      />
      
      <button type="submit">Crear Auto</button>
    </form>
  );
};
```

---

### Ejemplo 4: Filtros de Búsqueda

```javascript
const FiltrosAutos = ({ onFilter }) => {
  const [filtros, setFiltros] = useState({
    estado: 'todos',
    tipoCombustible: 'todos',
    transmision: 'todos',
    precioMin: '',
    precioMax: '',
    añoMin: '',
    añoMax: ''
  });
  
  const aplicarFiltros = (autos) => {
    return autos.filter(auto => {
      // Filtro por estado
      if (filtros.estado !== 'todos' && auto.estado !== filtros.estado) {
        return false;
 }
      
      // Filtro por combustible
      if (filtros.tipoCombustible !== 'todos' && auto.tipoCombustible !== filtros.tipoCombustible) {
        return false;
      }
    
      // Filtro por transmisión
      if (filtros.transmision !== 'todos' && auto.transmision !== filtros.transmision) {
        return false;
}
      
      // Filtro por precio
      if (filtros.precioMin && auto.precio < parseFloat(filtros.precioMin)) {
        return false;
 }
      if (filtros.precioMax && auto.precio > parseFloat(filtros.precioMax)) {
        return false;
      }
      
      // Filtro por año
      if (filtros.añoMin && auto.año < parseInt(filtros.añoMin)) {
  return false;
  }
      if (filtros.añoMax && auto.año > parseInt(filtros.añoMax)) {
        return false;
      }
  
      return true;
    });
  };
  
  return (
    <div className="filtros">
    <select
        value={filtros.estado}
        onChange={(e) => setFiltros({...filtros, estado: e.target.value})}
      >
        <option value="todos">Todos</option>
    <option value="0km">0 Kilómetro</option>
        <option value="Usado">Usados</option>
  </select>
      
      <select
        value={filtros.tipoCombustible}
        onChange={(e) => setFiltros({...filtros, tipoCombustible: e.target.value})}
      >
     <option value="todos">Todos los combustibles</option>
        <option value="Nafta">Nafta</option>
     <option value="Gasoil">Gasoil</option>
 <option value="GNC">GNC</option>
  <option value="Híbrido">Híbrido</option>
     <option value="Eléctrico">Eléctrico</option>
      </select>
      
      <select
        value={filtros.transmision}
        onChange={(e) => setFiltros({...filtros, transmision: e.target.value})}
      >
     <option value="todos">Todas las transmisiones</option>
        <option value="Manual">Manual</option>
 <option value="Automática">Automática</option>
    </select>
      
  <input
type="number"
        placeholder="Precio mínimo"
        value={filtros.precioMin}
        onChange={(e) => setFiltros({...filtros, precioMin: e.target.value})}
   />
    
      <input
        type="number"
    placeholder="Precio máximo"
        value={filtros.precioMax}
        onChange={(e) => setFiltros({...filtros, precioMax: e.target.value})}
      />

      <button onClick={() => onFilter(filtros)}>
        Aplicar Filtros
      </button>
    </div>
  );
};
```

---

## ??? UTILIDADES ADICIONALES

### Formateo de Kilometraje

```javascript
const formatKm = (kilometraje) => {
  if (!kilometraje) return '0 km';
  return new Intl.NumberFormat('es-AR').format(kilometraje) + ' km';
};

// Uso: formatKm(45000) ? "45.000 km"
```

### Badge de Estado

```javascript
const EstadoBadge = ({ estado }) => {
  const config = {
    '0km': {
      label: '0 KM',
      color: 'green',
      icon: '?'
    },
    'Usado': {
      label: 'Usado',
    color: 'blue',
      icon: '??'
    }
  };
  
  const { label, color, icon } = config[estado];
  
  return (
    <span className={`badge badge-${color}`}>
      {icon} {label}
    </span>
  );
};
```

### Icono de Combustible

```javascript
const getCombustibleIcon = (tipo) => {
  const icons = {
    'Nafta': '?',
    'Gasoil': '??',
    'GNC': '??',
 'Híbrido': '??',
    'Eléctrico': '?'
  };
  return icons[tipo] || '?';
};
```

---

## ? CHECKLIST DE ACTUALIZACIÓN FRONTEND

### Componentes a Actualizar
- [ ] Card de vehículo en listado
- [ ] Página de detalle de vehículo
- [ ] Formulario de creación
- [ ] Formulario de edición
- [ ] Sistema de filtros
- [ ] Badges de estado
- [ ] Iconos de especificaciones

### Validaciones a Implementar
- [ ] Estado: "Usado" o "0km"
- [ ] Puertas: 2-5 (solo autos)
- [ ] Combustible: valores válidos
- [ ] Cilindrada: 50-2000 (solo motos)
- [ ] Kilometraje: null si es 0km

### Estilos a Agregar
- [ ] Badge para 0km (verde, destacado)
- [ ] Badge para Usado (azul)
- [ ] Iconos de especificaciones
- [ ] Grid de especificaciones
- [ ] Responsive design

---

## ?? RESUMEN DE CAMBIOS

| Entidad | Nuevas Propiedades | Migración Requerida |
|---------|-------------------|---------------------|
| **VehiculoBase** | `estado` | ? |
| **Auto** | `puertas`, `tipoCombustible`, `transmision`, `kilometraje` | ? |
| **Moto** | `cilindrada`, `tipoMoto`, `kilometraje` | ? |

**Endpoints Afectados**: 
- GET /api/autos
- GET /api/autos/{id}
- POST /api/autos
- PUT /api/autos/{id}
- GET /api/motos
- GET /api/motos/{id}
- POST /api/motos
- PUT /api/motos/{id}

---

**Versión**: 1.1.0  
**Fecha**: Noviembre 2024  
**Cambio**: Propiedades de Concesionaria ?
