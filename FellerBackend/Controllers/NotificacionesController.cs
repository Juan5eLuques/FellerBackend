using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class NotificacionesController : ControllerBase
{
    private readonly FellerDbContext _context;
    private readonly IWhatsAppService _whatsappService;
    private readonly IUsuarioService _usuarioService;

    public NotificacionesController(FellerDbContext context, IWhatsAppService whatsappService, IUsuarioService usuarioService)
    {
        _context = context;
        _whatsappService = whatsappService;
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var notificaciones = await _context.Notificaciones
                .Include(n => n.Usuario)
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new
                {
                    n.Id,
                    UsuarioId = n.UsuarioId,
                    UsuarioNombre = n.Usuario!.Nombre,
                    UsuarioEmail = n.Usuario.Email,
                    n.Mensaje,
                    n.Tipo,
                    n.FechaEnvio,
                    n.Enviada
                })
                .ToListAsync();

            return Ok(ResponseWrapper<object>.SuccessResponse(notificaciones));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }

    [HttpPost("whatsapp")]
    public async Task<IActionResult> EnviarWhatsApp([FromBody] EnviarWhatsAppDto dto)
    {
        try
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Mensaje))
                return BadRequest(ResponseWrapper<object>.ErrorResponse("El mensaje es requerido"));

            // Verificar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
            if (usuario == null)
                return NotFound(ResponseWrapper<object>.ErrorResponse($"Usuario con ID {dto.UsuarioId} no encontrado"));

            if (string.IsNullOrEmpty(usuario.Telefono))
                return BadRequest(ResponseWrapper<object>.ErrorResponse("El usuario no tiene teléfono registrado"));

            // Enviar WhatsApp
            var enviado = await _whatsappService.EnviarMensajeAsync(usuario.Telefono, dto.Mensaje);

            // Guardar en historial
            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Mensaje = dto.Mensaje,
                Tipo = "WhatsApp",
                FechaEnvio = DateTime.UtcNow,
                Enviada = enviado
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            var result = new
            {
                notificacion.Id,
                notificacion.UsuarioId,
                UsuarioNombre = usuario.Nombre,
                UsuarioTelefono = usuario.Telefono,
                notificacion.Mensaje,
                notificacion.Tipo,
                notificacion.FechaEnvio,
                notificacion.Enviada
            };

            return Ok(ResponseWrapper<object>.SuccessResponse(result, "Mensaje enviado exitosamente"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error al enviar el mensaje", new List<string> { ex.Message }));
        }
    }
}

public class EnviarWhatsAppDto
{
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
