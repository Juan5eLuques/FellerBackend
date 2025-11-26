using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.DTOs.Auth;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class AuthService : IAuthService
{
 private readonly FellerDbContext _context;
 private readonly IJwtTokenHelper _jwtHelper;

    public AuthService(FellerDbContext context, IJwtTokenHelper jwtHelper)
 {
 _context = context;
   _jwtHelper = jwtHelper;
  }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
  {
     // Validar que el email no exista
        var emailExists = await _context.Usuarios
   .AnyAsync(u => u.Email == dto.Email);
  
  if (emailExists)
  throw new InvalidOperationException("El email ya está registrado");

// Crear nuevo usuario
        var usuario = new Usuario
{
 Nombre = dto.Nombre,
    Email = dto.Email,
   PasswordHash = PasswordHasher.HashPassword(dto.Password),
     Telefono = dto.Telefono,
          Rol = "Cliente", // Por defecto
     FechaRegistro = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
 await _context.SaveChangesAsync();

        // Generar JWT
        var token = _jwtHelper.GenerateToken(
   usuario.Email,
        usuario.Nombre,
    usuario.Rol,
   usuario.Id
    );

   return new AuthResponseDto
        {
      Token = token,
   Email = usuario.Email,
        Nombre = usuario.Nombre,
   Rol = usuario.Rol
        };
  }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Buscar usuario por email
        var usuario = await _context.Usuarios
       .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null)
     throw new UnauthorizedAccessException("Email o contraseña incorrectos");

    // Verificar contraseña
        var passwordValid = PasswordHasher.VerifyPassword(dto.Password, usuario.PasswordHash);
   
   if (!passwordValid)
     throw new UnauthorizedAccessException("Email o contraseña incorrectos");

   // Generar JWT
        var token = _jwtHelper.GenerateToken(
    usuario.Email,
   usuario.Nombre,
            usuario.Rol,
 usuario.Id
   );

        return new AuthResponseDto
 {
 Token = token,
        Email = usuario.Email,
       Nombre = usuario.Nombre,
Rol = usuario.Rol
        };
    }

    public async Task<Usuario?> GetCurrentUserAsync(int userId)
    {
   return await _context.Usuarios.FindAsync(userId);
    }
}
