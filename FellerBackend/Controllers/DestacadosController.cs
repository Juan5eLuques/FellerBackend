using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FellerBackend.DTOs.Autos;
using FellerBackend.DTOs.Motos;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestacadosController : ControllerBase
{
 private readonly IVehiculoService _vehiculoService;

    public DestacadosController(IVehiculoService vehiculoService)
    {
      _vehiculoService = vehiculoService;
    }

  // ==================== OBTENER DESTACADOS (PÚBLICO) ====================

    /// <summary>
    /// Obtener todos los vehículos destacados (autos y motos mezclados)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetVehiculosDestacados()
    {
        try
  {
            var vehiculos = await _vehiculoService.GetVehiculosDestacadosAsync();
    return Ok(ResponseWrapper<List<object>>.SuccessResponse(vehiculos));
    }
        catch (Exception ex)
        {
   return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
      "Error al obtener vehículos destacados",
      new List<string> { ex.Message }
            ));
        }
    }

    /// <summary>
    /// Obtener solo autos destacados
 /// </summary>
    [HttpGet("autos")]
    public async Task<IActionResult> GetAutosDestacados()
    {
    try
  {
       var autos = await _vehiculoService.GetAutosDestacadosAsync();
         return Ok(ResponseWrapper<List<AutoDto>>.SuccessResponse(autos));
      }
     catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
                "Error al obtener autos destacados",
         new List<string> { ex.Message }
            ));
        }
    }

 /// <summary>
    /// Obtener solo motos destacadas
    /// </summary>
    [HttpGet("motos")]
    public async Task<IActionResult> GetMotosDestacadas()
    {
      try
        {
     var motos = await _vehiculoService.GetMotosDestacadasAsync();
            return Ok(ResponseWrapper<List<MotoDto>>.SuccessResponse(motos));
   }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
                "Error al obtener motos destacadas",
       new List<string> { ex.Message }
            ));
        }
    }

    // ==================== GESTIONAR DESTACADOS (ADMIN) ====================

    /// <summary>
    /// Marcar un auto como destacado
    /// </summary>
    /// <param name="id">ID del auto</param>
    /// <param name="orden">Orden de aparición (opcional, se auto-asigna si no se proporciona)</param>
    [Authorize(Roles = "Admin")]
    [HttpPost("autos/{id}")]
    public async Task<IActionResult> MarcarAutoComoDestacado(int id, [FromQuery] int? orden = null)
    {
        try
        {
         var auto = await _vehiculoService.MarcarAutoComoDestacadoAsync(id, orden);
        return Ok(ResponseWrapper<AutoDto>.SuccessResponse(
          auto,
    $"Auto marcado como destacado con orden {auto.OrdenDestacado}"
  ));
        }
        catch (KeyNotFoundException ex)
        {
          return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
     return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
     "Error al marcar auto como destacado",
          new List<string> { ex.Message }
            ));
        }
    }

    /// <summary>
 /// Marcar una moto como destacada
    /// </summary>
    /// <param name="id">ID de la moto</param>
    /// <param name="orden">Orden de aparición (opcional, se auto-asigna si no se proporciona)</param>
    [Authorize(Roles = "Admin")]
    [HttpPost("motos/{id}")]
    public async Task<IActionResult> MarcarMotoComoDestacada(int id, [FromQuery] int? orden = null)
    {
        try
      {
   var moto = await _vehiculoService.MarcarMotoComoDestacadaAsync(id, orden);
      return Ok(ResponseWrapper<MotoDto>.SuccessResponse(
            moto,
        $"Moto marcada como destacada con orden {moto.OrdenDestacado}"
          ));
 }
        catch (KeyNotFoundException ex)
        {
 return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
   catch (Exception ex)
  {
          return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
            "Error al marcar moto como destacada",
       new List<string> { ex.Message }
      ));
  }
    }

    /// <summary>
    /// Desmarcar un auto como destacado
/// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("autos/{id}")]
    public async Task<IActionResult> DesmarcarAutoDestacado(int id)
    {
   try
        {
        var auto = await _vehiculoService.DesmarcarAutoDestacadoAsync(id);
            return Ok(ResponseWrapper<AutoDto>.SuccessResponse(
         auto,
     "Auto desmarcado como destacado"
        ));
        }
 catch (KeyNotFoundException ex)
        {
            return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
      catch (Exception ex)
        {
         return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
       "Error al desmarcar auto destacado",
                new List<string> { ex.Message }
      ));
        }
    }

    /// <summary>
    /// Desmarcar una moto como destacada
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("motos/{id}")]
    public async Task<IActionResult> DesmarcarMotoDestacada(int id)
    {
   try
     {
            var moto = await _vehiculoService.DesmarcarMotoDestacadaAsync(id);
            return Ok(ResponseWrapper<MotoDto>.SuccessResponse(
       moto,
      "Moto desmarcada como destacada"
    ));
        }
        catch (KeyNotFoundException ex)
    {
   return NotFound(ResponseWrapper<object>.ErrorResponse(ex.Message));
  }
        catch (Exception ex)
 {
      return StatusCode(500, ResponseWrapper<object>.ErrorResponse(
       "Error al desmarcar moto destacada",
              new List<string> { ex.Message }
       ));
        }
    }
}
