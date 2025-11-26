namespace FellerBackend.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Rol { get; set; } = "Cliente"; // Cliente o Admin
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

 // Navigation properties
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
