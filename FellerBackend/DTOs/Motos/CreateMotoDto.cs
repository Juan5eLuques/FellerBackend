namespace FellerBackend.DTOs.Motos;

public class CreateMotoDto
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; } = true;
    public string Estado { get; set; } = "Usado"; // "Usado" o "0km"
    
 // Propiedades específicas de Moto
    public int Cilindrada { get; set; }
    public string? TipoMoto { get; set; } // "Deportiva", "Cruiser", "Touring", "Naked", "Enduro"
    public int? Kilometraje { get; set; } // Null para 0km
    
    // Destacados (opcional al crear)
  public bool EsDestacado { get; set; } = false;
 public int? OrdenDestacado { get; set; }
}
