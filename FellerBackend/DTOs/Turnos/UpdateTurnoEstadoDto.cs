namespace FellerBackend.DTOs.Turnos;

public class UpdateTurnoEstadoDto
{
    public string Estado { get; set; } = string.Empty; // Pendiente, EnProceso, Finalizado, Cancelado
}
