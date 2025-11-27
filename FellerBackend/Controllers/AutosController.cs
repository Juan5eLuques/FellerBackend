using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.DTOs.Autos;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;
using FellerBackend.Models;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutosController : ControllerBase
{
    private readonly FellerDbContext _context;
    private readonly IVehiculoService _vehiculoService;
    private readonly IImagenService _imagenService;

    public AutosController(FellerDbContext context, IVehiculoService vehiculoService, IImagenService imagenService)
    {
        _context = context;
  _vehiculoService = vehiculoService;
  _imagenService = imagenService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var autos = await _vehiculoService.GetAllAutosAsync();
     return Ok(ResponseWrapper<List<AutoDto>>.SuccessResponse(autos));
        }
 catch (Exception ex)
      {
    return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
    }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
  try
        {
     var auto = await _vehiculoService.GetAutoByIdAsync(id);

      if (auto == null)
    return NotFound(ResponseWrapper<object>.ErrorResponse($"Auto con ID {id} no encontrado"));

   return Ok(ResponseWrapper<AutoDto>.SuccessResponse(auto));
        }
  catch (Exception ex)
        {
     return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
   }
    }

  [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAutoDto dto)
 {
        try
        {
      // Validaciones
    if (string.IsNullOrWhiteSpace(dto.Marca))
return BadRequest(ResponseWrapper<object>.ErrorResponse("La marca es requerida"));

      if (string.IsNullOrWhiteSpace(dto.Modelo))
      return BadRequest(ResponseWrapper<object>.ErrorResponse("El modelo es requerido"));

       if (dto.Anio < 1900 || dto.Anio > DateTime.Now.Year + 1)
   return BadRequest(ResponseWrapper<object>.ErrorResponse("Año inválido"));

    if (dto.Precio <= 0)
        return BadRequest(ResponseWrapper<object>.ErrorResponse("El precio debe ser mayor a 0"));

    // Validar estado
            var estadosValidos = new[] { "Usado", "0km" };
            if (!estadosValidos.Contains(dto.Estado))
       return BadRequest(ResponseWrapper<object>.ErrorResponse("Estado inválido. Valores permitidos: Usado, 0km"));

            // Validar tipo de combustible
        var combustiblesValidos = new[] { "Nafta", "Gasoil", "GNC", "Híbrido", "Eléctrico" };
     if (!combustiblesValidos.Contains(dto.TipoCombustible))
  return BadRequest(ResponseWrapper<object>.ErrorResponse("Tipo de combustible inválido"));

   // Validar puertas
  if (dto.Puertas < 2 || dto.Puertas > 5)
       return BadRequest(ResponseWrapper<object>.ErrorResponse("Cantidad de puertas inválida (2-5)"));

       var auto = await _vehiculoService.CreateAutoAsync(dto);
 return CreatedAtAction(nameof(GetById), new { id = auto.Id }, ResponseWrapper<AutoDto>.SuccessResponse(auto, "Auto creado exitosamente"));
}
  catch (Exception ex)
 {
      return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
   }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAutoDto dto)
    {
        try
        {
    var autoActualizado = await _vehiculoService.UpdateAutoAsync(id, dto);
   return Ok(ResponseWrapper<AutoDto>.SuccessResponse(autoActualizado, "Auto actualizado exitosamente"));
   }
  catch (KeyNotFoundException ex)
   {
 return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
     }
catch (Exception ex)
      {
     return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
   }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
    {
        try
  {
 var deleted = await _vehiculoService.DeleteAutoAsync(id);

   if (!deleted)
        return NotFound(ResponseWrapper<object>.ErrorResponse($"Auto con ID {id} no encontrado"));

       return Ok(ResponseWrapper<object>.SuccessResponse(null, "Auto eliminado exitosamente"));
 }
        catch (Exception ex)
  {
  return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
   }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/imagenes")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
  {
  try
      {
// Validar que el auto existe
      var autoExists = await _vehiculoService.VehiculoExistsAsync(id);
   if (!autoExists)
     return NotFound(ResponseWrapper<object>.ErrorResponse($"Auto con ID {id} no encontrado"));

   // Validar archivo
    if (file == null || file.Length == 0)
      return BadRequest(ResponseWrapper<object>.ErrorResponse("No se proporcionó ningún archivo"));

// Validar tipo de archivo
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
      var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
     return BadRequest(ResponseWrapper<object>.ErrorResponse("Tipo de archivo no permitido. Solo se permiten imágenes (jpg, jpeg, png, webp)"));

    // Validar tamaño (max 5MB)
     if (file.Length > 5 * 1024 * 1024)
      return BadRequest(ResponseWrapper<object>.ErrorResponse("El archivo es demasiado grande. Tamaño máximo: 5MB"));

      // Subir a S3
    var (url, key) = await _imagenService.UploadImageAsync(file, id, "Auto");

    // Guardar en BD
   var imagen = new ImagenVehiculo
   {
    VehiculoId = id,
       Url = url,
       Key = key,
       FechaSubida = DateTime.UtcNow
   };

   _context.ImagenesVehiculos.Add(imagen);
 await _context.SaveChangesAsync();

  var result = new { Id = imagen.Id, Url = imagen.Url };
    return Ok(ResponseWrapper<object>.SuccessResponse(result, "Imagen subida exitosamente"));
     }
  catch (Exception ex)
        {
       return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error al subir la imagen", new List<string> { ex.Message }));
}
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{autoId}/imagenes/{imagenId}")]
    public async Task<IActionResult> DeleteImage(int autoId, int imagenId)
{
   try
      {
    // Buscar la imagen
            var imagen = await _context.ImagenesVehiculos
     .FirstOrDefaultAsync(i => i.Id == imagenId && i.VehiculoId == autoId);

   if (imagen == null)
       return NotFound(ResponseWrapper<object>.ErrorResponse("Imagen no encontrada"));

     // Eliminar de S3
   var deleted = await _imagenService.DeleteImageAsync(imagen.Key);

   // Eliminar de BD
      _context.ImagenesVehiculos.Remove(imagen);
    await _context.SaveChangesAsync();

     return Ok(ResponseWrapper<object>.SuccessResponse(null, "Imagen eliminada exitosamente"));
  }
  catch (Exception ex)
{
    return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error al eliminar la imagen", new List<string> { ex.Message }));
  }
    }
}
