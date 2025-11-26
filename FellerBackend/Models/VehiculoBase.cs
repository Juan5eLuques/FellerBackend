namespace FellerBackend.Models;

public abstract class VehiculoBase
{
    public int Id { get; set; }
    public string TipoVehiculo { get; set; } = string.Empty; // "Auto" o "Moto" - Para TPH
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; } = true;
    
    // Estado del vehículo
    public string Estado { get; set; } = "Usado"; // "Usado" o "0km"
    
    // Destacados en Home
    public bool EsDestacado { get; set; } = false;
    public int? OrdenDestacado { get; set; } // Orden de aparición (1, 2, 3, etc.)
    
    public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

    // Relación con imágenes
    public ICollection<ImagenVehiculo> Imagenes { get; set; } = new List<ImagenVehiculo>();
}
