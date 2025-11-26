# ?? CONFIGURACIÓN DE SECRETOS - Guía Completa

## ? PROBLEMA RESUELTO

Las claves de AWS y otras credenciales sensibles han sido removidas del repositorio Git usando **User Secrets** para desarrollo local.

---

## ??? CONFIGURACIÓN PARA DESARROLLO LOCAL

### 1. User Secrets (Ya Configurado)

Los siguientes secretos ya están configurados en tu máquina local:

```bash
? AWS:AccessKey
? AWS:SecretKey
? ConnectionStrings:DefaultConnection
? JwtSettings:Secret
```

### 2. Verificar Secretos Configurados

```bash
cd D:\repos\feller\backend\FellerBackend\FellerBackend
dotnet user-secrets list
```

### 3. Agregar Más Secretos (Si necesitas)

```bash
# WhatsApp (Twilio)
dotnet user-secrets set "WhatsApp:AccountSid" "TU_ACCOUNT_SID"
dotnet user-secrets set "WhatsApp:AuthToken" "TU_AUTH_TOKEN"
dotnet user-secrets set "WhatsApp:PhoneNumber" "TU_NUMERO"

# Email
dotnet user-secrets set "Email:Username" "tu_email@gmail.com"
dotnet user-secrets set "Email:Password" "tu_password"
dotnet user-secrets set "Email:FromEmail" "tu_email@gmail.com"
```

### 4. Eliminar un Secreto

```bash
dotnet user-secrets remove "AWS:AccessKey"
```

### 5. Limpiar Todos los Secretos

```bash
dotnet user-secrets clear
```

---

## ?? CONFIGURACIÓN PARA PRODUCCIÓN

### Opción 1: Variables de Entorno (Recomendado)

#### Railway / Render / Azure

```bash
# Base de Datos
ConnectionStrings__DefaultConnection="Host=...;Port=...;Database=...;Username=...;Password=..."

# JWT
JwtSettings__Secret="TU_SECRET_KEY_32_CARACTERES_MINIMO"

# AWS S3
AWS__AccessKey="TU_AWS_ACCESS_KEY"
AWS__SecretKey="TU_AWS_SECRET_KEY"
AWS__Region="sa-east-1"
AWS__BucketName="feller-automotores"

# Email (Opcional)
Email__Username="tu_email@gmail.com"
Email__Password="tu_password"
Email__FromEmail="tu_email@gmail.com"

# WhatsApp (Opcional)
WhatsApp__AccountSid="TU_ACCOUNT_SID"
WhatsApp__AuthToken="TU_AUTH_TOKEN"
WhatsApp__PhoneNumber="TU_NUMERO"
```

**Nota**: En variables de entorno se usa `__` (doble guion bajo) en lugar de `:`.

#### Docker

Crear archivo `.env` (NO commitear al repo):

```env
ConnectionStrings__DefaultConnection=Host=...;Port=...
JwtSettings__Secret=...
AWS__AccessKey=...
AWS__SecretKey=...
```

Usar en `docker-compose.yml`:

```yaml
services:
  api:
    image: feller-backend
 env_file:
      - .env
    # O específicamente:
    environment:
      - ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
      - AWS__AccessKey=${AWS__AccessKey}
```

### Opción 2: Azure Key Vault

```csharp
// Program.cs
var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
```

### Opción 3: AWS Secrets Manager

```bash
dotnet add package AWSSDK.SecretsManager
```

```csharp
// Program.cs
var secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.SAEast1);
var request = new GetSecretValueRequest { SecretId = "feller-automotores-secrets" };
var response = await secretsManager.GetSecretValueAsync(request);
```

---

## ?? ARCHIVOS DEL PROYECTO

### Estructura de Configuración

```
FellerBackend/
??? appsettings.json      ? Commiteado (sin secretos)
??? appsettings.Development.json  ? Commiteado (sin secretos)
??? appsettings.Production.json         ? NO commitear (en .gitignore)
??? appsettings.example.json       ? Commiteado (ejemplo para el equipo)
??? .env.example     ? Commiteado (ejemplo para producción)
??? .env         ? NO commitear (en .gitignore)
??? FellerBackend.csproj   ? Contiene UserSecretsId
```

### appsettings.json (Valores Públicos)

```json
{
  "Logging": { ... },
  "AllowedHosts": "*",
  "JwtSettings": {
    "Issuer": "FellerAutomotores",
    "Audience": "FellerAutomotoresApp",
    "ExpirationHours": 24
  },
  "AWS": {
    "Region": "sa-east-1",
    "BucketName": "feller-automotores"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
    "FromName": "Feller Automotores"
  }
}
```

---

## ?? CONFIGURACIÓN PARA NUEVOS DESARROLLADORES

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/Juan5eLuques/FellerBackend.git
cd FellerBackend/FellerBackend
```

### Paso 2: Copiar Archivo de Ejemplo

```bash
# NO ES NECESARIO, solo de referencia
# appsettings.example.json ya muestra qué configurar
```

### Paso 3: Configurar User Secrets

```bash
# Inicializar (si no existe)
dotnet user-secrets init

