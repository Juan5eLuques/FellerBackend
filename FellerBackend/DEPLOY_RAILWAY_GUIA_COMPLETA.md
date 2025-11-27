# ?? DEPLOY EN RAILWAY - Feller Automotores API

## ?? RESUMEN

Guía completa para desplegar la API de Feller Automotores en Railway usando Dockerfile.

---

## ? ARCHIVOS DE CONFIGURACIÓN

### 1. Dockerfile

```dockerfile
# ===== Base (runtime) =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivo de proyecto
COPY FellerBackend/FellerBackend.csproj FellerBackend/

# Restaurar dependencias
RUN dotnet restore FellerBackend/FellerBackend.csproj

# Copiar todo el código fuente
COPY . .

# Compilar y publicar
WORKDIR /src/FellerBackend
RUN dotnet publish FellerBackend.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# ?? Forzamos URLS y ASPNETCORE_URLS con el $PORT de Railway
ENTRYPOINT ["sh","-lc","URLS=http://0.0.0.0:${PORT:-8080} ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet FellerBackend.dll"]
```

### 2. railway.toml

```toml
[build]
builder = "dockerfile"
```

---

## ?? PASO 1: PREPARAR REPOSITORIO

### Verificar archivos

```bash
cd D:\repos\feller\backend\FellerBackend

# Verificar que existen
ls Dockerfile
ls railway.toml
ls FellerBackend/FellerBackend.csproj
```

### Commit y Push

```bash
git add Dockerfile railway.toml
git commit -m "chore: configuración Railway con Dockerfile"
git push origin master
```

---

## ?? PASO 2: CREAR PROYECTO EN RAILWAY

### 2.1 Ir a Railway

1. Abrir: https://railway.app/
2. Login con GitHub
3. Click en **"New Project"**

### 2.2 Conectar Repositorio

1. Seleccionar **"Deploy from GitHub repo"**
2. Buscar: `Juan5eLuques/FellerBackend`
3. Click en el repositorio
4. Railway detectará el `Dockerfile` automáticamente

### 2.3 Configurar Variables de Entorno

Click en el proyecto ? Variables ? **Add Variable**

#### Variables REQUERIDAS:

```bash
# ===== DATABASE (Railway PostgreSQL) =====
ConnectionStrings__DefaultConnection="Host=your-db-host.railway.app;Port=5432;Database=railway;Username=postgres;Password=your-password;SSL Mode=Require;Trust Server Certificate=true"

# ===== JWT =====
JwtSettings__Secret="FellerAutomotores2025_SuperSecretKey_MinimoDe32Caracteres!@#"
JwtSettings__Issuer="FellerAutomotores"
JwtSettings__Audience="FellerAutomotoresApp"
JwtSettings__ExpirationHours=24

# ===== AWS S3 =====
AWS__Region="sa-east-1"
AWS__AccessKey="NUEVA_AWS_ACCESS_KEY"
AWS__SecretKey="NUEVA_AWS_SECRET_KEY"
AWS__BucketName="feller-automotores"

# ===== ASPNETCORE =====
ASPNETCORE_ENVIRONMENT="Production"
```

#### Variables OPCIONALES (Twilio, Email):

```bash
# ===== WHATSAPP (Twilio) =====
WhatsApp__AccountSid="TU_TWILIO_ACCOUNT_SID"
WhatsApp__AuthToken="TU_TWILIO_AUTH_TOKEN"
WhatsApp__PhoneNumber="+1234567890"

# ===== EMAIL (SMTP) =====
Email__SmtpServer="smtp.gmail.com"
Email__SmtpPort=587
Email__Username="tu_email@gmail.com"
Email__Password="tu_app_password"
Email__FromEmail="tu_email@gmail.com"
Email__FromName="Feller Automotores"
```

---

## ??? PASO 3: CONFIGURAR BASE DE DATOS

### 3.1 Crear PostgreSQL en Railway

1. En tu proyecto, click **"+ New"**
2. Seleccionar **"Database"** ? **"Add PostgreSQL"**
3. Railway creará automáticamente la base de datos

### 3.2 Obtener Connection String

1. Click en la base de datos PostgreSQL
2. Tab **"Connect"**
3. Copiar **"Postgres Connection URL"**
4. Ejemplo:
   ```
   postgresql://postgres:password@containers-us-west-123.railway.app:5432/railway
   ```

### 3.3 Convertir a formato .NET

Convertir la URL de PostgreSQL al formato .NET:

