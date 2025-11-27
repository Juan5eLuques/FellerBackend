# ?? CONFIGURACIÓN CORS PARA VERCEL + RAILWAY

## ?? PROBLEMA

```
Access to XMLHttpRequest at 'https://fellerbackend-production.up.railway.app/api/autos' 
from origin 'https://feller-jdc48qjfr-juan5eluques-projects.vercel.app' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present
```

## ? SOLUCIÓN

### 1. Agregar Variable de Entorno en Railway

Ve a Railway ? Tu Proyecto ? API Service ? **Variables** ? **+ New Variable**

```
Key: CORS__AllowedOrigins
Value: https://feller-jdc48qjfr-juan5eluques-projects.vercel.app,https://feller.vercel.app
```

**IMPORTANTE**: 
- Separa múltiples dominios con `,` (coma)
- NO agregues espacios
- Incluye HTTPS (no HTTP)
- Incluye todos los dominios de Vercel (preview y production)

---

## ?? EJEMPLOS DE CONFIGURACIÓN

### Solo un dominio

```
CORS__AllowedOrigins=https://feller-jdc48qjfr-juan5eluques-projects.vercel.app
```

### Múltiples dominios (Preview + Production)

```
CORS__AllowedOrigins=https://feller-jdc48qjfr-juan5eluques-projects.vercel.app,https://feller.vercel.app,https://www.fellerautomotores.com
```

### Con dominio personalizado

```
CORS__AllowedOrigins=https://fellerautomotores.com,https://www.fellerautomotores.com,https://feller.vercel.app
```

---

## ?? PASOS COMPLETOS

### Paso 1: Obtener URL de Vercel

1. Ir a tu proyecto en Vercel
2. Tab **"Deployments"**
3. Copiar la URL del deployment activo
4. Ejemplo: `https://feller-jdc48qjfr-juan5eluques-projects.vercel.app`

### Paso 2: Agregar Variable en Railway

1. Ir a Railway ? Tu Proyecto
2. Click en el servicio de la API
3. Tab **"Variables"**
4. Click **"+ New Variable"**
5. Agregar:
 ```
   Key: CORS__AllowedOrigins
   Value: https://feller-jdc48qjfr-juan5eluques-projects.vercel.app
   ```
6. Click **"Add"**

### Paso 3: Redeploy Automático

Railway redeployará automáticamente después de agregar la variable.

### Paso 4: Verificar

1. Espera 1-2 minutos
2. Prueba desde tu frontend en Vercel
3. El error de CORS debería desaparecer

---

## ?? TESTING

### Verificar CORS desde el Navegador

```javascript
// En DevTools Console del frontend
fetch('https://fellerbackend-production.up.railway.app/api/autos')
  .then(res => res.json())
  .then(console.log)
  .catch(console.error);
```

Si funciona ? ? CORS configurado correctamente  
Si falla ? ? Revisar configuración

### Verificar Headers CORS

```bash
curl -I -X OPTIONS \
  -H "Origin: https://feller-jdc48qjfr-juan5eluques-projects.vercel.app" \
  -H "Access-Control-Request-Method: GET" \
  https://fellerbackend-production.up.railway.app/api/autos
```

Deberías ver:
```
Access-Control-Allow-Origin: https://feller-jdc48qjfr-juan5eluques-projects.vercel.app
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, PATCH, OPTIONS
Access-Control-Allow-Headers: *
Access-Control-Allow-Credentials: true
```

---

## ?? CONFIGURACIÓN ACTUAL

### Código en Program.cs

```csharp
// CORS lee desde configuración
var allowedOrigins = new List<string>
{
    // Localhost siempre permitido (desarrollo)
    "http://localhost:3000",
    "http://localhost:5173",
    // ... otros localhost
};

// Agregar dominios de producción desde variable de entorno
var productionOrigins = builder.Configuration["CORS:AllowedOrigins"];
if (!string.IsNullOrEmpty(productionOrigins))
{
    allowedOrigins.AddRange(productionOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries));
}

policy.WithOrigins(allowedOrigins.ToArray())
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
```

**Ventajas**:
- ? Localhost siempre funciona (desarrollo)
- ? Dominios de producción desde variable de entorno
- ? Fácil agregar/quitar dominios sin redeployar código
- ? Soporta múltiples dominios

---

## ?? TROUBLESHOOTING

### Error persiste después de agregar variable

1. **Verificar que Railway redeployó**:
   - Railway ? Deployments
 - Ver que hay un nuevo deployment después de agregar la variable

2. **Verificar formato de variable**:
   ```
   ? CORRECTO: CORS__AllowedOrigins
   ? INCORRECTO: CORS:AllowedOrigins (Railway usa __)
   
   ? CORRECTO: https://feller.vercel.app
   ? INCORRECTO: http://feller.vercel.app (debe ser HTTPS)
   
   ? CORRECTO: domain1.com,domain2.com
   ? INCORRECTO: domain1.com, domain2.com (sin espacios)
   ```

3. **Limpiar cache del navegador**:
   ```
   Chrome/Edge: Ctrl + Shift + Delete
   Firefox: Ctrl + Shift + Delete
   Safari: Cmd + Option + E
   ```

4. **Verificar que no hay typos en la URL de Vercel**:
   - Copiar directamente desde Vercel
   - NO escribir manualmente

### Error: "...does not match the request origin"

**Causa**: La URL en CORS no coincide exactamente con el origin del request.

**Solución**: Asegurarse que:
- Protocolo correcto (https://)
- No hay trailing slash (/)
- No hay subdominios faltantes (www. si aplica)

### Error: "Credentials flag is 'true', but the 'Access-Control-Allow-Credentials' header is ''"

**Causa**: El servidor no está enviando el header `Access-Control-Allow-Credentials`.

**Solución**: Ya está configurado en el código con `.AllowCredentials()`.

---

## ?? RESUMEN

| Item | Valor |
|------|-------|
| Variable Railway | `CORS__AllowedOrigins` |
| Valor ejemplo | `https://feller.vercel.app` |
| Múltiples dominios | Separar con `,` |
| Redeploy | Automático al agregar variable |
| Testing | DevTools Console del frontend |

---

## ? CHECKLIST

- [ ] Obtener URL exacta de Vercel
- [ ] Agregar variable `CORS__AllowedOrigins` en Railway
- [ ] Esperar redeploy automático (1-2 min)
- [ ] Probar desde frontend en Vercel
- [ ] Verificar en DevTools que no hay error CORS
- [ ] (Opcional) Agregar dominio personalizado cuando esté listo

---

**Versión**: 1.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Implementado - Pendiente configurar variable en Railway
