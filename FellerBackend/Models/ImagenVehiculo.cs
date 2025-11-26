namespace FellerBackend.Models;

public class ImagenVehiculo
{
    public int Id { get; set; }
  public int VehiculoId { get; set; }
    public string Url { get; set; } = string.Empty; // URL pública o presignada
    public string Key { get; set; } = string.Empty; // Key en S3
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

    // Navigation property
    public VehiculoBase? Vehiculo { get; set; }
}