```
postgresql://postgres:PASSWORD@HOST:PORT/DATABASE
```

Convertir a:

```
Host=HOST;Port=PORT;Database=DATABASE;Username=postgres;Password=PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

**Ejemplo**:

```bash
# Railway URL
postgresql://postgres:YubMlpAtULwv@containers-us-west-123.railway.app:5432/railway

# .NET Connection String
ConnectionStrings__DefaultConnection="Host=containers-us-west-123.railway.app;Port=5432;Database=railway;Username=postgres;Password=YubMlpAtULwv;SSL Mode=Require;Trust Server Certificate=true"
```

### 3.4 Agregar Variable de Entorno

En Railway ? Variables ? Add Variable:

```
Key: ConnectionStrings__DefaultConnection
Value: Host=...;Port=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true
```

---

## ?? PASO 4: DESPLEGAR

### 4.1 Trigger Deploy

Railway desplegará automáticamente cuando:
- Haces push a `master`
- Cambias variables de entorno
- Click manual en **"Deploy"**

### 4.2 Ver Logs

1. Click en el servicio de la API
2. Tab **"Deployments"**
3. Click en el deployment activo
4. Ver logs en tiempo real

### 4.3 Verificar Build

Busca en los logs:

```
? Build succeeded
? Starting deployment
? Service is live
```

---

## ??? PASO 5: APLICAR MIGRACIONES

### Opción A: Railway CLI (Recomendado)

```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Login
railway login

# Link al proyecto
railway link

# Aplicar migraciones
railway run dotnet ef database update --project FellerBackend/FellerBackend.csproj
```

### Opción B: Desde Local

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend

# Usar connection string de Railway
$env:ConnectionStrings__DefaultConnection="Host=...railway..."

dotnet ef database update
```

### Opción C: Seed Script SQL

Conectarse directamente a la BD de Railway y ejecutar:

```sql
-- Ver MIGRACION_PRODUCCION_RAILWAY.md para scripts
```

---

## ?? PASO 6: VERIFICAR DEPLOYMENT

### 6.1 Obtener URL del Servicio

1. Click en el servicio de la API
2. Tab **"Settings"**
3. Sección **"Domains"**
4. Copiar la URL (ejemplo: `feller-backend-production.up.railway.app`)

### 6.2 Probar Endpoints

```bash
# Health Check
curl https://feller-backend-production.up.railway.app/

# Swagger (si está habilitado)
https://feller-backend-production.up.railway.app/swagger

# Test API
curl https://feller-backend-production.up.railway.app/api/autos
```

### 6.3 Verificar Base de Datos

```bash
# Conectar a PostgreSQL
railway connect postgres

# O con psql
psql "postgresql://postgres:...@...railway.app:5432/railway"

# Verificar tablas
\dt

# Verificar migraciones
SELECT * FROM "__EFMigrationsHistory";
```

---

## ?? PASO 7: CONFIGURACIÓN ADICIONAL

### 7.1 Dominio Personalizado (Opcional)

1. Railway ? Settings ? Domains
2. Click **"Add Domain"**
3. Ingresar tu dominio: `api.fellerautomotores.com`
4. Agregar registro CNAME en tu DNS:
   ```
   CNAME api ? feller-backend-production.up.railway.app
   ```

### 7.2 CORS para Frontend

En Railway ? Variables ? Add Variable:

```bash
# Si tienes dominio personalizado para frontend
CORS__AllowedOrigins="https://fellerautomotores.com,https://www.fellerautomotores.com"
```

Actualizar `Program.cs` para leer esta variable:

```csharp
var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]?.Split(',') 
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
   .AllowAnyHeader()
 .AllowAnyMethod()
     .AllowCredentials();
    });
});
```

### 7.3 Health Checks

Agregar health check endpoint en `Program.cs`:

```csharp
app.MapHealthChecks("/health");
```

Railway usará este endpoint para verificar que el servicio está activo.

---

## ?? TROUBLESHOOTING

### Error: "Application failed to start"

**Causa**: Variables de entorno incorrectas

**Solución**:
1. Verificar todas las variables requeridas
2. Verificar formato de Connection String
3. Ver logs: Railway ? Deployments ? View Logs

### Error: "Port already in use"

**Causa**: El puerto no se está configurando correctamente

**Solución**:
Verificar que el `ENTRYPOINT` en Dockerfile use `$PORT`:

```dockerfile
ENTRYPOINT ["sh","-lc","URLS=http://0.0.0.0:${PORT:-8080} ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet FellerBackend.dll"]
```

