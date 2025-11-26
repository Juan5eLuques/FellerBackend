using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FellerBackend.DTOs.Turnos;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;
using System.Security.Claims;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnoService;

    public TurnosController(ITurnoService turnoService)
    {
        _turnoService = turnoService;
    }

    [HttpGet("mios")]
    public async Task<IActionResult> GetMisTurnos()
    {
        try
        {
            // Obtener ID del usuario desde el token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(ResponseWrapper<object>.ErrorResponse("Token inválido"));

            var userId = int.Parse(userIdClaim.Value);
            var turnos = await _turnoService.GetTurnosByUsuarioAsync(userId);
            return Ok(ResponseWrapper<List<TurnoDto>>.SuccessResponse(turnos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTurnoDto dto)
    {
        try
        {
            // Validaciones
            if (dto.Fecha == default)
                return BadRequest(ResponseWrapper<object>.ErrorResponse("La fecha es requerida"));

            if (string.IsNullOrWhiteSpace(dto.TipoLavado))
                return BadRequest(ResponseWrapper<object>.ErrorResponse("El tipo de lavado es requerido"));

            // Validar tipos de lavado válidos
            var tiposValidos = new[] { "Básico", "Completo", "Premium" };
            if (!tiposValidos.Contains(dto.TipoLavado))
                return BadRequest(ResponseWrapper<object>.ErrorResponse("Tipo de lavado inválido. Opciones: Básico, Completo, Premium"));

            // Obtener ID del usuario desde el token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(ResponseWrapper<object>.ErrorResponse("Token inválido"));

            var userId = int.Parse(userIdClaim.Value);
            var turno = await _turnoService.CreateTurnoAsync(userId, dto);
            return CreatedAtAction(nameof(GetMisTurnos), null, ResponseWrapper<TurnoDto>.SuccessResponse(turno, "Turno creado exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var turnos = await _turnoService.GetAllTurnosAsync();
            return Ok(ResponseWrapper<List<TurnoDto>>.SuccessResponse(turnos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] UpdateTurnoEstadoDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Estado))
                return BadRequest(ResponseWrapper<object>.ErrorResponse("El estado es requerido"));

            var turnoActualizado = await _turnoService.UpdateEstadoAsync(id, dto.Estado);
            return Ok(ResponseWrapper<TurnoDto>.SuccessResponse(turnoActualizado, "Estado del turno actualizado exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _turnoService.DeleteTurnoAsync(id);

            if (!deleted)
                return NotFound(ResponseWrapper<object>.ErrorResponse($"Turno con ID {id} no encontrado"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Turno cancelado exitosamente"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [HttpGet("disponibilidad")]
    public async Task<IActionResult> GetDisponibilidad([FromQuery] DateTime fecha)
    {
        try
        {
            if (fecha == default)
                return BadRequest(ResponseWrapper<object>.ErrorResponse("La fecha es requerida"));

            var horariosDisponibles = await _turnoService.GetDisponibilidadAsync(fecha);

            var result = new
            {
                Fecha = fecha,
                HorariosDisponibles = horariosDisponibles.Select(h => h.ToString(@"hh\:mm")).ToList()
            };

            return Ok(ResponseWrapper<object>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }
}
