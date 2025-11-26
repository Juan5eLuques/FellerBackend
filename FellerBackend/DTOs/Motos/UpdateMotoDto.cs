namespace FellerBackend.DTOs.Motos;

public class UpdateMotoDto
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Año { get; set; }
    public decimal? Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool? Disponible { get; set; }
    public string? Estado { get; set; } // "Usado" o "0km"
    
 // Propiedades específicas de Moto
    public int? Cilindrada { get; set; }
    public string? TipoMoto { get; set; }
    public int? Kilometraje { get; set; }
    
    // Destacados
    public bool? EsDestacado { get; set; }
    public int? OrdenDestacado { get; set; }
}
