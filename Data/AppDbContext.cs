using Microsoft.EntityFrameworkCore;
using MuebleriaProfe.Models;



namespace MuebleriaProfe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tus tablas principales
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<PlanPago> PlanesPago { get; set; }
        public DbSet<Abono> Abonos { get; set; }
        public DbSet<Gasto> Gastos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🛡️ REGLAS DE INTEGRIDAD PARA POSTGRESQL
            modelBuilder.Entity<Usuario>(entity =>
            {
                // El UUID nunca se debe repetir
               

                // Dos empleados no pueden tener el mismo login
                entity.HasIndex(e => e.NombreUsuario).IsUnique();

                // El correo tampoco debe repetirse (para evitar problemas de recuperación)
                entity.HasIndex(e => e.CorreoElectronico).IsUnique();
            });
        }
    }
}