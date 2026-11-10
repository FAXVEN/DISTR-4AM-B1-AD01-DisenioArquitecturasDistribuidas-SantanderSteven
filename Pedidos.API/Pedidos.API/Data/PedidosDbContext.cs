using Microsoft.EntityFrameworkCore;
using Pedidos.API.Models;

namespace Pedidos.API.Data;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallePedidos => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Pedido>()
            .HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(p => p.IdCliente)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Detalles)
            .WithOne()
            .HasForeignKey(d => d.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);


    }
}