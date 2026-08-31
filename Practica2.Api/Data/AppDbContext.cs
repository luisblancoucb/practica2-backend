using Microsoft.EntityFrameworkCore;
using Practica2.Api.Models;

namespace Practica2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Cita> Citas => Set<Cita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.Correo)
            .IsUnique();

        modelBuilder.Entity<Servicio>()
            .Property(servicio => servicio.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Cita>()
            .Property(cita => cita.Estado)
            .HasConversion<string>();

        modelBuilder.Entity<Cita>()
            .HasOne(cita => cita.Cliente)
            .WithMany(cliente => cliente.Citas)
            .HasForeignKey(cita => cita.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cita>()
            .HasOne(cita => cita.Servicio)
            .WithMany(servicio => servicio.Citas)
            .HasForeignKey(cita => cita.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}