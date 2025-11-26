namespace FellerBackend.DTOs.Autos;

public class AutoDto
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; }
    public string Estado { get; set; } = string.Empty; // "Usado" o "0km"
    public DateTime FechaPublicacion { get; set; }
    
    // Propiedades específicas de Auto
    public int Puertas { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;
    public string? Transmision { get; set; }
    public int? Kilometraje { get; set; }
    
    // Destacados
    public bool EsDestacado { get; set; }
    public int? OrdenDestacado { get; set; }
 
    public List<ImagenDto> Imagenes { get; set; } = new();
}

public class ImagenDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
}
