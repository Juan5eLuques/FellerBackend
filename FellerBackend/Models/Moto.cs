namespace FellerBackend.Models;

public class Moto : VehiculoBase
{
    public Moto()
    {
        TipoVehiculo = "Moto";
    }

    // Propiedades específicas de Moto
    public int Cilindrada { get; set; } // Ejemplo: 150, 250, 500
    public string? TipoMoto { get; set; } // "Deportiva", "Cruiser", "Touring", "Naked", "Enduro"
    public int? Kilometraje { get; set; } // Null para 0km
}
