using FellerBackend.Models;
using FellerBackend.DTOs.Usuarios;

namespace FellerBackend.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto?> GetByEmailAsync(string email);
    Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
}
