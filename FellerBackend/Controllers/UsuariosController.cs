using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FellerBackend.DTOs.Usuarios;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITurnoService _turnoService;

    public UsuariosController(IUsuarioService usuarioService, ITurnoService turnoService)
{
        _usuarioService = usuarioService;
        _turnoService = turnoService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
public async Task<IActionResult> GetAll()
    {
        try
      {
   var usuarios = await _usuarioService.GetAllAsync();
 return Ok(ResponseWrapper<List<UsuarioDto>>.SuccessResponse(usuarios));
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
      var usuario = await _usuarioService.GetByIdAsync(id);
      
   if (usuario == null)
          return NotFound(ResponseWrapper<object>.ErrorResponse($"Usuario con ID {id} no encontrado"));

      return Ok(ResponseWrapper<UsuarioDto>.SuccessResponse(usuario));
        }
  catch (Exception ex)
{
return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
  }
  }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto)
    {
   try
     {
   var usuarioActualizado = await _usuarioService.UpdateAsync(id, dto);
      return Ok(ResponseWrapper<UsuarioDto>.SuccessResponse(usuarioActualizado, "Usuario actualizado exitosamente"));
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
    [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(int id)
    {
  try
{
    var deleted = await _usuarioService.DeleteAsync(id);

      if (!deleted)
       return NotFound(ResponseWrapper<object>.ErrorResponse($"Usuario con ID {id} no encontrado"));

       return Ok(ResponseWrapper<object>.SuccessResponse(null, "Usuario eliminado exitosamente"));
   }
  catch (Exception ex)
        {
      return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
   }
    }

    [HttpGet("{id}/turnos")]
    public async Task<IActionResult> GetTurnosByUsuario(int id)
    {
        try
        {
      // Verificar que el usuario existe
       var usuarioExists = await _usuarioService.ExistsAsync(id);
     if (!usuarioExists)
    return NotFound(ResponseWrapper<object>.ErrorResponse($"Usuario con ID {id} no encontrado"));

   var turnos = await _turnoService.GetTurnosByUsuarioAsync(id);
     return Ok(ResponseWrapper<object>.SuccessResponse(turnos));
   }
        catch (Exception ex)
   {
return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }
}
