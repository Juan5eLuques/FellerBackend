# ?? MIGRACIÓN: Agregar Propiedades de Concesionaria

## Nuevas Propiedades Agregadas

### VehiculoBase (Común)
- ? `Estado` (string) - "Usado" o "0km"

### Auto (Específicas)
- ? `Puertas` (int) - Cantidad de puertas (2-5)
- ? `TipoCombustible` (string) - Nafta, Gasoil, GNC, Híbrido, Eléctrico
- ? `Transmision` (string, nullable) - Manual, Automática
- ? `Kilometraje` (int, nullable) - Null para 0km

### Moto (Específicas)
- ? `Cilindrada` (int) - CC de la moto
- ? `TipoMoto` (string, nullable) - Deportiva, Cruiser, Touring, Naked, Enduro
- ? `Kilometraje` (int, nullable) - Null para 0km

---

## Comandos para Crear Migración

### 1. Crear la migración
```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet ef migrations add AgregarPropiedadesConcesionaria
```

### 2. Actualizar base de datos
```bash
dotnet ef database update
```

---

## Verificación Post-Migración

### Verificar en PostgreSQL
```sql
-- Ver estructura de tabla Vehiculos
\d "Vehiculos"

-- Debería mostrar las nuevas columnas:
-- Estado, Puertas, TipoCombustible, Transmision, Kilometraje, Cilindrada, TipoMoto
```

---

## Valores Por Defecto

### Para Autos
- **Estado**: "Usado"
- **Puertas**: 4
- **TipoCombustible**: "Nafta"
- **Transmision**: null
- **Kilometraje**: null (para 0km)

### Para Motos
- **Estado**: "Usado"
- **Cilindrada**: requerido
- **TipoMoto**: null
- **Kilometraje**: null (para 0km)

---

## Ejemplo de Datos

### Auto 0km
```json
{
  "marca": "Toyota",
  "modelo": "Corolla",
  "año": 2024,
  "precio": 35000.00,
  "estado": "0km",
  "puertas": 4,
  "tipoCombustible": "Nafta",
  "transmision": "Automática",
  "kilometraje": null
}
```

### Auto Usado
```json
{
  "marca": "Ford",
  "modelo": "Focus",
  "año": 2020,
  "precio": 22000.00,
  "estado": "Usado",
  "puertas": 5,
  "tipoCombustible": "Gasoil",
  "transmision": "Manual",
  "kilometraje": 45000
}
```

### Moto 0km
```json
{
  "marca": "Honda",
  "modelo": "CB 500X",
  "año": 2024,
  "precio": 10000.00,
  "estado": "0km",
  "cilindrada": 500,
  "tipoMoto": "Touring",
  "kilometraje": null
}
```

### Moto Usada
```json
{
  "marca": "Yamaha",
  "modelo": "MT-07",
  "año": 2021,
  "precio": 8500.00,
  "estado": "Usado",
  "cilindrada": 689,
  "tipoMoto": "Naked",
  "kilometraje": 12000
}
```

---

## ?? IMPORTANTE

Después de ejecutar la migración:

1. ? Los vehículos existentes tendrán valores por defecto
2. ? Actualizar datos existentes si es necesario
3. ? El frontend debe adaptarse a las nuevas propiedades
4. ? Actualizar la documentación de la API

---

**Estado**: Pendiente de ejecutar  
**Próximo paso**: Ejecutar comandos de migración
