using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.DTOs.Autos;
using FellerBackend.DTOs.Motos;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class VehiculoService : IVehiculoService
{
    private readonly FellerDbContext _context;
    private readonly IImagenService _imagenService;

    public VehiculoService(FellerDbContext context, IImagenService imagenService)
    {
     _context = context;
  _imagenService = imagenService;
    }

    // ==================== AUTOS ====================

    public async Task<List<AutoDto>> GetAllAutosAsync()
    {
        return await _context.Autos
   .Include(a => a.Imagenes)
 .Select(a => new AutoDto
   {
         Id = a.Id,
      Marca = a.Marca,
           Modelo = a.Modelo,
    Anio = a.Anio,
           Precio = a.Precio,
     Descripcion = a.Descripcion,
      Disponible = a.Disponible,
          Estado = a.Estado,
        FechaPublicacion = a.FechaPublicacion,
     Puertas = a.Puertas,
     TipoCombustible = a.TipoCombustible,
     Transmision = a.Transmision,
  Kilometraje = a.Kilometraje,
        EsDestacado = a.EsDestacado,
       OrdenDestacado = a.OrdenDestacado,
      Imagenes = a.Imagenes.Select(i => new DTOs.Autos.ImagenDto
     {
    Id = i.Id,
  Url = i.Url
    }).ToList()
 })
     .ToListAsync();
    }

    public async Task<AutoDto?> GetAutoByIdAsync(int id)
    {
  var auto = await _context.Autos
     .Include(a => a.Imagenes)
      .FirstOrDefaultAsync(a => a.Id == id);

  if (auto == null)
return null;

        return new AutoDto
        {
  Id = auto.Id,
      Marca = auto.Marca,
 Modelo = auto.Modelo,
   Anio = auto.Anio,
Precio = auto.Precio,
    Descripcion = auto.Descripcion,
    Disponible = auto.Disponible,
         Estado = auto.Estado,
 FechaPublicacion = auto.FechaPublicacion,
   Puertas = auto.Puertas,
   TipoCombustible = auto.TipoCombustible,
     Transmision = auto.Transmision,
  Kilometraje = auto.Kilometraje,
 EsDestacado = auto.EsDestacado,
  OrdenDestacado = auto.OrdenDestacado,
          Imagenes = auto.Imagenes.Select(i => new DTOs.Autos.ImagenDto
        {
      Id = i.Id,
    Url = i.Url
    }).ToList()
    };
    }

    public async Task<AutoDto> CreateAutoAsync(CreateAutoDto dto)
    {
var auto = new Auto
        {
  Marca = dto.Marca,
       Modelo = dto.Modelo,
 Anio = dto.Anio,
       Precio = dto.Precio,
     Descripcion = dto.Descripcion,
        Disponible = dto.Disponible,
      Estado = dto.Estado,
       FechaPublicacion = DateTime.UtcNow,
    Puertas = dto.Puertas,
   TipoCombustible = dto.TipoCombustible,
 Transmision = dto.Transmision,
   Kilometraje = dto.Kilometraje
};

   _context.Autos.Add(auto);
    await _context.SaveChangesAsync();

     return new AutoDto
        {
     Id = auto.Id,
  Marca = auto.Marca,
   Modelo = auto.Modelo,
  Anio = auto.Anio,
   Precio = auto.Precio,
   Descripcion = auto.Descripcion,
    Disponible = auto.Disponible,
  Estado = auto.Estado,
      FechaPublicacion = auto.FechaPublicacion,
       Puertas = auto.Puertas,
       TipoCombustible = auto.TipoCombustible,
 Transmision = auto.Transmision,
Kilometraje = auto.Kilometraje,
    Imagenes = new List<DTOs.Autos.ImagenDto>()
};
    }

    public async Task<AutoDto> UpdateAutoAsync(int id, UpdateAutoDto dto)
    {
  var auto = await _context.Autos
            .Include(a => a.Imagenes)
  .FirstOrDefaultAsync(a => a.Id == id);

      if (auto == null)
     throw new KeyNotFoundException($"Auto con ID {id} no encontrado");

      // Actualizar solo los campos proporcionados
      if (!string.IsNullOrWhiteSpace(dto.Marca))
      auto.Marca = dto.Marca;

    if (!string.IsNullOrWhiteSpace(dto.Modelo))
  auto.Modelo = dto.Modelo;

      if (dto.Anio.HasValue)
 auto.Anio = dto.Anio.Value;

        if (dto.Precio.HasValue)
  auto.Precio = dto.Precio.Value;

        if (dto.Descripcion != null)
auto.Descripcion = dto.Descripcion;

     if (dto.Disponible.HasValue)
   auto.Disponible = dto.Disponible.Value;

        if (!string.IsNullOrWhiteSpace(dto.Estado))
       auto.Estado = dto.Estado;

        if (dto.Puertas.HasValue)
       auto.Puertas = dto.Puertas.Value;

   if (!string.IsNullOrWhiteSpace(dto.TipoCombustible))
  auto.TipoCombustible = dto.TipoCombustible;

if (dto.Transmision != null)
     auto.Transmision = dto.Transmision;

   if (dto.Kilometraje.HasValue)
         auto.Kilometraje = dto.Kilometraje;

   await _context.SaveChangesAsync();

   return new AutoDto
        {
  Id = auto.Id,
    Marca = auto.Marca,
  Modelo = auto.Modelo,
   Anio = auto.Anio,
          Precio = auto.Precio,
      Descripcion = auto.Descripcion,
   Disponible = auto.Disponible,
      Estado = auto.Estado,
         FechaPublicacion = auto.FechaPublicacion,
    Puertas = auto.Puertas,
            TipoCombustible = auto.TipoCombustible,
      Transmision = auto.Transmision,
     Kilometraje = auto.Kilometraje,
    Imagenes = auto.Imagenes.Select(i => new DTOs.Autos.ImagenDto
   {
  Id = i.Id,
     Url = i.Url
 }).ToList()
      };
    }

    public async Task<bool> DeleteAutoAsync(int id)
    {
        var auto = await _context.Autos
  .Include(a => a.Imagenes)
  .FirstOrDefaultAsync(a => a.Id == id);

        if (auto == null)
        return false;

 // Eliminar imágenes de S3 antes de eliminar el auto
        if (auto.Imagenes.Any())
        {
foreach (var imagen in auto.Imagenes)
          {
try
     {
       // Extraer el key de la URL de S3
      // URL format: https://bucket.s3.region.amazonaws.com/key
     // o: https://bucket.s3.amazonaws.com/key
           var uri = new Uri(imagen.Url);
   var key = uri.AbsolutePath.TrimStart('/');
       
    await _imagenService.DeleteImageAsync(key);
  }
 catch (Exception ex)
            {
        // Log error pero continuar con la eliminación
      // TODO: Implementar logging
          Console.WriteLine($"Error al eliminar imagen de S3: {ex.Message}");
      }
   }
        }

    _context.Autos.Remove(auto);
        await _context.SaveChangesAsync();

        return true;
    }

    // ==================== MOTOS ====================

    public async Task<List<MotoDto>> GetAllMotosAsync()
    {
   return await _context.Motos
  .Include(m => m.Imagenes)
    .Select(m => new MotoDto
{
      Id = m.Id,
  Marca = m.Marca,
   Modelo = m.Modelo,
        Anio = m.Anio,
 Precio = m.Precio,
   Descripcion = m.Descripcion,
   Disponible = m.Disponible,
      Estado = m.Estado,
     FechaPublicacion = m.FechaPublicacion,
       Cilindrada = m.Cilindrada,
 TipoMoto = m.TipoMoto,
  Kilometraje = m.Kilometraje,
     Imagenes = m.Imagenes.Select(i => new DTOs.Motos.ImagenDto
    {
    Id = i.Id,
Url = i.Url
 }).ToList()
})
  .ToListAsync();
    }

    public async Task<MotoDto?> GetMotoByIdAsync(int id)
    {
    var moto = await _context.Motos
  .Include(m => m.Imagenes)
   .FirstOrDefaultAsync(m => m.Id == id);

     if (moto == null)
       return null;

        return new MotoDto
   {
   Id = moto.Id,
 Marca = moto.Marca,
  Modelo = moto.Modelo,
   Anio = moto.Anio,
      Precio = moto.Precio,
   Descripcion = moto.Descripcion,
Disponible = moto.Disponible,
     Estado = moto.Estado,
    FechaPublicacion = moto.FechaPublicacion,
  Cilindrada = moto.Cilindrada,
TipoMoto = moto.TipoMoto,
       Kilometraje = moto.Kilometraje,
    Imagenes = moto.Imagenes.Select(i => new DTOs.Motos.ImagenDto
{
 Id = i.Id,
    Url = i.Url
  }).ToList()
   };
    }

  public async Task<MotoDto> CreateMotoAsync(CreateMotoDto dto)
    {
        var moto = new Moto
        {
            Marca = dto.Marca,
            Modelo = dto.Modelo,
  Anio = dto.Anio,
            Precio = dto.Precio,
            Descripcion = dto.Descripcion,
     Disponible = dto.Disponible,
          Estado = dto.Estado,
          FechaPublicacion = DateTime.UtcNow,
    Cilindrada = dto.Cilindrada,
    TipoMoto = dto.TipoMoto,
   Kilometraje = dto.Kilometraje
    };

        _context.Motos.Add(moto);
        await _context.SaveChangesAsync();

        return new MotoDto
        {
 Id = moto.Id,
  Marca = moto.Marca,
          Modelo = moto.Modelo,
   Anio = moto.Anio,
      Precio = moto.Precio,
      Descripcion = moto.Descripcion,
        Disponible = moto.Disponible,
    FechaPublicacion = moto.FechaPublicacion,
 Cilindrada = moto.Cilindrada,
    TipoMoto = moto.TipoMoto,
  Kilometraje = moto.Kilometraje,
       Imagenes = new List<DTOs.Motos.ImagenDto>()
    };
    }

    public async Task<MotoDto> UpdateMotoAsync(int id, UpdateMotoDto dto)
 {
    var moto = await _context.Motos
  .Include(m => m.Imagenes)
     .FirstOrDefaultAsync(m => m.Id == id);

   if (moto == null)
      throw new KeyNotFoundException($"Moto con ID {id} no encontrada");

   if (!string.IsNullOrWhiteSpace(dto.Marca))
moto.Marca = dto.Marca;

 if (!string.IsNullOrWhiteSpace(dto.Modelo))
   moto.Modelo = dto.Modelo;

   if (dto.Anio.HasValue)
     moto.Anio = dto.Anio.Value;

   if (dto.Precio.HasValue)
   moto.Precio = dto.Precio.Value;

if (dto.Descripcion != null)
   moto.Descripcion = dto.Descripcion;

  if (dto.Disponible.HasValue)
moto.Disponible = dto.Disponible.Value;

        if (!string.IsNullOrWhiteSpace(dto.Estado))
   moto.Estado = dto.Estado;

  if (dto.Cilindrada.HasValue)
   moto.Cilindrada = dto.Cilindrada.Value;

      if (dto.TipoMoto != null)
     moto.TipoMoto = dto.TipoMoto;

   if (dto.Kilometraje.HasValue)
  moto.Kilometraje = dto.Kilometraje;

      await _context.SaveChangesAsync();

    return new MotoDto
      {
 Id = moto.Id,
 Marca = moto.Marca,
 Modelo = moto.Modelo,
 Anio = moto.Anio,
    Precio = moto.Precio,
    Descripcion = moto.Descripcion,
       Disponible = moto.Disponible,
     Estado = moto.Estado,
    FechaPublicacion = moto.FechaPublicacion,
    Cilindrada = moto.Cilindrada,
   TipoMoto = moto.TipoMoto,
  Kilometraje = moto.Kilometraje,
   Imagenes = moto.Imagenes.Select(i => new DTOs.Motos.ImagenDto
       {
    Id = i.Id,
Url = i.Url
      }).ToList()
        };
    }

    public async Task<bool> DeleteMotoAsync(int id)
  {
        var moto = await _context.Motos
     .Include(m => m.Imagenes)
      .FirstOrDefaultAsync(m => m.Id == id);

   if (moto == null)
  return false;

     // Eliminar imágenes de S3 antes de eliminar la moto
     if (moto.Imagenes.Any())
 {
            foreach (var imagen in moto.Imagenes)
  {
        try
   {
         // Extraer el key de la URL de S3
    var uri = new Uri(imagen.Url);
 var key = uri.AbsolutePath.TrimStart('/');
  
  await _imagenService.DeleteImageAsync(key);
    }
     catch (Exception ex)
             {
    // Log error pero continuar con la eliminación
        Console.WriteLine($"Error al eliminar imagen de S3: {ex.Message}");
  }
        }
  }

    _context.Motos.Remove(moto);
        await _context.SaveChangesAsync();

        return true;
    }

 // ==================== COMÚN ====================

    public async Task<bool> VehiculoExistsAsync(int id)
    {
        return await _context.Vehiculos.AnyAsync(v => v.Id == id);
    }

    // ==================== DESTACADOS ====================

    public async Task<List<AutoDto>> GetAutosDestacadosAsync()
    {
        return await _context.Autos
 .Include(a => a.Imagenes)
    .Where(a => a.EsDestacado && a.Disponible)
        .OrderBy(a => a.OrdenDestacado ?? int.MaxValue)
   .ThenByDescending(a => a.FechaPublicacion)
     .Select(a => new AutoDto
   {
           Id = a.Id,
     Marca = a.Marca,
      Modelo = a.Modelo,
                Anio = a.Anio,
    Precio = a.Precio,
  Descripcion = a.Descripcion,
     Disponible = a.Disponible,
      Estado = a.Estado,
      FechaPublicacion = a.FechaPublicacion,
    Puertas = a.Puertas,
  TipoCombustible = a.TipoCombustible,
   Transmision = a.Transmision,
           Kilometraje = a.Kilometraje,
   EsDestacado = a.EsDestacado,
     OrdenDestacado = a.OrdenDestacado,
       Imagenes = a.Imagenes.Select(i => new DTOs.Autos.ImagenDto
  {
        Id = i.Id,
     Url = i.Url
      }).ToList()
       })
   .ToListAsync();
    }

    public async Task<List<MotoDto>> GetMotosDestacadasAsync()
    {
        return await _context.Motos
            .Include(m => m.Imagenes)
            .Where(m => m.EsDestacado && m.Disponible)
     .OrderBy(m => m.OrdenDestacado ?? int.MaxValue)
       .ThenByDescending(m => m.FechaPublicacion)
 .Select(m => new MotoDto
 {
  Id = m.Id,
   Marca = m.Marca,
 Modelo = m.Modelo,
Anio = m.Anio,
      Precio = m.Precio,
   Descripcion = m.Descripcion,
  Disponible = m.Disponible,
    Estado = m.Estado,
   FechaPublicacion = m.FechaPublicacion,
 Cilindrada = m.Cilindrada,
 TipoMoto = m.TipoMoto,
  Kilometraje = m.Kilometraje,
   EsDestacado = m.EsDestacado,
  OrdenDestacado = m.OrdenDestacado,
      Imagenes = m.Imagenes.Select(i => new DTOs.Motos.ImagenDto
       {
Id = i.Id,
      Url = i.Url
  }).ToList()
      })
  .ToListAsync();
  }

    public async Task<List<object>> GetVehiculosDestacadosAsync()
    {
        var autos = await GetAutosDestacadosAsync();
        var motos = await GetMotosDestacadasAsync();
        
    var vehiculos = new List<object>();
        
        foreach (var auto in autos)
        {
  vehiculos.Add(new { tipo = "Auto", vehiculo = auto });
        }
        
   foreach (var moto in motos)
        {
            vehiculos.Add(new { tipo = "Moto", vehiculo = moto });
        }
        
      return vehiculos
     .OrderBy(v =>
            {
        if (v.GetType().GetProperty("vehiculo")?.GetValue(v) is AutoDto auto)
            return auto.OrdenDestacado ?? int.MaxValue;
                if (v.GetType().GetProperty("vehiculo")?.GetValue(v) is MotoDto moto)
return moto.OrdenDestacado ?? int.MaxValue;
         return int.MaxValue;
            })
        .ToList();
    }

    public async Task<AutoDto> MarcarAutoComoDestacadoAsync(int id, int? orden = null)
    {
        var auto = await _context.Autos
  .Include(a => a.Imagenes)
      .FirstOrDefaultAsync(a => a.Id == id);

  if (auto == null)
throw new KeyNotFoundException($"Auto con ID {id} no encontrado");

        if (!orden.HasValue)
        {
     var maxOrden = await _context.Vehiculos
   .Where(v => v.EsDestacado && v.OrdenDestacado.HasValue)
    .MaxAsync(v => (int?)v.OrdenDestacado) ?? 0;
        orden = maxOrden + 1;
  }

        auto.EsDestacado = true;
   auto.OrdenDestacado = orden.Value;
        await _context.SaveChangesAsync();

        return new AutoDto
        {
      Id = auto.Id,
   Marca = auto.Marca,
  Modelo = auto.Modelo,
      Anio = auto.Anio,
   Precio = auto.Precio,
    Descripcion = auto.Descripcion,
     Disponible = auto.Disponible,
     Estado = auto.Estado,
 FechaPublicacion = auto.FechaPublicacion,
 Puertas = auto.Puertas,
    TipoCombustible = auto.TipoCombustible,
     Transmision = auto.Transmision,
 Kilometraje = auto.Kilometraje,
    EsDestacado = auto.EsDestacado,
OrdenDestacado = auto.OrdenDestacado,
  Imagenes = auto.Imagenes.Select(i => new DTOs.Autos.ImagenDto
        {
Id = i.Id,
    Url = i.Url
        }).ToList()
        };
    }

  public async Task<MotoDto> MarcarMotoComoDestacadaAsync(int id, int? orden = null)
    {
        var moto = await _context.Motos
       .Include(m => m.Imagenes)
     .FirstOrDefaultAsync(m => m.Id == id);

        if (moto == null)
throw new KeyNotFoundException($"Moto con ID {id} no encontrada");

   if (!orden.HasValue)
        {
var maxOrden = await _context.Vehiculos
     .Where(v => v.EsDestacado && v.OrdenDestacado.HasValue)
              .MaxAsync(v => (int?)v.OrdenDestacado) ?? 0;
      orden = maxOrden + 1;
  }

    moto.EsDestacado = true;
   moto.OrdenDestacado = orden.Value;
 await _context.SaveChangesAsync();

   return new MotoDto
        {
 Id = moto.Id,
    Marca = moto.Marca,
  Modelo = moto.Modelo,
      Anio = moto.Anio,
     Precio = moto.Precio,
    Descripcion = moto.Descripcion,
     Disponible = moto.Disponible,
Estado = moto.Estado,
     FechaPublicacion = moto.FechaPublicacion,
 Cilindrada = moto.Cilindrada,
  TipoMoto = moto.TipoMoto,
    Kilometraje = moto.Kilometraje,
       EsDestacado = moto.EsDestacado,
  OrdenDestacado = moto.OrdenDestacado,
    Imagenes = moto.Imagenes.Select(i => new DTOs.Motos.ImagenDto
   {
    Id = i.Id,
       Url = i.Url
      }).ToList()
 };
    }

    public async Task<AutoDto> DesmarcarAutoDestacadoAsync(int id)
    {
        var auto = await _context.Autos
       .Include(a => a.Imagenes)
       .FirstOrDefaultAsync(a => a.Id == id);

        if (auto == null)
    throw new KeyNotFoundException($"Auto con ID {id} no encontrado");

        auto.EsDestacado = false;
        auto.OrdenDestacado = null;
        await _context.SaveChangesAsync();

     return new AutoDto
        {
      Id = auto.Id,
      Marca = auto.Marca,
    Modelo = auto.Modelo,
   Anio = auto.Anio,
   Precio = auto.Precio,
      Descripcion = auto.Descripcion,
  Disponible = auto.Disponible,
      Estado = auto.Estado,
      FechaPublicacion = auto.FechaPublicacion,
 Puertas = auto.Puertas,
    TipoCombustible = auto.TipoCombustible,
    Transmision = auto.Transmision,
    Kilometraje = auto.Kilometraje,
   EsDestacado = auto.EsDestacado,
  OrdenDestacado = auto.OrdenDestacado,
    Imagenes = auto.Imagenes.Select(i => new DTOs.Autos.ImagenDto
    {
   Id = i.Id,
       Url = i.Url
     }).ToList()
   };
  }

    public async Task<MotoDto> DesmarcarMotoDestacadaAsync(int id)
    {
 var moto = await _context.Motos
       .Include(m => m.Imagenes)
  .FirstOrDefaultAsync(m => m.Id == id);

 if (moto == null)
throw new KeyNotFoundException($"Moto con ID {id} no encontrada");

  moto.EsDestacado = false;
 moto.OrdenDestacado = null;
   await _context.SaveChangesAsync();

    return new MotoDto
      {
       Id = moto.Id,
 Marca = moto.Marca,
  Modelo = moto.Modelo,
     Anio = moto.Anio,
   Precio = moto.Precio,
      Descripcion = moto.Descripcion,
   Disponible = moto.Disponible,
    Estado = moto.Estado,
  FechaPublicacion = moto.FechaPublicacion,
  Cilindrada = moto.Cilindrada,
     TipoMoto = moto.TipoMoto,
    Kilometraje = moto.Kilometraje,
   EsDestacado = moto.EsDestacado,
  OrdenDestacado = moto.OrdenDestacado,
   Imagenes = moto.Imagenes.Select(i => new DTOs.Motos.ImagenDto
      {
   Id = i.Id,
  Url = i.Url
  }).ToList()
   };
    }
}