### Error: "Database connection failed"

**Causa**: Connection String incorrecto

**Solución**:
1. Verificar Connection String en Variables
2. Debe incluir: `SSL Mode=Require;Trust Server Certificate=true`
3. Probar conexión desde local:
   ```bash
   psql "Host=...;Port=...;Database=...;Username=...;Password=..."
   ```

### Error: "S3 Access Denied"

**Causa**: Credenciales AWS incorrectas

**Solución**:
1. Verificar `AWS__AccessKey` y `AWS__SecretKey`
2. Verificar permisos IAM en AWS Console
3. Probar upload desde local con las mismas credenciales

### Build Fails

**Causa**: Dependencias o configuración incorrecta

**Solución**:
1. Verificar que `Dockerfile` esté en la raíz del repo
2. Verificar que `FellerBackend.csproj` exista
3. Probar build local:
   ```bash
   docker build -t feller-api -f Dockerfile .
   docker run -p 8080:8080 feller-api
   ```

---

## ?? MONITOREO

### Ver Logs en Tiempo Real

```bash
railway logs
```

### Ver Métricas

Railway ? Metrics:
- CPU Usage
- Memory Usage
- Network I/O
- Request Rate

### Alertas

Railway ? Settings ? Notifications:
- Deploy Success/Failure
- Service Down
- High Resource Usage

---

## ?? COSTOS ESTIMADOS

### Railway Free Tier

- ? $5 USD de crédito gratis/mes
- ? 500 horas de ejecución
- ? PostgreSQL incluido

### Railway Pro (si necesitas más)

- ?? $20 USD/mes
- ?? Recursos ilimitados
- ?? Más proyectos
- ?? Soporte prioritario

### Estimación para Feller Automotores

```
API (.NET 8): ~100 horas/mes ? $2-3 USD
PostgreSQL: Incluido ? $0 USD
Bandwidth: ~5GB/mes ? $0 USD (dentro del límite)

Total estimado: $2-3 USD/mes (con Free Tier)
```

---

## ?? SEGURIDAD

### 1. Rotar Secretos Regularmente

```bash
# Cada 3 meses, cambiar:
- JwtSettings__Secret
- AWS__AccessKey y AWS__SecretKey
- Database Password
```

### 2. Usar Secrets Management

Railway ? Variables ? Variables (no Raw Environment)

### 3. HTTPS Only

Railway proporciona HTTPS automáticamente. **NO** deshabilitar.

### 4. Firewall

Railway maneja esto automáticamente. Solo expondrás:
- Puerto 8080 (API)
- Puerto 5432 (PostgreSQL, solo interno)

---

## ? CHECKLIST DE DEPLOYMENT

### Pre-Deploy
- [ ] Dockerfile creado y testeado
- [ ] railway.toml configurado
- [ ] Variables de entorno preparadas
- [ ] Connection String de Railway obtenido
- [ ] AWS credentials rotadas y válidas
- [ ] Código commiteado y pusheado

### Deploy
- [ ] Proyecto creado en Railway
- [ ] Repositorio conectado
- [ ] PostgreSQL agregado
- [ ] Variables de entorno configuradas
- [ ] Deploy exitoso
- [ ] Logs verificados sin errores

### Post-Deploy
- [ ] Migraciones aplicadas
- [ ] Endpoints de prueba funcionando
- [ ] Swagger accesible (si está habilitado)
- [ ] Base de datos con datos iniciales (seed)
- [ ] S3 upload funcional
- [ ] Dominio personalizado configurado (opcional)

---

## ?? SOPORTE

### Railway
- Docs: https://docs.railway.app/
- Discord: https://discord.gg/railway
- Status: https://status.railway.app/

### .NET + Docker
- Docs: https://learn.microsoft.com/en-us/dotnet/core/docker/

### AWS S3
- Docs: https://docs.aws.amazon.com/s3/

---

## ?? RESUMEN

| Paso | Descripción | Estado |
|------|-------------|--------|
| 1 | Preparar repo | ? Completado |
| 2 | Crear proyecto Railway | ? Pendiente |
| 3 | Configurar PostgreSQL | ? Pendiente |
| 4 | Desplegar | ? Pendiente |
| 5 | Aplicar migraciones | ? Pendiente |
| 6 | Verificar deployment | ? Pendiente |
| 7 | Configuración adicional | ? Opcional |

---

**Versión**: 1.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Listo para deploy
