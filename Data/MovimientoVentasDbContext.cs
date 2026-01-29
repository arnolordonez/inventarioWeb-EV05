using Microsoft.EntityFrameworkCore;
using InventarioWEB.Models;

namespace InventarioWEB.Data
{
    public class MovimientoVentasDbContext : DbContext
    {
        public MovimientoVentasDbContext(DbContextOptions<MovimientoVentasDbContext> options)
            : base(options) { }

        // ==========================================================
        // TABLAS
        // ==========================================================
        public DbSet<Abono> Abonos { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Color> Colores { get; set; } = null!;
        public DbSet<DetallePedido> DetallePedidos { get; set; } = null!;
        public DbSet<Genero> Generos { get; set; } = null!;
        public DbSet<MetodoPago> MetodosPago { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Referencia> Referencias { get; set; } = null!;
        public DbSet<ReferenciaTela> ReferenciasTelas { get; set; } = null!;
        public DbSet<Talla> Tallas { get; set; } = null!;
        public DbSet<Tela> Telas { get; set; } = null!;
        public DbSet<TipoCliente> TipoCliente { get; set; } = null!;
        public DbSet<PasswordResetCliente> PasswordResetsClientes { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================================
            // NOMBRE REAL DE TABLAS
            // ==========================================================
            modelBuilder.Entity<Cliente>().ToTable("cliente");
            modelBuilder.Entity<Pedido>().ToTable("pedido");
            modelBuilder.Entity<DetallePedido>().ToTable("detalle_pedido");
            modelBuilder.Entity<Abono>().ToTable("abono");
            modelBuilder.Entity<MetodoPago>().ToTable("metodopago");
            modelBuilder.Entity<Producto>().ToTable("producto");
            modelBuilder.Entity<Genero>().ToTable("genero");
            modelBuilder.Entity<Color>().ToTable("colores");
            modelBuilder.Entity<Talla>().ToTable("tallas");
            modelBuilder.Entity<Tela>().ToTable("telas");
            modelBuilder.Entity<Referencia>().ToTable("referencias");
            modelBuilder.Entity<ReferenciaTela>().ToTable("referencias_telas");
            modelBuilder.Entity<TipoCliente>().ToTable("tipocliente");
            modelBuilder.Entity<PasswordResetCliente>().ToTable("passwordresetsclientes");
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Rol>().ToTable("roles");

            // ==========================================================
            // ⚠️ IMPORTANTE:
            // NO configurar ID_Producto como autogenerado.
            // Es un VARCHAR(50) y se genera en el Controller.
            // ==========================================================

            // ==========================================================
            // RELACIONES
            // ==========================================================

            // CLIENTE 1:N PEDIDOS
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ID_Cliente)
                .OnDelete(DeleteBehavior.Restrict);

            // PEDIDO 1:N DETALLES
            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Pedido)
                .WithMany(p => p.DetallePedidos)
                .HasForeignKey(dp => dp.ID_Pedido)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCTO 1:N DETALLE PEDIDO
            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Producto)
                .WithMany()
                .HasForeignKey(dp => dp.ID_Producto)
                .OnDelete(DeleteBehavior.Restrict);

            // PEDIDO 1:N ABONOS
            modelBuilder.Entity<Abono>()
                .HasOne(a => a.Pedido)
                .WithMany(p => p.Abonos)
                .HasForeignKey(a => a.ID_Pedido)
                .OnDelete(DeleteBehavior.Cascade);

            // METODO PAGO 1:N PEDIDO
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.MetodoPago)
                .WithMany(mp => mp.Pedidos)
                .HasForeignKey(p => p.ID_MetodoPago)
                .OnDelete(DeleteBehavior.Restrict);

            // METODO PAGO 1:N ABONOS
            modelBuilder.Entity<Abono>()
                .HasOne(a => a.MetodoPago)
                .WithMany(mp => mp.Abonos)
                .HasForeignKey(a => a.ID_MetodoPago)
                .OnDelete(DeleteBehavior.Restrict);

            // GENERO 1:N TALLAS
            modelBuilder.Entity<Talla>()
                .HasOne(t => t.Genero)
                .WithMany(g => g.Tallas)
                .HasForeignKey(t => t.ID_Genero);

            // GENERO 1:N REFERENCIAS
            modelBuilder.Entity<Referencia>()
                .HasOne(r => r.Genero)
                .WithMany(g => g.Referencias)
                .HasForeignKey(r => r.ID_Genero);

            // REFERENCIA_TELA: PK COMPUESTA
            modelBuilder.Entity<ReferenciaTela>()
                .HasKey(rt => new {
                    rt.ID_Referencias,
                    rt.ID_Tallas,
                    rt.ID_Genero,
                    rt.ID_Telas
                });

            modelBuilder.Entity<ReferenciaTela>()
                .HasOne(rt => rt.Referencia)
                .WithMany(r => r.ReferenciasTelas)
                .HasForeignKey(rt => rt.ID_Referencias);

            modelBuilder.Entity<ReferenciaTela>()
                .HasOne(rt => rt.Talla)
                .WithMany(t => t.ReferenciasTelas)
                .HasForeignKey(rt => rt.ID_Tallas);

            modelBuilder.Entity<ReferenciaTela>()
                .HasOne(rt => rt.Genero)
                .WithMany(g => g.ReferenciasTelas)
                .HasForeignKey(rt => rt.ID_Genero);

            modelBuilder.Entity<ReferenciaTela>()
                .HasOne(rt => rt.Tela)
                .WithMany(t => t.ReferenciasTelas)
                .HasForeignKey(rt => rt.ID_Telas);

            // USUARIO-ROL
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            // PRODUCTO RELACIONES
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Talla)
                .WithMany()
                .HasForeignKey(p => p.ID_Tallas)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Referencia)
                .WithMany()
                .HasForeignKey(p => p.ID_Referencias)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Tela)
                .WithMany()
                .HasForeignKey(p => p.ID_Telas)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Color)
                .WithMany()
                .HasForeignKey(p => p.ID_Color)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
