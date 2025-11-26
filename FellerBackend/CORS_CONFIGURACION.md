# ?? SOLUCIÓN CORS - Feller Automotores API

## ? ERROR COMÚN

```
Access to fetch at 'http://localhost:5000/api/...' from origin 'http://localhost:3000' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present 
on the requested resource.
```

---

## ? SOLUCIÓN APLICADA

He configurado CORS de dos maneras:

### 1. Desarrollo (Más Permisivo)
En **modo desarrollo**, se permite **cualquier origen**:

```csharp
// Program.cs
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll"); // Permite CUALQUIER origen
}
```

### 2. Producción (Restrictivo)
En **producción**, solo orígenes específicos:

```csharp
else
{
    app.UseCors("AllowFrontend"); // Solo orígenes permitidos
}
```

---

## ?? CONFIGURACIÓN ACTUAL

### Orígenes Permitidos en Producción

- ? `http://localhost:3000` - React (Create React App)
- ? `http://localhost:5173` - Vite
- ? `http://localhost:4200` - Angular
- ? `http://localhost:8080` - Vue/otros
- ? `http://localhost:5000` - Mismo origen
- ? `http://127.0.0.1:*` - Variantes con IP

### Headers Permitidos
- ? **Todos** (`AllowAnyHeader`)

### Métodos Permitidos
- ? **Todos** (GET, POST, PUT, DELETE, PATCH, OPTIONS)

### Credenciales
- ? **Sí** (`AllowCredentials`) - Permite cookies y headers de autenticación

---

## ?? PROBAR QUE FUNCIONA

### Desde el Frontend (JavaScript)

```javascript
// Test básico de CORS
fetch('http://localhost:5000/api/autos')
  .then(response => response.json())
  .then(data => console.log('? CORS funciona:', data))
  .catch(error => console.error('? Error CORS:', error));

// Test con autenticación
fetch('http://localhost:5000/api/auth/me', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  credentials: 'include' // Importante para CORS con credenciales
})
  .then(response => response.json())
.then(data => console.log('? Auth funciona:', data))
  .catch(error => console.error('? Error:', error));
```

### Desde Axios (React/Vue)

```javascript
import axios from 'axios';

// Configurar base URL
axios.defaults.baseURL = 'http://localhost:5000/api';
axios.defaults.withCredentials = true; // Importante para CORS

// Interceptor para agregar token
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
  config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Test
axios.get('/autos')
  .then(response => console.log('? Funciona:', response.data))
  .catch(error => console.error('? Error:', error));
```

### Desde Angular (HttpClient)

```typescript
import { HttpClient, HttpHeaders } from '@angular/common/http';

constructor(private http: HttpClient) {}

testCors() {
  const headers = new HttpHeaders({
    'Authorization': `Bearer ${this.token}`
  });

  this.http.get('http://localhost:5000/api/autos', { 
    headers,
    withCredentials: true 
  })
  .subscribe(
    data => console.log('? Funciona:', data),
    error => console.error('? Error:', error)
  );
}
```

---

## ?? SI SIGUE SIN FUNCIONAR

### 1. Verificar que el Backend está corriendo

```bash
# Debe estar corriendo en http://localhost:5000
dotnet run
```

### 2. Verificar el puerto del Frontend

Si tu frontend corre en un puerto diferente (ej: 8080), agrégalo a Program.cs:

```csharp
policy.WithOrigins(
    "http://localhost:3000",
    "http://localhost:8080", // Agregar tu puerto aquí
    // ... otros puertos
)
```

### 3. Limpiar caché del navegador

```
1. Presiona F12 (DevTools)
2. Click derecho en el botón de recargar
3. Selecciona "Vaciar caché y recargar de forma forzada"
```

### 4. Verificar en las DevTools

Abre la consola del navegador (F12) y busca:

**? Error CORS**:
```
Access-Control-Allow-Origin
```

**? Sin error CORS**:
No debería aparecer ningún mensaje de CORS

