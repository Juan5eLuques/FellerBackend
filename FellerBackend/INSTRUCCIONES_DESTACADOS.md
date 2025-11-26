# ?? INSTRUCCIONES FINALES - Vehículos Destacados

## ? ESTADO ACTUAL

**Código completado** ?  
**Compilación exitosa** ?  
**Pendiente**: Migración de base de datos

---

## ?? PASOS PARA ACTIVAR

### 1. Detener el Servidor (Si está corriendo)

```bash
Ctrl + C
```

### 2. Crear Migración

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef migrations add AgregarVehiculosDestacados
```

**Resultado esperado**:
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

### 3. Aplicar Migración

```bash
dotnet ef database update
```

**Resultado esperado**:
```
Applying migration '20241119XXXXXX_AgregarVehiculosDestacados'.
Done.
```

### 4. Iniciar Servidor

```bash
dotnet run
```

---

## ?? PROBAR EN SWAGGER

### 1. Abrir Swagger

```
http://localhost:5000
```

### 2. Login como Admin

```http
POST /api/auth/login
{
  "email": "admin@feller.com",
  "password": "Admin123!"
}
```

**Copiar el token** del response.

### 3. Authorize en Swagger

Click en el botón **"Authorize"** (candado verde)  
Pegar: `Bearer {tu_token}`

### 4. Marcar Vehículo como Destacado

```http
POST /api/destacados/autos/1?orden=1
```

**Response esperado**:
```json
{
  "success": true,
  "message": "Auto marcado como destacado con orden 1",
  "data": {
 "id": 1,
    "esDestacado": true,
    "ordenDestacado": 1,
...
  }
}
```

### 5. Obtener Destacados (Público)

```http
GET /api/destacados
```

**Response esperado**:
```json
{
  "success": true,
  "data": [
    {
    "tipo": "Auto",
      "vehiculo": {
        "id": 1,
   "marca": "...",
        "esDestacado": true,
        "ordenDestacado": 1
      }
    }
  ]
}
```

---

## ?? ENDPOINTS DISPONIBLES

### Públicos
- ? `GET /api/destacados` - Todos los destacados
- ? `GET /api/destacados/autos` - Solo autos
- ? `GET /api/destacados/motos` - Solo motos

### Admin
- ? `POST /api/destacados/autos/{id}?orden={num}` - Marcar auto
- ? `POST /api/destacados/motos/{id}?orden={num}` - Marcar moto
- ? `DELETE /api/destacados/autos/{id}` - Desmarcar auto
- ? `DELETE /api/destacados/motos/{id}` - Desmarcar moto

---

## ?? INTEGRACIÓN FRONTEND

### React - Obtener Destacados para Home

```javascript
import { useState, useEffect } from 'react';
import axios from 'axios';

