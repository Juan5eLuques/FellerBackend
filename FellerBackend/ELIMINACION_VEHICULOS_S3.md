# ??? ELIMINACIÓN DE VEHÍCULOS CON IMÁGENES EN S3

## ?? RESUMEN

Se ha implementado la eliminación automática de imágenes de AWS S3 cuando se elimina un vehículo (auto o moto).

---

## ? FUNCIONALIDAD IMPLEMENTADA

### Flujo de Eliminación

```
1. Usuario Admin elimina vehículo (DELETE /api/autos/{id} o /api/motos/{id})
   ?
2. VehiculoService obtiene el vehículo con todas sus imágenes
   ?
3. Por cada imagen asociada:
   - Extrae el key de S3 desde la URL
   - Llama a ImagenService.DeleteImageAsync(key)
   - Elimina archivo de S3
   ?
4. Elimina el vehículo de la base de datos (CASCADE elimina imágenes en BD)
   ?
5. Retorna true si todo fue exitoso
```

---

## ?? CAMBIOS REALIZADOS

### 1. VehiculoService.cs

#### Constructor actualizado

```csharp
private readonly IImagenService _imagenService;

public VehiculoService(FellerDbContext context, IImagenService imagenService)
{
    _context = context;
    _imagenService = imagenService; // ? Inyección de ImagenService
}
```

#### DeleteAutoAsync actualizado

```csharp
public async Task<bool> DeleteAutoAsync(int id)
{
    var auto = await _context.Autos
     .Include(a => a.Imagenes) // ? Incluir imágenes
        .FirstOrDefaultAsync(a => a.Id == id);

    if (auto == null)
        return false;

    // ? Eliminar imágenes de S3
    if (auto.Imagenes.Any())
    {
        foreach (var imagen in auto.Imagenes)
        {
 try
            {
           var uri = new Uri(imagen.Url);
     var key = uri.AbsolutePath.TrimStart('/');
             await _imagenService.DeleteImageAsync(key);
    }
      catch (Exception ex)
     {
   Console.WriteLine($"Error al eliminar imagen de S3: {ex.Message}");
            }
    }
    }

    _context.Autos.Remove(auto);
    await _context.SaveChangesAsync();
    return true;
}
```

#### DeleteMotoAsync actualizado

Similar a DeleteAutoAsync.

---

## ?? ENDPOINTS AFECTADOS

### DELETE /api/autos/{id}

**Antes**:
- Eliminaba solo el auto de la BD
- Las imágenes quedaban huérfanas en S3 ??

**Ahora**:
- Elimina el auto de la BD
- Elimina TODAS las imágenes asociadas de S3 ?
- Libera espacio en S3 (ahorra costos)

### DELETE /api/motos/{id}

**Antes**:
- Eliminaba solo la moto de la BD
- Las imágenes quedaban huérfanas en S3 ??

**Ahora**:
- Elimina la moto de la BD
- Elimina TODAS las imágenes asociadas de S3 ?
- Libera espacio en S3 (ahorra costos)

---

## ?? EJEMPLO DE USO

### Caso: Eliminar Auto con 3 Imágenes

```http
DELETE /api/autos/5
Authorization: Bearer {admin_token}
```

**Proceso interno**:

```
1. Buscar Auto ID=5 con sus imágenes
   ?? Imagenes:
      ?? autos/5/abc123.jpg
      ?? autos/5/def456.png
      ?? autos/5/ghi789.webp

2. Eliminar de S3:
   ?? DELETE autos/5/abc123.jpg ?
   ?? DELETE autos/5/def456.png ?
   ?? DELETE autos/5/ghi789.webp ?

3. Eliminar de BD:
   ?? DELETE FROM ImagenVehiculo WHERE VehiculoId=5 ?
   ?? DELETE FROM Vehiculos WHERE Id=5 ?

4. Respuesta:
   {
     "success": true,
     "message": "Auto eliminado exitosamente"
   }
```

---

## ??? MANEJO DE ERRORES

### Si falla eliminación de imagen en S3

```csharp
try
{
    await _imagenService.DeleteImageAsync(key);
}
catch (Exception ex)
{
    // ?? Se logea el error pero NO detiene la eliminación
    Console.WriteLine($"Error al eliminar imagen de S3: {ex.Message}");
}
```

**Comportamiento**:
- ? Continúa eliminando otras imágenes
- ? Continúa eliminando el vehículo de BD
- ?? Imagen puede quedar huérfana en S3 (pero es poco probable)

### Casos de error comunes

| Error | Causa | Solución |
|-------|-------|----------|
| `NoSuchKey` | Imagen ya fue eliminada | Ignorar, continuar |
| `AccessDenied` | Sin permisos en S3 | Verificar IAM policy |
| `NetworkError` | Sin conexión | Reintenta manualmente |

---

