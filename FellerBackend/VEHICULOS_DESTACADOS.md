# ?? VEHÍCULOS DESTACADOS - Documentación Completa

## ?? RESUMEN

Se ha implementado un sistema completo para gestionar vehículos destacados que se mostrarán en la home de la web.

---

## ?? ENFOQUE IMPLEMENTADO

**Flag + Orden** en el modelo `VehiculoBase`:

```csharp
public bool EsDestacado { get; set; } = false;
public int? OrdenDestacado { get; set; }
```

### ? Ventajas
- ? Simple y directo
- ? Control total del orden
- ? No requiere tablas adicionales
- ? Consultas rápidas con índice
- ? Funciona para autos y motos

---

## ?? CAMBIOS REALIZADOS

### 1. Modelos Actualizados

#### VehiculoBase.cs
```csharp
+ public bool EsDestacado { get; set; } = false;
+ public int? OrdenDestacado { get; set; }
```

### 2. DbContext Configurado

```csharp
// Índice para búsqueda rápida
entity.HasIndex(e => new { e.EsDestacado, e.OrdenDestacado });
entity.Property(e => e.EsDestacado).HasDefaultValue(false);
```

### 3. DTOs Actualizados

? `AutoDto` - Incluye `EsDestacado` y `OrdenDestacado`  
? `CreateAutoDto` - Permite establecer al crear  
? `UpdateAutoDto` - Permite modificar  
? `MotoDto` - Incluye `EsDestacado` y `OrdenDestacado`  
? `CreateMotoDto` - Permite establecer al crear  
? `UpdateMotoDto` - Permite modificar  

### 4. Servicios

#### IVehiculoService.cs - Nuevos Métodos

```csharp
// Obtener destacados
Task<List<AutoDto>> GetAutosDestacadosAsync();
Task<List<MotoDto>> GetMotosDestacadasAsync();
Task<List<object>> GetVehiculosDestacadosAsync(); // Mixto

// Gestionar destacados
Task<AutoDto> MarcarAutoComoDestacadoAsync(int id, int? orden = null);
Task<MotoDto> MarcarMotoComoDestacadaAsync(int id, int? orden = null);
Task<AutoDto> DesmarcarAutoDestacadoAsync(int id);
Task<MotoDto> DesmarcarMotoDestacadaAsync(int id);
```

#### VehiculoService.cs - Implementación

? Todos los métodos implementados con mapeo completo  
? Auto-asignación de orden si no se proporciona  
? Ordenamiento por `OrdenDestacado` y `FechaPublicacion`  

### 5. Nuevo Controlador

**DestacadosController.cs** - API RESTful completa

---

## ?? ENDPOINTS DISPONIBLES

### ?? OBTENER DESTACADOS (PÚBLICO)

#### 1. Obtener Todos los Vehículos Destacados

```http
GET /api/destacados
```

**Response**:
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
        "esDestacado": true,
     "ordenDestacado": 2,
        "imagenes": [...]
      }
    }
  ]
}
```

**Orden**: Por `OrdenDestacado` ascendente

---

#### 2. Obtener Solo Autos Destacados

```http
GET /api/destacados/autos
```

**Response**:
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
      "estado": "0km",
      "puertas": 4,
      "tipoCombustible": "Nafta",
      "esDestacado": true,
      "ordenDestacado": 1,
      "imagenes": [
    {
          "id": 1,
          "url": "https://..."
        }
      ]
 }
  ]
}
```

**Filtro**: Solo `EsDestacado = true` y `Disponible = true`  
**Orden**: `OrdenDestacado` ASC, luego `FechaPublicacion` DESC

---

#### 3. Obtener Solo Motos Destacadas

```http
GET /api/destacados/motos
```

**Response**: Similar a autos

---

### ?? GESTIONAR DESTACADOS (ADMIN)

#### 4. Marcar Auto como Destacado

```http
POST /api/destacados/autos/{id}?orden={numero}
Authorization: Bearer {admin_token}
```

**Parámetros**:
- `id` (requerido): ID del auto
- `orden` (opcional): Orden de aparición (1, 2, 3...)

**Si no se proporciona `orden`**: Se auto-asigna el siguiente número disponible.

**Response**:
```json
{
  "success": true,
  "message": "Auto marcado como destacado con orden 3",
  "data": {
    "id": 1,
    "marca": "Toyota",
    "esDestacado": true,
 "ordenDestacado": 3,
  ...
  }
}
```

---

#### 5. Marcar Moto como Destacada

```http
POST /api/destacados/motos/{id}?orden={numero}
Authorization: Bearer {admin_token}
```

Similar a autos.

---

