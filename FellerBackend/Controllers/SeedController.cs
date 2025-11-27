using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FellerBackend.Data;
using FellerBackend.Models;
using FellerBackend.Helpers;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly FellerDbContext _context;

public SeedController(FellerDbContext context)
    {
    _context = context;
}

    /// <summary>
    /// SOLO PARA DESARROLLO - Crear el primer usuario administrador
    /// IMPORTANTE: Este endpoint NO debe estar en producción
    /// </summary>
    [HttpPost("create-first-admin")]
    public async Task<IActionResult> CreateFirstAdmin()
    {
        try
        {
            // Verificar que no existan usuarios admin
       var adminExists = await _context.Usuarios.AnyAsync(u => u.Rol == "Admin");
 
    if (adminExists)
{
        return BadRequest(new
          {
         success = false,
      message = "Ya existe al menos un usuario administrador en el sistema"
          });
       }

        // Crear primer admin con credenciales por defecto
  var admin = new Usuario
      {
  Nombre = "Administrador Principal",
      Email = "admin@feller.com",
          PasswordHash = PasswordHasher.HashPassword("Admin123!"),
             Telefono = "+54 9 11 0000-0000",
  Rol = "Admin",
  FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(admin);
          await _context.SaveChangesAsync();

         return Ok(new
    {
      success = true,
       message = "Usuario administrador creado exitosamente",
     data = new
                {
      email = admin.Email,
     password = "Admin123!",
            instrucciones = "Usa estas credenciales para hacer login. CAMBIA LA CONTRASEÑA inmediatamente."
                }
   });
        }
        catch (Exception ex)
        {
        return StatusCode(500, new
  {
       success = false,
       message = "Error al crear el administrador",
     error = ex.Message
         });
    }
    }

    /// <summary>
    /// SOLO PARA DESARROLLO - Crear datos de prueba
    /// </summary>
    [HttpPost("seed-test-data")]
    public async Task<IActionResult> SeedTestData()
{
        try
  {
     var dataExists = await _context.Usuarios.AnyAsync();
            
   if (dataExists)
            {
  return BadRequest(new
     {
  success = false,
      message = "Ya existen datos en la base de datos"
     });
       }

  // Crear Admin
            var admin = new Usuario
            {
 Nombre = "Admin Feller",
      Email = "admin@feller.com",
     PasswordHash = PasswordHasher.HashPassword("Admin123!"),
      Telefono = "+54 9 11 1111-1111",
    Rol = "Admin",
          FechaRegistro = DateTime.UtcNow
      };
        _context.Usuarios.Add(admin);

            // Crear Cliente de prueba
            var cliente = new Usuario
      {
  Nombre = "Juan Pérez",
       Email = "juan@test.com",
PasswordHash = PasswordHasher.HashPassword("Password123!"),
      Telefono = "+54 9 11 2222-2222",
          Rol = "Cliente",
         FechaRegistro = DateTime.UtcNow
   };
   _context.Usuarios.Add(cliente);

          // Crear Autos de prueba
            var auto1 = new Auto
      {
    Marca = "Toyota",
     Modelo = "Corolla",
                Anio = 2022,
     Precio = 25000,
                Descripcion = "Toyota Corolla 2022 en excelente estado, único dueño",
         Disponible = true,
        FechaPublicacion = DateTime.UtcNow
            };
          _context.Autos.Add(auto1);

            var auto2 = new Auto
    {
       Marca = "Ford",
    Modelo = "Focus",
                Anio = 2023,
           Precio = 22000,
    Descripcion = "Ford Focus 2023, full equipo, como nuevo",
      Disponible = true,
     FechaPublicacion = DateTime.UtcNow
            };
        _context.Autos.Add(auto2);

    // Crear Motos de prueba
     var moto1 = new Moto
  {
          Marca = "Honda",
                Modelo = "CB 500X",
         Anio = 2023,
        Precio = 8000,
           Descripcion = "Honda CB 500X 2023, impecable",
                Disponible = true,
       FechaPublicacion = DateTime.UtcNow
     };
        _context.Motos.Add(moto1);

     var moto2 = new Moto
        {
           Marca = "Yamaha",
        Modelo = "MT-07",
         Anio = 2022,
      Precio = 9500,
   Descripcion = "Yamaha MT-07 2022, muy cuidada",
         Disponible = true,
      FechaPublicacion = DateTime.UtcNow
            };
 _context.Motos.Add(moto2);

  await _context.SaveChangesAsync();

            return Ok(new
      {
    success = true,
          message = "Datos de prueba creados exitosamente",
   data = new
       {
    admin = new { email = "admin@feller.com", password = "Admin123!" },
         cliente = new { email = "juan@test.com", password = "Password123!" },
               autos = 2,
 motos = 2
      }
            });
        }
 catch (Exception ex)
        {
          return StatusCode(500, new
            {
          success = false,
        message = "Error al crear datos de prueba",
       error = ex.Message
            });
        }
    }

    /// <summary>
    /// PELIGRO - Eliminar TODOS los datos de la base de datos
    /// </summary>
    [HttpDelete("delete-all-data")]
    public async Task<IActionResult> DeleteAllData([FromQuery] string confirmacion)
    {
        if (confirmacion != "CONFIRMO_ELIMINAR_TODO")
        {
        return BadRequest(new
   {
      success = false,
        message = "Debes enviar confirmacion=CONFIRMO_ELIMINAR_TODO en el query string para confirmar esta acción"
            });
   }

        try
    {
       // Eliminar en orden para respetar las relaciones
            _context.Notificaciones.RemoveRange(_context.Notificaciones);
        _context.Turnos.RemoveRange(_context.Turnos);
       _context.ImagenesVehiculos.RemoveRange(_context.ImagenesVehiculos);
  _context.Autos.RemoveRange(_context.Autos);
            _context.Motos.RemoveRange(_context.Motos);
            _context.Usuarios.RemoveRange(_context.Usuarios);

            await _context.SaveChangesAsync();

            return Ok(new
            {
     success = true,
                message = "Todos los datos han sido eliminados"
   });
}
   catch (Exception ex)
        {
            return StatusCode(500, new
            {
          success = false,
    message = "Error al eliminar los datos",
                error = ex.Message
            });
}
    }
}
