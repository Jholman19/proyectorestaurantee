using Microsoft.EntityFrameworkCore;

namespace RestauranteApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<Suscripcion> Suscripciones => Set<Suscripcion>();
    public DbSet<Consumo> Consumos => Set<Consumo>();
    public DbSet<Aviso> Avisos => Set<Aviso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignorar la propiedad calculada Activa (usa Activo como propiedad real)
        modelBuilder.Entity<Suscripcion>()
            .Ignore(s => s.Activa);

        // Guardar Dia como fecha (sin hora) - lo controlamos usando .Date en código también
        modelBuilder.Entity<Consumo>()
            .Property(x => x.Dia);

        modelBuilder.Entity<Aviso>()
            .Property(x => x.Dia);

        // ✅ Regla de máximo 2 por día por tipo:
        // SuscripcionId + Dia + TipoComida + Numero debe ser único
        modelBuilder.Entity<Consumo>()
            .HasIndex(x => new { x.SuscripcionId, x.Dia, x.TipoComida, x.Numero })
            .IsUnique();

        // ✅ Un aviso por día por tipo (no tiene sentido duplicar)
        modelBuilder.Entity<Aviso>()
            .HasIndex(x => new { x.SuscripcionId, x.Dia, x.TipoComida })
            .IsUnique();
        
        modelBuilder.Entity<Suscripcion>()
            .Property(x => x.Activo)
            .HasDefaultValue(true);

        // ✅ Configurar eliminación en cascada explícitamente
        // Al eliminar un Cliente, se eliminan todas sus Suscripciones
        modelBuilder.Entity<Cliente>()
            .HasMany(c => c.Suscripciones)
            .WithOne(s => s.Cliente)
            .HasForeignKey(s => s.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Al eliminar una Suscripcion, se eliminan todos sus Consumos
        modelBuilder.Entity<Suscripcion>()
            .HasMany(s => s.Consumos)
            .WithOne(c => c.Suscripcion)
            .HasForeignKey(c => c.SuscripcionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Al eliminar una Suscripcion, se eliminan todos sus Avisos
        modelBuilder.Entity<Suscripcion>()
            .HasMany(s => s.Avisos)
            .WithOne(a => a.Suscripcion)
            .HasForeignKey(a => a.SuscripcionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}