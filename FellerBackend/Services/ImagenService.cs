using Amazon.S3;
using Amazon.S3.Model;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class ImagenService : IImagenService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly string _bucketName;

    public ImagenService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
    _configuration = configuration;
   _bucketName = _configuration["AWS:BucketName"]!;
    }

    public async Task<(string Url, string Key)> UploadImageAsync(IFormFile file, int vehiculoId, string tipoVehiculo)
    {
     // Generar key única para S3
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var key = $"{tipoVehiculo.ToLower()}s/{vehiculoId}/{fileName}";

        using var stream = file.OpenReadStream();
   
      var request = new PutObjectRequest
  {
            BucketName = _bucketName,
          Key = key,
            InputStream = stream,
            ContentType = file.ContentType
            // NO usar CannedACL para evitar el error de ACLs
        };

        await _s3Client.PutObjectAsync(request);

        // Generar URL pública
        var url = $"https://{_bucketName}.s3.{_configuration["AWS:Region"]}.amazonaws.com/{key}";
        
        return (url, key);
    }

    public async Task<bool> DeleteImageAsync(string key)
  {
        try
        {
var request = new DeleteObjectRequest
            {
    BucketName = _bucketName,
        Key = key
            };

      await _s3Client.DeleteObjectAsync(request);
         return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetImageUrl(string key)
    {
        // Retornar URL pública
        return $"https://{_bucketName}.s3.{_configuration["AWS:Region"]}.amazonaws.com/{key}";
    }
}
