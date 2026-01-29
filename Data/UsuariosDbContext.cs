using Microsoft.EntityFrameworkCore;
using InventarioWEB.Models;

using InventarioWEB;
namespace InventarioWEB.Data
{
    /// <summary>
    /// Contexto para la base de datos "Usuarios" en MySQL Workbench.
    /// Gestiona las tablas de autenticación: usuario y roles.
    /// </summary>
    public class UsuariosDbContext : DbContext
    {
        public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options)
            : base(options)
        {
        }

        // Tabla principal de usuarios
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        // Tabla de roles de usuario
        public DbSet<Rol> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre Usuario y Rol (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol) // ← corregido: IdRol
                .OnDelete(DeleteBehavior.Restrict);

            // Nombres exactos de tablas en MySQL
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Rol>().ToTable("roles");
        }
    }
}
