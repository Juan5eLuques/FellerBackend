using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.DTOs.Usuarios;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class UsuarioService : IUsuarioService
{
 private readonly FellerDbContext _context;

    public UsuarioService(FellerDbContext context)
    {
      _context = context;
  }

    public async Task<List<UsuarioDto>> GetAllAsync()
    {
    return await _context.Usuarios
            .Select(u => new UsuarioDto
 {
      Id = u.Id,
      Nombre = u.Nombre,
     Email = u.Email,
      Telefono = u.Telefono,
          Rol = u.Rol,
        FechaRegistro = u.FechaRegistro
       })
    .ToListAsync();
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        
  if (usuario == null)
            return null;

        return new UsuarioDto
        {
            Id = usuario.Id,
      Nombre = usuario.Nombre,
     Email = usuario.Email,
         Telefono = usuario.Telefono,
         Rol = usuario.Rol,
            FechaRegistro = usuario.FechaRegistro
        };
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var usuario = await _context.Usuarios
     .FirstOrDefaultAsync(u => u.Email == email);
        
        if (usuario == null)
        return null;

  return new UsuarioDto
     {
    Id = usuario.Id,
            Nombre = usuario.Nombre,
        Email = usuario.Email,
      Telefono = usuario.Telefono,
            Rol = usuario.Rol,
         FechaRegistro = usuario.FechaRegistro
      };
    }

  public async Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
var usuario = await _context.Usuarios.FindAsync(id);
  
        if (usuario == null)
throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

   // Actualizar solo los campos proporcionados
        if (!string.IsNullOrWhiteSpace(dto.Nombre))
usuario.Nombre = dto.Nombre;

     if (!string.IsNullOrWhiteSpace(dto.Email))
    {
            // Validar que el email no esté en uso por otro usuario
            var emailExists = await _context.Usuarios
 .AnyAsync(u => u.Email == dto.Email && u.Id != id);
        
     if (emailExists)
    throw new InvalidOperationException("El email ya está en uso");

    usuario.Email = dto.Email;
        }

        if (dto.Telefono != null)
 usuario.Telefono = dto.Telefono;

        if (!string.IsNullOrWhiteSpace(dto.Rol))
    usuario.Rol = dto.Rol;

        await _context.SaveChangesAsync();

  return new UsuarioDto
        {
     Id = usuario.Id,
         Nombre = usuario.Nombre,
  Email = usuario.Email,
    Telefono = usuario.Telefono,
         Rol = usuario.Rol,
            FechaRegistro = usuario.FechaRegistro
        };
    }

    public async Task<bool> DeleteAsync(int id)
  {
 var usuario = await _context.Usuarios.FindAsync(id);
    
        if (usuario == null)
     return false;

        _context.Usuarios.Remove(usuario);
  await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Usuarios.AnyAsync(u => u.Id == id);
    }

 public async Task<bool> EmailExistsAsync(string email)
 {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }
}
