namespace FellerBackend.DTOs.Autos;

public class UpdateAutoDto
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Anio { get; set; }
    public decimal? Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool? Disponible { get; set; }
    public string? Estado { get; set; } // "Usado" o "0km"
    
    // Propiedades específicas de Auto
    public int? Puertas { get; set; }
    public string? TipoCombustible { get; set; }
    public string? Transmision { get; set; }
    public int? Kilometraje { get; set; }
    
    // Destacados
    public bool? EsDestacado { get; set; }
    public int? OrdenDestacado { get; set; }
}
