using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FellerBackend.DTOs.Auth;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;
using System.Security.Claims;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
   {
   // Validaciones básicas
if (string.IsNullOrWhiteSpace(dto.Nombre))
     return BadRequest(ResponseWrapper<object>.ErrorResponse("El nombre es requerido"));

    if (string.IsNullOrWhiteSpace(dto.Email))
         return BadRequest(ResponseWrapper<object>.ErrorResponse("El email es requerido"));

if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
       return BadRequest(ResponseWrapper<object>.ErrorResponse("La contraseña debe tener al menos 6 caracteres"));

   var result = await _authService.RegisterAsync(dto);
  return Ok(ResponseWrapper<AuthResponseDto>.SuccessResponse(result, "Usuario registrado exitosamente"));
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
    try
        {
  // Validaciones básicas
   if (string.IsNullOrWhiteSpace(dto.Email))
          return BadRequest(ResponseWrapper<object>.ErrorResponse("El email es requerido"));

  if (string.IsNullOrWhiteSpace(dto.Password))
        return BadRequest(ResponseWrapper<object>.ErrorResponse("La contraseña es requerida"));

       var result = await _authService.LoginAsync(dto);
     return Ok(ResponseWrapper<AuthResponseDto>.SuccessResponse(result, "Login exitoso"));
  }
        catch (UnauthorizedAccessException ex)
{
       return Unauthorized(ResponseWrapper<object>.ErrorResponse(ex.Message));
        }
  catch (Exception ex)
        {
      return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
 }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
   // Obtener ID del usuario desde el token JWT
          var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
           return Unauthorized(ResponseWrapper<object>.ErrorResponse("Token inválido"));

    var userId = int.Parse(userIdClaim.Value);
            var usuario = await _authService.GetCurrentUserAsync(userId);

if (usuario == null)
       return NotFound(ResponseWrapper<object>.ErrorResponse("Usuario no encontrado"));

     var usuarioDto = new
     {
       usuario.Id,
          usuario.Nombre,
          usuario.Email,
          usuario.Telefono,
          usuario.Rol,
    usuario.FechaRegistro
  };

       return Ok(ResponseWrapper<object>.SuccessResponse(usuarioDto));
}
  catch (Exception ex)
        {
      return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
   }
    }
}
