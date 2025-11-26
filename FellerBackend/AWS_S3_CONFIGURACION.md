# ?? CONFIGURACIÓN AWS S3 - Feller Automotores

## ? Error: "The bucket does not allow ACLs"

Este error ocurre porque tu bucket de S3 tiene **ACLs deshabilitadas** (configuración por defecto en buckets nuevos).

---

## ? SOLUCIÓN: Configurar Bucket Policy

### Opción 1: Bucket Policy (Recomendada)

#### 1. Ir a AWS S3 Console
```
https://s3.console.aws.amazon.com/s3/buckets/feller-automotores
```

#### 2. Ir a la pestaña "Permissions"

#### 3. Desbloquear acceso público
Busca "Block public access (bucket settings)" y click en "Edit"

Desmarcar:
- ? Block all public access

Guardar cambios.

#### 4. Agregar Bucket Policy
Scroll down hasta "Bucket policy" y click en "Edit"

Pega esta política:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
   "Resource": "arn:aws:s3:::feller-automotores/*"
    }
  ]
}
```

**?? IMPORTANTE**: Reemplaza `feller-automotores` con el nombre real de tu bucket si es diferente.

#### 5. Guardar política

#### 6. Probar nuevamente
Ahora deberías poder subir imágenes sin error.

---

## Opción 2: CORS Configuration (Adicional)

Si también necesitas acceso desde el frontend, configura CORS:

#### 1. En S3 Console, ir a "Permissions"

#### 2. Scroll hasta "Cross-origin resource sharing (CORS)"

#### 3. Click en "Edit" y pega:

```json
[
  {
    "AllowedHeaders": [
      "*"
    ],
    "AllowedMethods": [
      "GET",
      "PUT",
      "POST",
      "DELETE",
      "HEAD"
    ],
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200",
      "https://tudominio.com"
    ],
    "ExposeHeaders": [
      "ETag"
    ],
    "MaxAgeSeconds": 3000
  }
]
```

---

## ?? Estructura de Carpetas en S3

Después de la configuración, las imágenes se organizarán así:

```
feller-automotores/
??? autos/
?   ??? 1/
?   ?   ??? abc123-def456-ghi789.jpg
?   ?   ??? xyz789-abc123-def456.png
?   ??? 2/
?       ??? ...
??? motos/
    ??? 1/
    ?   ??? ...
    ??? 2/
        ??? ...
```

---

## ?? Alternativa: URLs Presignadas (Más Segura)

Si NO quieres hacer el bucket público, puedes usar URLs presignadas (temporales).

### Modificar ImagenService.cs

Reemplaza el método `GetImageUrl`:

```csharp
public string GetImageUrl(string key)
{
    // Generar URL presignada válida por 1 hora
    var request = new GetPreSignedUrlRequest
    {
    BucketName = _bucketName,
        Key = key,
        Expires = DateTime.UtcNow.AddHours(1)
    };
    
    return _s3Client.GetPreSignedURL(request);
}
```

**Ventajas**:
- ? No necesitas hacer el bucket público
- ? URLs expiran automáticamente
- ? Más seguro

**Desventajas**:
- ? Las URLs expiran (necesitas regenerarlas)
- ? Más procesamiento en el servidor

---

## ?? Probar la Configuración

### 1. Subir una imagen de prueba

```http
POST http://localhost:5000/api/autos/1/imagenes
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

file: [seleccionar archivo]
```

### 2. Verificar la URL en la respuesta

```json
{
  "success": true,
  "message": "Imagen subida exitosamente",
  "data": {
    "id": 1,
    "url": "https://feller-automotores.s3.sa-east-1.amazonaws.com/autos/1/abc123.jpg"
  }
}
```

### 3. Abrir la URL en el navegador

Deberías ver la imagen directamente.

---

## ??? Seguridad en Producción

### Recomendaciones:

1. **CloudFront** (CDN de AWS)
   - Mejora velocidad
   - Oculta el bucket real
   - Cache global

2. **Bucket privado + URLs presignadas**
   - Solo tu backend puede generar URLs
   - URLs temporales

3. **AWS WAF**
   - Protección contra ataques

---

## ?? Solución de Problemas

### Error: "Access Denied"
```
Causa: Block public access está activo
Solución: Desbloquear en "Permissions"
```

### Error: "NoSuchBucket"
```
Causa: Nombre de bucket incorrecto
Solución: Verificar en appsettings.json
```

### Error: "InvalidAccessKeyId"
```
Causa: Credenciales AWS incorrectas
Solución: Verificar AccessKey y SecretKey
```

### Las imágenes no se ven
```
Causa 1: Bucket policy no aplicada
Solución: Verificar paso 4

Causa 2: URL incorrecta
Solución: Verificar región en appsettings.json
```

---

## ?? Verificación Completa

### 1. Verificar appsettings.json

```json
{
  "AWS": {
    "Region": "sa-east-1",  // ? Tu región
    "AccessKey": "TU_AWS_ACCESS_KEY",
    "SecretKey": "TU_AWS_SECRET_KEY",
    "BucketName": "feller-automotores"  // ? Tu bucket
  }
}
```

### 2. Verificar en AWS S3 Console

- ? Bucket existe
- ? Block public access desactivado
- ? Bucket policy aplicada
- ? CORS configurado (opcional)

### 3. Verificar permisos del usuario IAM

El usuario con las credenciales debe tener:
- ? `s3:PutObject`
- ? `s3:GetObject`
- ? `s3:DeleteObject`
- ? `s3:ListBucket`

---

## ?? Resumen de la Solución

**Antes** (con error):
```csharp
var request = new PutObjectRequest
{
    BucketName = _bucketName,
    Key = key,
    InputStream = stream,
    ContentType = file.ContentType,
    CannedACL = S3CannedACL.PublicRead  // ? Causa error
};
```

**Después** (sin error):
```csharp
var request = new PutObjectRequest
{
    BucketName = _bucketName,
    Key = key,
    InputStream = stream,
    ContentType = file.ContentType
    // ? Sin CannedACL
};
```

**Y configurar Bucket Policy en AWS** ?

---

## ?? Ayuda Adicional

Si sigues teniendo problemas:

1. Verifica los logs de CloudWatch en AWS
2. Usa AWS CLI para probar:
```bash
aws s3 ls s3://feller-automotores/
aws s3 cp test.jpg s3://feller-automotores/test.jpg
```

3. Verifica que las credenciales sean correctas:
```bash
aws sts get-caller-identity
```

---

**Estado**: ? ImagenService corregido  
**Próximo paso**: Configurar Bucket Policy en AWS S3 Console
