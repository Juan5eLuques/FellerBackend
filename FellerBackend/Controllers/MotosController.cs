using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.DTOs.Motos;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;
using FellerBackend.Models;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotosController : ControllerBase
{
    private readonly FellerDbContext _context;
    private readonly IVehiculoService _vehiculoService;
    private readonly IImagenService _imagenService;

    public MotosController(FellerDbContext context, IVehiculoService vehiculoService, IImagenService imagenService)
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
            var motos = await _vehiculoService.GetAllMotosAsync();
            return Ok(ResponseWrapper<List<MotoDto>>.SuccessResponse(motos));
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
            var moto = await _vehiculoService.GetMotoByIdAsync(id);

            if (moto == null)
                return NotFound(ResponseWrapper<object>.ErrorResponse($"Moto con ID {id} no encontrada"));

            return Ok(ResponseWrapper<MotoDto>.SuccessResponse(moto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotoDto dto)
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

            // Validar cilindrada
            if (dto.Cilindrada < 50 || dto.Cilindrada > 2000)
                return BadRequest(ResponseWrapper<object>.ErrorResponse("Cilindrada inválida (50-2000cc)"));

            var moto = await _vehiculoService.CreateMotoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = moto.Id }, ResponseWrapper<MotoDto>.SuccessResponse(moto, "Moto creada exitosamente"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMotoDto dto)
    {
        try
        {
            var motoActualizada = await _vehiculoService.UpdateMotoAsync(id, dto);
            return Ok(ResponseWrapper<MotoDto>.SuccessResponse(motoActualizada, "Moto actualizada exitosamente"));
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
            var deleted = await _vehiculoService.DeleteMotoAsync(id);

            if (!deleted)
                return NotFound(ResponseWrapper<object>.ErrorResponse($"Moto con ID {id} no encontrada"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Moto eliminada exitosamente"));
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
            // Validar que la moto existe
            var motoExists = await _vehiculoService.VehiculoExistsAsync(id);
            if (!motoExists)
                return NotFound(ResponseWrapper<object>.ErrorResponse($"Moto con ID {id} no encontrada"));

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
            var (url, key) = await _imagenService.UploadImageAsync(file, id, "Moto");

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
    [HttpDelete("{motoId}/imagenes/{imagenId}")]
    public async Task<IActionResult> DeleteImage(int motoId, int imagenId)
    {
        try
        {
            // Buscar la imagen
            var imagen = await _context.ImagenesVehiculos
                .FirstOrDefaultAsync(i => i.Id == imagenId && i.VehiculoId == motoId);

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
