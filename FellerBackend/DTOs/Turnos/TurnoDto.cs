namespace FellerBackend.DTOs.Turnos;

public class TurnoDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
  public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string TipoLavado { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaFinalizacion { get; set; }
    public DateTime FechaCreacion { get; set; }
}
