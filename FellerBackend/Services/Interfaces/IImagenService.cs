namespace FellerBackend.Services.Interfaces;

public interface IImagenService
{
    Task<(string Url, string Key)> UploadImageAsync(IFormFile file, int vehiculoId, string tipoVehiculo);
    Task<bool> DeleteImageAsync(string key);
    string GetImageUrl(string key);
}
