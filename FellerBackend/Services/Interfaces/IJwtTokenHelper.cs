namespace FellerBackend.Services.Interfaces;

public interface IJwtTokenHelper
{
    string GenerateToken(string email, string nombre, string rol, int userId);
    int? ValidateToken(string token);
}
