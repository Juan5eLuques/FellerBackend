namespace FellerBackend.DTOs.Turnos;

public class CreateTurnoDto
{
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string TipoLavado { get; set; } = string.Empty;
}
