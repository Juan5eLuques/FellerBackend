namespace FellerBackend.Models;

public class Auto : VehiculoBase
{
    public Auto()
    {
        TipoVehiculo = "Auto";
    }

    // Propiedades específicas de Auto
    public int Puertas { get; set; } = 4; // Por defecto 4 puertas
    public string TipoCombustible { get; set; } = "Nafta"; // "Nafta", "Gasoil", "GNC", "Híbrido", "Eléctrico"
    public string? Transmision { get; set; } // "Manual", "Automática"
    public int? Kilometraje { get; set; } // Null para 0km
}