### 5. Verificar Headers en Network

En la pestaña Network (F12):

1. Busca la request que falla
2. Ve a la pestaña "Headers"
3. Verifica:
   - **Request Headers**: Debe tener `Origin: http://localhost:XXXX`
   - **Response Headers**: Debe tener `Access-Control-Allow-Origin: *` (en desarrollo)

---

## ?? REINICIAR DESPUÉS DE CAMBIOS

Después de modificar la configuración de CORS:

```bash
# 1. Detener el proyecto
Ctrl + C

# 2. Limpiar build
dotnet clean

# 3. Compilar
dotnet build

# 4. Ejecutar
dotnet run
```

---

## ?? CONFIGURACIÓN COMPLETA

### Program.cs

```csharp
// ========================================
// CONFIGURACIÓN DE CORS
// ========================================
builder.Services.AddCors(options =>
{
    // Política específica para frontend conocido
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
     "http://localhost:3000",
            "http://localhost:5173", 
         "http://localhost:4200",
   "http://localhost:5000",
            "https://localhost:5000",
        "http://localhost:8080",
       "http://127.0.0.1:3000",
  "http://127.0.0.1:5173",
 "http://127.0.0.1:4200",
"http://127.0.0.1:5000"
        )
      .AllowAnyHeader()
      .AllowAnyMethod()
     .AllowCredentials()
        .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

    // Política permisiva para desarrollo
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
     .AllowAnyMethod();
    });
});

// ... en el middleware:

// IMPORTANTE: El orden es crítico
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll"); // ? Desarrollo: permite todo
}
else
{
    app.UseCors("AllowFrontend"); // ? Producción: restrictivo
}

app.UseAuthentication();
app.UseAuthorization();
```

---

## ?? PRODUCCIÓN

### Antes de Deploy

**?? IMPORTANTE**: Cambiar a política restrictiva:

```csharp
// Reemplazar dominios reales
policy.WithOrigins(
    "https://tudominio.com",
 "https://www.tudominio.com",
    "https://app.tudominio.com"
)
```

---

## ??? TROUBLESHOOTING

### Error: "Credentials flag is 'true', but 'Access-Control-Allow-Credentials' is not"

**Solución**: Verificar que tienes `.AllowCredentials()` en la política

### Error: "Origin 'http://localhost:XXXX' is not allowed"

**Solución**: Agregar tu puerto a `WithOrigins()`

### Error: Preflight request fails (OPTIONS)

**Solución**: Verificar que `.AllowAnyMethod()` está presente

### No funciona en Producción

**Solución**: Cambiar de `AllowAll` a `AllowFrontend` con dominios reales

---

## ?? DIFERENCIAS DESARROLLO vs PRODUCCIÓN

| Aspecto | Desarrollo | Producción |
|---------|-----------|------------|
| **Política** | `AllowAll` | `AllowFrontend` |
| **Orígenes** | Cualquiera (`*`) | Lista específica |
| **Seguridad** | ?? Baja | ? Alta |
| **Debugging** | ? Fácil | Medio |

---

## ? CHECKLIST POST-CAMBIOS

- [ ] Backend reiniciado (`dotnet run`)
- [ ] Frontend reiniciado
- [ ] Caché del navegador limpio
- [ ] DevTools abierto (F12) para ver errores
- [ ] Network tab para verificar headers
- [ ] Test con endpoint público (`GET /api/autos`)
- [ ] Test con endpoint protegido (`GET /api/auth/me`)

---

## ?? ESTADO ACTUAL

? **CORS configurado correctamente**
- ? Desarrollo: Permite cualquier origen
- ? Producción: Lista blanca de orígenes
- ? Headers: Todos permitidos
- ? Métodos: Todos permitidos
- ? Credenciales: Habilitadas

**Próximo paso**: Reiniciar el backend y probar desde el frontend

---

**Versión**: 1.2.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Configurado
