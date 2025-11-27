namespace FellerBackend.DTOs.Autos;

public class CreateAutoDto
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; } = true;
    public string Estado { get; set; } = "Usado";
    
    public int Puertas { get; set; } = 4;
    public string TipoCombustible { get; set; } = "Nafta";
  public string? Transmision { get; set; }
    public int? Kilometraje { get; set; }
    
    public bool EsDestacado { get; set; } = false;
    public int? OrdenDestacado { get; set; }
}