## ?? PERMISOS AWS REQUERIDOS

El usuario IAM debe tener permisos para eliminar objetos:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
   "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
    "Resource": "arn:aws:s3:::feller-automotores/*"
    }
  ]
}
```

---

## ?? TESTING

### Test Manual

```bash
# 1. Crear auto con imágenes
POST /api/autos
POST /api/autos/1/imagenes (subir 2 imágenes)

# 2. Verificar en S3
# Ver en AWS Console: feller-automotores/autos/1/

# 3. Eliminar auto
DELETE /api/autos/1

# 4. Verificar en S3
# La carpeta autos/1/ debe estar vacía o no existir
```

### Test con Postman

```javascript
// Pre-request: Crear auto y subir imágenes
pm.sendRequest({
    url: 'http://localhost:5000/api/autos',
    method: 'POST',
    header: { 'Authorization': `Bearer ${token}` },
    body: { /* auto data */ }
}, (err, res) => {
    const autoId = res.json().data.id;
    
 // Subir imagen
    // ...
    
    // Eliminar auto
    pm.sendRequest({
 url: `http://localhost:5000/api/autos/${autoId}`,
        method: 'DELETE',
        header: { 'Authorization': `Bearer ${token}` }
    }, (err, res) => {
 pm.expect(res.json().success).to.be.true;
    });
});
```

---

## ?? MEJORAS FUTURAS

### 1. Logging Estructurado

Reemplazar `Console.WriteLine` con ILogger:

```csharp
private readonly ILogger<VehiculoService> _logger;

// En catch:
_logger.LogError(ex, "Error al eliminar imagen de S3: {Key}", key);
```

### 2. Cola de Eliminación Asíncrona

Para evitar que falle la eliminación del vehículo si S3 está lento:

```csharp
// Agregar a una cola (Redis, RabbitMQ, etc.)
await _queueService.EnqueueImageDeletion(key);

// Worker procesa cola en background
```

### 3. Eliminación en Batch

Si hay muchas imágenes, eliminar en lote:

```csharp
var deleteRequest = new DeleteObjectsRequest
{
    BucketName = _bucketName,
    Objects = imagenes.Select(i => new KeyVersion { Key = ExtractKey(i.Url) }).ToList()
};

await _s3Client.DeleteObjectsAsync(deleteRequest);
```

### 4. Papelera (Soft Delete)

En lugar de eliminar permanentemente:

```csharp
// Mover a carpeta de papelera
var newKey = $"papelera/{DateTime.UtcNow:yyyy-MM-dd}/{key}";
await _s3Client.CopyObjectAsync(_bucketName, key, _bucketName, newKey);
await _s3Client.DeleteObjectAsync(_bucketName, key);
```

---

## ?? IMPACTO EN COSTOS AWS

### Antes (Sin eliminación automática)

```
Mes 1: Crear 100 autos con 300 imágenes
Mes 2: Eliminar 50 autos
       ?? 150 imágenes huérfanas en S3 ??
Mes 3: Eliminar 30 autos más
     ?? 240 imágenes huérfanas en S3 ????

Costo mensual S3: $0.023 por GB
Si cada imagen = 2MB
240 imágenes = 480MB = 0.48GB
Costo desperdiciado: ~$0.011/mes * ?
```

### Ahora (Con eliminación automática)

```
Mes 1: Crear 100 autos con 300 imágenes
Mes 2: Eliminar 50 autos
       ?? 150 imágenes eliminadas de S3 ?
Mes 3: Eliminar 30 autos más
  ?? 90 imágenes eliminadas de S3 ?

Costo mensual S3: Solo por imágenes activas
Ahorro: 100% en archivos huérfanos
```

---

## ? CHECKLIST DE IMPLEMENTACIÓN

- [x] Inyectar `IImagenService` en `VehiculoService`
- [x] Actualizar `DeleteAutoAsync` para eliminar imágenes de S3
- [x] Actualizar `DeleteMotoAsync` para eliminar imágenes de S3
- [x] Incluir `.Include(x => x.Imagenes)` en queries de eliminación
- [x] Extraer key de URL de S3 correctamente
- [x] Manejar errores sin detener eliminación
- [x] Compilar sin errores
- [ ] Probar en ambiente de desarrollo
- [ ] Probar en ambiente de producción
- [ ] Verificar eliminación en S3 Console
- [ ] Implementar logging estructurado (futuro)

---

## ?? RESUMEN

| Aspecto | Estado |
|---------|--------|
| Eliminación de imágenes S3 | ? Implementado |
| Eliminación de vehículo BD | ? Implementado |
| Manejo de errores | ? Implementado |
| Logging | ?? Básico (Console.WriteLine) |
| Testing | ? Pendiente |
| Documentación | ? Completada |

---

**Versión**: 1.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Implementado y listo para testing
