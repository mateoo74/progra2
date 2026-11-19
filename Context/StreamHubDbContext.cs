using Microsoft.EntityFrameworkCore;
using StreamHubApi.Models;

namespace StreamHubApi.Context
{
    public class StreamHubDbContext : DbContext
    {
        public StreamHubDbContext(DbContextOptions<StreamHubDbContext> options) : base(options)
        {
        }

        public DbSet<Contenido> Contenidos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<PasarelaPago> PasarelasPago { get; set; }
        public DbSet<Boleta> Boletas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Evita que se registren dos usuarios con el mismo email
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}