#### 6. Desmarcar Auto Destacado

```http
DELETE /api/destacados/autos/{id}
Authorization: Bearer {admin_token}
```

**Response**:
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

---

#### 7. Desmarcar Moto Destacada

```http
DELETE /api/destacados/motos/{id}
Authorization: Bearer {admin_token}
```

Similar a autos.

---

## ?? EJEMPLOS DE USO

### Frontend - Obtener Destacados para Home

```javascript
// React/Vue/Angular
const DestacadosHome = () => {
  const [destacados, setDestacados] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDestacados = async () => {
      try {
        const response = await axios.get('/api/destacados');
    setDestacados(response.data.data);
      } catch (error) {
        console.error('Error al cargar destacados:', error);
      } finally {
        setLoading(false);
      }
};

    fetchDestacados();
  }, []);

  if (loading) return <Spinner />;

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

---

### Admin Panel - Marcar como Destacado

```javascript
const MarcarDestacado = async (vehiculoId, tipo, orden = null) => {
  const token = localStorage.getItem('token');
  
  try {
    const url = `/api/destacados/${tipo.toLowerCase()}s/${vehiculoId}`;
    const params = orden ? `?orden=${orden}` : '';
    
    const response = await axios.post(
      url + params,
      {},
      {
  headers: {
          'Authorization': `Bearer ${token}`
  }
      }
    );

    alert(response.data.message);
    // Recargar lista
  } catch (error) {
    alert('Error: ' + error.response?.data?.message);
  }
};

// Uso
<button onClick={() => MarcarDestacado(autoId, 'Auto', 1)}>
  Marcar como Destacado (Orden 1)
</button>
```

---

### Admin Panel - Desmarcar Destacado

```javascript
const DesmarcarDestacado = async (vehiculoId, tipo) => {
  const token = localStorage.getItem('token');
  
  if (!confirm('¿Desmarcar como destacado?')) return;
  
  try {
    const response = await axios.delete(
      `/api/destacados/${tipo.toLowerCase()}s/${vehiculoId}`,
      {
        headers: {
          'Authorization': `Bearer ${token}`
      }
      }
    );

    alert(response.data.message);
    // Recargar lista
  } catch (error) {
    alert('Error: ' + error.response?.data?.message);
  }
};
```

---

### Admin Panel - Gestionar Orden

```javascript
const GestionDestacados = () => {
  const [autos, setAutos] = useState([]);
  const [motos, setMotos] = useState([]);

  const cambiarOrden = async (id, tipo, nuevoOrden) => {
    // Desmarcar y volver a marcar con nuevo orden
    await DesmarcarDestacado(id, tipo);
    await MarcarDestacado(id, tipo, nuevoOrden);
  };

  return (
    <div>
      <h2>Gestionar Destacados</h2>
   
      <h3>Autos Destacados</h3>
      {autos.filter(a => a.esDestacado).map(auto => (
        <div key={auto.id}>
  <span>{auto.marca} {auto.modelo}</span>
          <input 
type="number" 
   value={auto.ordenDestacado}
     onChange={(e) => cambiarOrden(auto.id, 'Auto', e.target.value)}
          />
          <button onClick={() => DesmarcarDestacado(auto.id, 'Auto')}>
            Quitar
          </button>
        </div>
      ))}
    </div>
  );
};
```

---

## ?? LÓGICA DE ORDENAMIENTO

### Orden Automático

Si no se proporciona `orden` al marcar como destacado:

```csharp
var maxOrden = await _context.Vehiculos
  .Where(v => v.EsDestacado && v.OrdenDestacado.HasValue)
    .MaxAsync(v => (int?)v.OrdenDestacado) ?? 0;
