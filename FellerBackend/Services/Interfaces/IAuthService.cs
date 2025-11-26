using FellerBackend.DTOs.Auth;
using FellerBackend.Models;

namespace FellerBackend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
 Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<Usuario?> GetCurrentUserAsync(int userId);
}
