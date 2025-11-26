namespace FellerBackend.DTOs.Autos;

public class CreateAutoDto
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; } = true;
    public string Estado { get; set; } = "Usado"; // "Usado" o "0km"
    
    // Propiedades específicas de Auto
    public int Puertas { get; set; } = 4;
    public string TipoCombustible { get; set; } = "Nafta"; // "Nafta", "Gasoil", "GNC", "Híbrido", "Eléctrico"
 public string? Transmision { get; set; } // "Manual", "Automática"
 public int? Kilometraje { get; set; } // Null para 0km
    
  // Destacados (opcional al crear)
 public bool EsDestacado { get; set; } = false;
    public int? OrdenDestacado { get; set; }
}
