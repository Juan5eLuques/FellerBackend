using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.DTOs.Dashboard;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class DashboardService : IDashboardService
{
    private readonly FellerDbContext _context;

    public DashboardService(FellerDbContext context)
    {
      _context = context;
    }

    public async Task<DashboardResumenDto> GetResumenAsync()
    {
    var hoy = DateTime.UtcNow.Date;

        var autosPublicados = await _context.Autos.CountAsync();
        var motosPublicadas = await _context.Motos.CountAsync();
  var turnosDelDia = await _context.Turnos
   .CountAsync(t => t.Fecha.Date == hoy);
var usuariosRegistrados = await _context.Usuarios.CountAsync();
      var turnosPendientes = await _context.Turnos
 .CountAsync(t => t.Estado == "Pendiente");
        var turnosEnProceso = await _context.Turnos
       .CountAsync(t => t.Estado == "EnProceso");

      return new DashboardResumenDto
        {
 AutosPublicados = autosPublicados,
            MotosPublicadas = motosPublicadas,
     TurnosDelDia = turnosDelDia,
        UsuariosRegistrados = usuariosRegistrados,
       TurnosPendientes = turnosPendientes,
            TurnosEnProceso = turnosEnProceso
        };
    }
}
