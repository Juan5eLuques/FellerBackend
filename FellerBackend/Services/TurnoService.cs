using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.DTOs.Turnos;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class TurnoService : ITurnoService
{
    private readonly FellerDbContext _context;
    private readonly IWhatsAppService _whatsappService;

    public TurnoService(FellerDbContext context, IWhatsAppService whatsappService)
  {
 _context = context;
        _whatsappService = whatsappService;
    }

    public async Task<List<TurnoDto>> GetTurnosByUsuarioAsync(int usuarioId)
    {
        return await _context.Turnos
       .Include(t => t.Usuario)
          .Where(t => t.UsuarioId == usuarioId)
   .OrderByDescending(t => t.Fecha)
   .Select(t => new TurnoDto
{
Id = t.Id,
UsuarioId = t.UsuarioId,
  NombreUsuario = t.Usuario!.Nombre,
      EmailUsuario = t.Usuario.Email,
     Fecha = t.Fecha,
    Hora = t.Hora,
TipoLavado = t.TipoLavado,
 Estado = t.Estado,
FechaFinalizacion = t.FechaFinalizacion,
FechaCreacion = t.FechaCreacion
 })
         .ToListAsync();
    }

    public async Task<TurnoDto> CreateTurnoAsync(int usuarioId, CreateTurnoDto dto)
    {
        // Validar que el usuario existe
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

        // Validar que la fecha no sea pasada
  if (dto.Fecha.Date < DateTime.UtcNow.Date)
       throw new InvalidOperationException("No se pueden crear turnos en fechas pasadas");

// Validar disponibilidad
        var turnoExistente = await _context.Turnos
       .AnyAsync(t => t.Fecha.Date == dto.Fecha.Date && 
    t.Hora == dto.Hora &&
    t.Estado != "Cancelado");

if (turnoExistente)
       throw new InvalidOperationException("El horario seleccionado no está disponible");

        var turno = new Turno
        {
UsuarioId = usuarioId,
  Fecha = dto.Fecha.ToUniversalTime(),
Hora = dto.Hora,
    TipoLavado = dto.TipoLavado,
  Estado = "Pendiente",
        FechaCreacion = DateTime.UtcNow
   };

   _context.Turnos.Add(turno);
  await _context.SaveChangesAsync();

// Recargar con usuario
      await _context.Entry(turno)
            .Reference(t => t.Usuario)
  .LoadAsync();

        return new TurnoDto
   {
    Id = turno.Id,
    UsuarioId = turno.UsuarioId,
    NombreUsuario = turno.Usuario!.Nombre,
    EmailUsuario = turno.Usuario.Email,
       Fecha = turno.Fecha,
Hora = turno.Hora,
TipoLavado = turno.TipoLavado,
   Estado = turno.Estado,
            FechaFinalizacion = turno.FechaFinalizacion,
  FechaCreacion = turno.FechaCreacion
        };
    }

    public async Task<List<TurnoDto>> GetAllTurnosAsync()
    {
     return await _context.Turnos
     .Include(t => t.Usuario)
.OrderByDescending(t => t.Fecha)
   .Select(t => new TurnoDto
     {
     Id = t.Id,
 UsuarioId = t.UsuarioId,
    NombreUsuario = t.Usuario!.Nombre,
     EmailUsuario = t.Usuario.Email,
    Fecha = t.Fecha,
Hora = t.Hora,
    TipoLavado = t.TipoLavado,
 Estado = t.Estado,
     FechaFinalizacion = t.FechaFinalizacion,
  FechaCreacion = t.FechaCreacion
       })
  .ToListAsync();
    }

    public async Task<TurnoDto> UpdateEstadoAsync(int id, string nuevoEstado)
    {
var turno = await _context.Turnos
       .Include(t => t.Usuario)
 .FirstOrDefaultAsync(t => t.Id == id);

      if (turno == null)
       throw new KeyNotFoundException($"Turno con ID {id} no encontrado");

   // Validar estados válidos
   var estadosValidos = new[] { "Pendiente", "EnProceso", "Finalizado", "Cancelado" };
 if (!estadosValidos.Contains(nuevoEstado))
       throw new InvalidOperationException($"Estado '{nuevoEstado}' no es válido");

      turno.Estado = nuevoEstado;

// Si pasa a finalizado, registrar fecha y enviar notificación
        if (nuevoEstado == "Finalizado" && turno.FechaFinalizacion == null)
{
   turno.FechaFinalizacion = DateTime.UtcNow;

 // Enviar notificación por WhatsApp
  if (!string.IsNullOrEmpty(turno.Usuario?.Telefono))
  {
 var mensaje = $"¡Hola {turno.Usuario.Nombre}! Tu turno de lavado {turno.TipoLavado} ha finalizado. ¡Gracias por confiar en Feller Automotores!";
       await _whatsappService.EnviarMensajeAsync(turno.Usuario.Telefono, mensaje);

// Guardar en historial de notificaciones
var notificacion = new Notificacion
       {
      UsuarioId = turno.UsuarioId,
Mensaje = mensaje,
           Tipo = "WhatsApp",
    FechaEnvio = DateTime.UtcNow,
      Enviada = true
     };
       _context.Notificaciones.Add(notificacion);
    }
}

  await _context.SaveChangesAsync();

        return new TurnoDto
 {
       Id = turno.Id,
      UsuarioId = turno.UsuarioId,
   NombreUsuario = turno.Usuario!.Nombre,
 EmailUsuario = turno.Usuario.Email,
 Fecha = turno.Fecha,
  Hora = turno.Hora,
 TipoLavado = turno.TipoLavado,
        Estado = turno.Estado,
   FechaFinalizacion = turno.FechaFinalizacion,
FechaCreacion = turno.FechaCreacion
  };
    }

    public async Task<bool> DeleteTurnoAsync(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);

        if (turno == null)
      return false;

        _context.Turnos.Remove(turno);
 await _context.SaveChangesAsync();

        return true;
  }

    public async Task<List<TimeSpan>> GetDisponibilidadAsync(DateTime fecha)
  {
  // Horarios disponibles de 9:00 a 18:00 cada hora
 var horariosDisponibles = new List<TimeSpan>();
   for (int hora = 9; hora <= 18; hora++)
{
    horariosDisponibles.Add(new TimeSpan(hora, 0, 0));
   }

    // Obtener turnos ocupados para esa fecha
        var turnosOcupados = await _context.Turnos
     .Where(t => t.Fecha.Date == fecha.Date && t.Estado != "Cancelado")
    .Select(t => t.Hora)
  .ToListAsync();

        // Filtrar horarios disponibles
 return horariosDisponibles
       .Where(h => !turnosOcupados.Contains(h))
  .ToList();
    }
}
