using Catalogo.Api.Controllers.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.API.Data
{
    public class CatalogoDbContext : DbContext
    {
        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias => Set<Categoria>();

        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Producto> Productos => Set<Producto>();
    }
}
