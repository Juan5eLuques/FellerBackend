using Microsoft.EntityFrameworkCore;
using FellerBackend.Models;

namespace FellerBackend.Data;

public class FellerDbContext : DbContext
{
    public FellerDbContext(DbContextOptions<FellerDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<VehiculoBase> Vehiculos { get; set; }
    public DbSet<Auto> Autos { get; set; }
    public DbSet<Moto> Motos { get; set; }
public DbSet<ImagenVehiculo> ImagenesVehiculos { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
  base.OnModelCreating(modelBuilder);

        // Configuración de herencia TPH (Table Per Hierarchy)
        modelBuilder.Entity<VehiculoBase>()
   .HasDiscriminator<string>("TipoVehiculo")
    .HasValue<Auto>("Auto")
            .HasValue<Moto>("Moto");

        // Configuración Usuario
        modelBuilder.Entity<Usuario>(entity =>
{
            entity.HasKey(e => e.Id);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
 entity.HasIndex(e => e.Email).IsUnique();
    entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Rol).HasMaxLength(20);
        });

   // Configuración VehiculoBase
        modelBuilder.Entity<VehiculoBase>(entity =>
  {
 entity.HasKey(e => e.Id);
            entity.Property(e => e.Marca).IsRequired().HasMaxLength(50);
 entity.Property(e => e.Modelo).IsRequired().HasMaxLength(50);
         entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Estado).IsRequired().HasDefaultValue("Usado");
            entity.Property(e => e.EsDestacado).HasDefaultValue(false);
            
  // Índice para búsqueda rápida de destacados
      entity.HasIndex(e => new { e.EsDestacado, e.OrdenDestacado });
        });

        // Configuración específica de Auto
        modelBuilder.Entity<Auto>(entity =>
        {
   entity.Property(e => e.Puertas).IsRequired().HasDefaultValue(4);
  entity.Property(e => e.TipoCombustible).IsRequired().HasMaxLength(20).HasDefaultValue("Nafta");
            entity.Property(e => e.Transmision).HasMaxLength(20);
        });

        // Configuración específica de Moto
      modelBuilder.Entity<Moto>(entity =>
    {
            entity.Property(e => e.Cilindrada).IsRequired().HasDefaultValue(150);
       entity.Property(e => e.TipoMoto).HasMaxLength(20);
        });

        // Configuración ImagenVehiculo
        modelBuilder.Entity<ImagenVehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
     entity.HasOne(e => e.Vehiculo)
       .WithMany(v => v.Imagenes)
   .HasForeignKey(e => e.VehiculoId)
       .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración Turno
        modelBuilder.Entity<Turno>(entity =>
        {
  entity.HasKey(e => e.Id);
 entity.HasOne(e => e.Usuario)
         .WithMany(u => u.Turnos)
  .HasForeignKey(e => e.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
 entity.Property(e => e.TipoLavado).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(20);
        });

        // Configuración Notificacion
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
   entity.HasOne(e => e.Usuario)
        .WithMany(u => u.Notificaciones)
    .HasForeignKey(e => e.UsuarioId)
       .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Tipo).HasMaxLength(20);
        });
  }
}
