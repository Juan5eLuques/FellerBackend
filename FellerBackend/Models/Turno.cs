namespace FellerBackend.Models;

public class Turno
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string TipoLavado { get; set; } = string.Empty; // Básico, Completo, Premium
    public string Estado { get; set; } = "Pendiente"; // Pendiente, EnProceso, Finalizado, Cancelado
    public DateTime? FechaFinalizacion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Usuario? Usuario { get; set; }
}