orden = maxOrden + 1;
```

**Ejemplo**:
- Hay 3 destacados con orden: 1, 2, 3
- Se marca uno nuevo sin especificar orden
- **Se asigna automáticamente**: orden = 4

### Ordenamiento en Consultas

```csharp
.OrderBy(v => v.OrdenDestacado ?? int.MaxValue)
.ThenByDescending(v => v.FechaPublicacion)
```

**Significado**:
1. Primero por `OrdenDestacado` (ascendente)
2. Si `OrdenDestacado` es null, se pone al final
3. Entre vehículos sin orden, los más recientes primero

---

## ?? COMPONENTE FRONTEND SUGERIDO

### Card de Vehículo Destacado

```javascript
const VehiculoDestacadoCard = ({ tipo, vehiculo }) => {
  const imagenPrincipal = vehiculo.imagenes[0]?.url || '/placeholder.jpg';
  const ruta = tipo === 'Auto' ? '/autos' : '/motos';

  return (
    <div className="destacado-card">
   {/* Badge "Destacado" */}
      <span className="badge-destacado">? Destacado</span>
      
      {/* Badge de Estado (0km/Usado) */}
      <span className={`badge-estado ${vehiculo.estado === '0km' ? 'badge-new' : 'badge-used'}`}>
        {vehiculo.estado}
      </span>

      {/* Imagen */}
      <img src={imagenPrincipal} alt={`${vehiculo.marca} ${vehiculo.modelo}`} />

      {/* Información */}
      <div className="info">
        <h3>{vehiculo.marca} {vehiculo.modelo}</h3>
        <p className="year">{vehiculo.año}</p>
        <p className="price">{formatPrecio(vehiculo.precio)}</p>
        
        {/* Specs específicas */}
 {tipo === 'Auto' && (
          <div className="specs">
     <span>?? {vehiculo.puertas} puertas</span>
<span>? {vehiculo.tipoCombustible}</span>
       </div>
        )}
        
    {tipo === 'Moto' && (
          <div className="specs">
        <span>??? {vehiculo.cilindrada}cc</span>
         {vehiculo.tipoMoto && <span>{vehiculo.tipoMoto}</span>}
          </div>
        )}

        <button onClick={() => navigate(`${ruta}/${vehiculo.id}`)}>
          Ver Detalles
        </button>
      </div>
    </div>
);
};
```

---

## ??? MIGRACIÓN REQUERIDA

Para aplicar estos cambios a la base de datos:

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef migrations add AgregarVehiculosDestacados
dotnet ef database update
```

**Columnas que se crearán**:
- `EsDestacado` (boolean, default: false)
- `OrdenDestacado` (integer, nullable)

**Índice que se creará**:
- `IX_Vehiculos_EsDestacado_OrdenDestacado`

---

## ?? CONFIGURACIÓN RECOMENDADA

### Cantidad de Destacados

En el frontend, limitar la cantidad mostrada:

```javascript
// Mostrar solo los primeros 6 destacados
const fetchDestacados = async () => {
  const response = await axios.get('/api/destacados');
const destacados = response.data.data.slice(0, 6);
  setDestacados(destacados);
};
```

O crear un endpoint específico:

```csharp
[HttpGet("top/{cantidad}")]
public async Task<IActionResult> GetTopDestacados(int cantidad = 6)
{
    var vehiculos = await _vehiculoService.GetVehiculosDestacadosAsync();
    return Ok(ResponseWrapper<List<object>>.SuccessResponse(vehiculos.Take(cantidad).ToList()));
}
```

---

## ?? TESTING

### 1. Crear Admin y Login

```http
POST /api/seed/create-first-admin
POST /api/auth/login
```

### 2. Crear Vehículos de Prueba

```http
POST /api/autos
POST /api/motos
```

### 3. Marcar como Destacados

```http
POST /api/destacados/autos/1?orden=1
POST /api/destacados/motos/1?orden=2
POST /api/destacados/autos/2
```

### 4. Verificar en Home

```http
GET /api/destacados
```

### 5. Gestionar Orden

```http
POST /api/destacados/autos/1?orden=5  # Cambiar orden
DELETE /api/destacados/autos/1          # Quitar destacado
```

---

## ?? DASHBOARD ADMIN

Agregar estadística de destacados:

```csharp
// En DashboardService.cs
public int VehiculosDestacados => _context.Vehiculos
    .Count(v => v.EsDestacado);
```

---

## ? CHECKLIST DE IMPLEMENTACIÓN

- [x] Modelo `VehiculoBase` actualizado
- [x] `DbContext` configurado con índice
- [x] DTOs actualizados (Auto y Moto)
- [x] Interface `IVehiculoService` extendida
- [x] `VehiculoService` con métodos implementados
- [x] `DestacadosController` creado
- [x] Endpoints documentados
- [ ] Migración creada (`dotnet ef migrations add`)
- [ ] Migración aplicada (`dotnet ef database update`)
- [ ] Frontend: Componente Home con destacados
- [ ] Frontend: Admin panel para gestionar destacados
- [ ] Testing completo

---

## ?? PRÓXIMOS PASOS

1. **Crear y aplicar migración**:
```bash
dotnet ef migrations add AgregarVehiculosDestacados
dotnet ef database update
```

2. **Probar en Swagger**:
   - Marcar vehículos como destacados
   - Obtener lista de destacados
   - Cambiar orden
 - Quitar destacados

3. **Implementar en Frontend**:
   - Componente para home
   - Admin panel para gestión

---

**Versión**: 1.3.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Implementado - Pendiente migración