const Home = () => {
  const [destacados, setDestacados] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDestacados = async () => {
      try {
const response = await axios.get('http://localhost:5000/api/destacados');
   setDestacados(response.data.data);
      } catch (error) {
        console.error('Error al cargar destacados:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchDestacados();
  }, []);

  if (loading) return <div>Cargando...</div>;

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

const VehiculoCard = ({ tipo, vehiculo }) => {
  const imagen = vehiculo.imagenes[0]?.url || '/placeholder.jpg';
  const ruta = tipo === 'Auto' ? '/autos' : '/motos';

  return (
    <div className="card">
 <span className="badge-destacado">? Destacado</span>
    <img src={imagen} alt={`${vehiculo.marca} ${vehiculo.modelo}`} />
      <h3>{vehiculo.marca} {vehiculo.modelo}</h3>
      <p className="price">${vehiculo.precio.toLocaleString()}</p>
      <a href={`${ruta}/${vehiculo.id}`}>Ver Detalles</a>
    </div>
  );
};

export default Home;
```

---

### Admin Panel - Gestionar Destacados

```javascript
const GestionarDestacados = () => {
  const [autos, setAutos] = useState([]);
  const token = localStorage.getItem('token');

  const marcarDestacado = async (id, orden) => {
    try {
      await axios.post(
`http://localhost:5000/api/destacados/autos/${id}?orden=${orden}`,
    {},
        { headers: { Authorization: `Bearer ${token}` }}
      );
      alert('? Marcado como destacado');
      // Recargar lista
    } catch (error) {
alert('? Error: ' + error.response?.data?.message);
  }
  };

  const desmarcarDestacado = async (id) => {
    if (!confirm('¿Desmarcar como destacado?')) return;
    
    try {
      await axios.delete(
`http://localhost:5000/api/destacados/autos/${id}`,
        { headers: { Authorization: `Bearer ${token}` }}
      );
      alert('? Desmarcado');
      // Recargar lista
    } catch (error) {
alert('? Error: ' + error.response?.data?.message);
    }
  };

  return (
    <div>
      <h2>Gestionar Destacados</h2>
  {autos.map(auto => (
        <div key={auto.id}>
          <span>{auto.marca} {auto.modelo}</span>
          {auto.esDestacado ? (
      <>
  <span>? Destacado (Orden: {auto.ordenDestacado})</span>
<button onClick={() => desmarcarDestacado(auto.id)}>
      Quitar
       </button>
     </>
) : (
        <button onClick={() => marcarDestacado(auto.id, 1)}>
              Marcar como Destacado
  </button>
          )}
 </div>
      ))}
  </div>
  );
};
```

---

## ?? CSS SUGERIDO

```css
.destacados {
  padding: 40px 20px;
  background: #f5f5f5;
}

.destacados h2 {
  text-align: center;
  margin-bottom: 30px;
  font-size: 32px;
}

.destacados .grid {
display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 24px;
}

.card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  transition: transform 0.3s;
  position: relative;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.15);
}

.badge-destacado {
  position: absolute;
  top: 12px;
  left: 12px;
  background: #FFD700;
  color: #333;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: bold;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
}

.card img {
  width: 100%;
  height: 220px;
  object-fit: cover;
}

.card h3 {
  padding: 16px;
  margin: 0;
  font-size: 20px;
}

.card .price {
  padding: 0 16px;
  font-size: 24px;
  color: #2563eb;
  font-weight: bold;
}

.card a {
  display: block;
  padding: 16px;
  text-align: center;
  background: #2563eb;
  color: white;
  text-decoration: none;
  font-weight: 600;
  transition: background 0.3s;
}

.card a:hover {
  background: #1d4ed8;
}
```

---

## ? CHECKLIST COMPLETO

### Backend
- [x] Modelo VehiculoBase actualizado
- [x] DbContext configurado
- [x] DTOs actualizados
- [x] Interface IVehiculoService extendida
- [x] VehiculoService implementado
- [x] DestacadosController creado
- [x] Código compilado exitosamente
- [ ] Migración creada
- [ ] Migración aplicada
- [ ] Probado en Swagger

### Frontend
- [ ] Componente Home con destacados
- [ ] Admin panel para gestionar
- [ ] CSS implementado
- [ ] Probado en navegador

---

## ?? DOCUMENTACIÓN

- **Completa**: `VEHICULOS_DESTACADOS.md`
- **Resumen**: `DESTACADOS_RESUMEN.md`
- **Este archivo**: Instrucciones paso a paso

---

## ?? TROUBLESHOOTING

### Error al crear migración

```bash
# Limpiar y rebuild
dotnet clean
dotnet build
dotnet ef migrations add AgregarVehiculosDestacados
```

### Error al aplicar migración

```bash
# Ver migraciones aplicadas
dotnet ef migrations list

# Verificar conexión a BD
psql -U postgres -h localhost -d feller_db -c "SELECT 1"
```

### No aparecen los endpoints en Swagger

```bash
# Reiniciar completamente
Ctrl + C
dotnet clean
dotnet run
```

---

## ?? RESULTADO FINAL

Después de completar estos pasos tendrás:

? **API funcional** con 7 nuevos endpoints  
? **Base de datos** actualizada con columnas de destacados  
? **Control total** del orden de aparición  
? **Sistema optimizado** con índices  
? **Documentación completa** para el equipo  

---

**Próximo paso**: Ejecutar los 4 comandos de la sección "PASOS PARA ACTIVAR"

**Tiempo estimado**: 5 minutos