# Agregar secretos
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=feller_db;Username=postgres;Password=TU_PASSWORD"

dotnet user-secrets set "JwtSettings:Secret" "TU_SECRET_KEY_MINIMO_32_CARACTERES"

dotnet user-secrets set "AWS:AccessKey" "TU_AWS_ACCESS_KEY"
dotnet user-secrets set "AWS:SecretKey" "TU_AWS_SECRET_KEY"
```

### Paso 4: Verificar Configuración

```bash
dotnet user-secrets list
```

### Paso 5: Ejecutar

```bash
dotnet run
```

---

## ?? VERIFICAR QUE TODO FUNCIONA

### Test 1: Verificar Conexión a BD

El proyecto debería conectarse usando los secretos:

```
? Conexión a PostgreSQL exitosa
   Base de datos: feller_db
```

### Test 2: Verificar AWS S3

```bash
# Probar subir imagen en Swagger
POST /api/imagenes/autos/1
```

Si funciona, los secretos están correctos.

### Test 3: Verificar JWT

```bash
# Login en Swagger
POST /api/auth/login
```

Si genera token, el secret está correcto.

---

## ?? SEGURIDAD

### ? QUÉ SÍ COMMITEAR

- `appsettings.json` (sin secretos)
- `appsettings.Development.json` (sin secretos)
- `appsettings.example.json` (plantilla)
- `.env.example` (plantilla)
- `.gitignore` (protección)

### ? QUÉ NO COMMITEAR

- `appsettings.Production.json`
- `appsettings.Staging.json`
- `.env`
- `.env.local`
- Cualquier archivo con credenciales reales

### ?? BUENAS PRÁCTICAS

1. **Nunca** hacer commit de credenciales
2. **Rotar** claves comprometidas inmediatamente
3. **Usar** diferentes claves para dev/staging/prod
4. **Revisar** `.gitignore` antes de cada commit
5. **Usar** Git hooks para prevenir commits accidentales

---

## ?? MIGRAR CREDENCIALES EXISTENTES

Si ya commiteaste credenciales por error:

### 1. Eliminar del Historial

```bash
# ADVERTENCIA: Esto reescribe el historial de Git
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch appsettings.json" \
  --prune-empty --tag-name-filter cat -- --all

# Forzar push
git push origin --force --all
```

### 2. Rotar Credenciales

**AWS**:
1. Ir a AWS Console ? IAM ? Users
2. Eliminar AccessKey comprometida
3. Crear nueva AccessKey
4. Actualizar en User Secrets

**JWT**:
1. Generar nuevo secret
2. Actualizar en User Secrets
3. Los usuarios existentes deberán re-loguearse

---

## ?? PRIORIDAD DE CONFIGURACIÓN

.NET carga la configuración en este orden (el último sobrescribe):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. **User Secrets** (solo en Development)
4. **Variables de Entorno**
5. Argumentos de línea de comandos

**Ejemplo**:
```
appsettings.json:     AWS:Region = "sa-east-1"
User Secrets:         AWS:AccessKey = "AKIAXXX..."
Variables Entorno:    AWS__SecretKey = "secret123"
Resultado final:  Todas las configuraciones combinadas
```

---

## ??? TROUBLESHOOTING

### "No se pudo conectar a PostgreSQL"

```bash
# Verificar que el secreto está configurado
dotnet user-secrets list

# Ver ConnectionStrings:DefaultConnection
# Debe tener tu password correcto
```

### "Access Denied" en S3

```bash
# Verificar AWS secrets
dotnet user-secrets list

# Verificar que AccessKey y SecretKey son correctos
# Verificar permisos en AWS IAM
```

### "JWT Invalid" o "Unauthorized"

```bash
# Verificar JwtSettings:Secret
dotnet user-secrets list

# Debe tener mínimo 32 caracteres
# Si cambiaste el secret, debes re-loguearte
```

### "User secrets not found"

```bash
# Inicializar
dotnet user-secrets init

# Verificar que .csproj tiene UserSecretsId
cat FellerBackend.csproj | grep UserSecretsId
```

---

## ?? CONTACTO

Si necesitas las credenciales reales para desarrollo:
- Contactar al administrador del proyecto
- Solicitar acceso a AWS IAM (crear tu propio usuario)
- Solicitar string de conexión a BD de desarrollo

**NUNCA** compartir credenciales por:
- ? Email
- ? Slack/Discord/WhatsApp
- ? Repositorios Git
- ? Documentos compartidos

? Usar: Password managers (1Password, LastPass) o secrets managers

---

## ? RESUMEN

### Para Desarrollar Localmente

1. ? Clonar repo
2. ? Ejecutar `dotnet user-secrets init`
3. ? Configurar secretos con `dotnet user-secrets set`
4. ? Ejecutar `dotnet run`

### Para Deploy en Producción

1. ? Configurar variables de entorno en el servidor
2. ? **NO** incluir credenciales en el código
3. ? Verificar que `.gitignore` protege archivos sensibles

---

**Última actualización**: Noviembre 2024  
**Versión**: 1.0  
**Estado**: ? Configurado correctamente
