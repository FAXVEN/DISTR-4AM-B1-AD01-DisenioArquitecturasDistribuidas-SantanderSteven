using Inventario.API.Controllers;
using Inventario.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.API.Data;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bodega> Bodegas => Set<Bodega>();

    public DbSet<Existencia> Existencias => Set<Existencia>();
    public DbSet<MovimientoInventario> Movimientos => Set<MovimientoInventario>();
    public DbSet<ReservaStock> Reservas => Set<ReservaStock>();
}