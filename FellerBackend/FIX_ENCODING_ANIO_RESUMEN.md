# ? FIX ENCODING: Año ? Anio

## ?? PROBLEMA RESUELTO

Railway rechazaba el build por caracteres especiales (eñe) en el código que causaban errores de encoding UTF-8 en Docker.

---

## ?? CAMBIOS REALIZADOS

### 1. Modelos y DTOs

? **VehiculoBase.cs**: `public int Año` ? `public int Anio`  
? **CreateAutoDto.cs**: `public int Año` ? `public int Anio`  
? **UpdateAutoDto.cs**: `public int? Año` ? `public int? Anio`  
? **AutoDto.cs**: `public int Año` ? `public int Anio`  
? **CreateMotoDto.cs**: `public int Año` ? `public int Anio`  
? **UpdateMotoDto.cs**: `public int? Año` ? `public int? Anio`  
? **MotoDto.cs**: `public int Año` ? `public int Anio`  

### 2. Services

? **VehiculoService.cs**: Todas las referencias a `Año` reemplazadas por `Anio` en:
  - `GetAllAutosAsync()`
  - `GetAutoByIdAsync()`
  - `CreateAutoAsync()`
  - `UpdateAutoAsync()`
  - `GetAllMotosAsync()`
  - `GetMotoByIdAsync()`
  - `CreateMotoAsync()`
  - `UpdateMotoAsync()`
  - `GetAutosDestacadosAsync()`
  - `GetMotosDestacadasAsync()`
  - `MarcarAutoComoDestacadoAsync()`
  - `MarcarMotoComoDestacadaAsync()`
  - `DesmarcarAutoDestacadoAsync()`
  - `DesmarcarMotoDestacadaAsync()`

### 3. Controllers

? **AutosController.cs**: Ya usaba `dto.Anio`  
? **MotosController.cs**: Ya usaba `dto.Anio`  
? **SeedController.cs**: Pendiente actualizar (si existe)

---

## ??? MIGRACIÓN DE BASE DE DATOS

### Base de Datos LOCAL (ya aplicada)

La columna se llama "Año" en las migraciones existentes. No necesitas cambiarla en local si ya funciona.

### Base de Datos RAILWAY (Producción)

**IMPORTANTE**: Debes renombrar la columna manualmente:

```sql
ALTER TABLE "Vehiculos" RENAME COLUMN "Año" TO "Anio";
```

Ver `MIGRACION_RENOMBRAR_ANIO.md` para instrucciones detalladas.

---

## ?? TESTING

### Verificar Compilación

```bash
dotnet build
# Debe compilar sin errores
```

### Verificar que Railway acepta el build

Railway ahora puede compilar sin errores de encoding porque:
- ? No hay caracteres `ñ` en código
- ? Solo comentarios tienen caracteres especiales (se ignoran en build)
- ? Todas las propiedades usan ASCII

---

## ?? DEPLOY EN RAILWAY

1. ? Push a GitHub (ya hecho)
2. ? Railway detecta cambios automáticamente
3. ? Build debería completarse sin errores
4. ??  **IMPORTANTE**: Aplicar migración SQL en la BD de Railway:

```bash
railway connect postgres

ALTER TABLE "Vehiculos" RENAME COLUMN "Año" TO "Anio";
```

---

## ?? NOTA PARA EL FRONTEND

El frontend debe usar `anio` (sin eñe) en los requests:

```javascript
// Crear auto
{
  "marca": "Toyota",
  "modelo": "Corolla",
  "anio": 2024,  // ? Sin eñe
  "precio": 35000,
  ...
}
```

---

## ? CHECKLIST POST-FIX

- [x] Código actualizado (Año ? Anio)
- [x] Push a GitHub
- [ ] Deploy en Railway sin errores
- [ ] Aplicar migración SQL en Railway BD
- [ ] Probar endpoints en Railway
- [ ] Actualizar Frontend para usar `anio`

---

**Versión**: 1.0  
**Fecha**: Noviembre 2024  
**Estado**: ? Código corregido - Pendiente migración BD Railway
