namespace FellerBackend.DTOs.Motos;

public class CreateMotoDto
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Disponible { get; set; } = true;
    public string Estado { get; set; } = "Usado";
    
    public int Cilindrada { get; set; }
    public string? TipoMoto { get; set; }
    public int? Kilometraje { get; set; }

    public bool EsDestacado { get; set; } = false;
    public int? OrdenDestacado { get; set; }
}
