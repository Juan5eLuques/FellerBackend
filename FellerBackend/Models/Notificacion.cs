namespace FellerBackend.Models;

public class Notificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // WhatsApp, Email, SMS
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    public bool Enviada { get; set; } = false;

    // Navigation property
    public Usuario? Usuario { get; set; }
}
