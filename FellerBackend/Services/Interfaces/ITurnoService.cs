using FellerBackend.DTOs.Turnos;

namespace FellerBackend.Services.Interfaces;

public interface ITurnoService
{
    Task<List<TurnoDto>> GetTurnosByUsuarioAsync(int usuarioId);
    Task<TurnoDto> CreateTurnoAsync(int usuarioId, CreateTurnoDto dto);
 Task<List<TurnoDto>> GetAllTurnosAsync();
    Task<TurnoDto> UpdateEstadoAsync(int id, string nuevoEstado);
    Task<bool> DeleteTurnoAsync(int id);
    Task<List<TimeSpan>> GetDisponibilidadAsync(DateTime fecha);
}
