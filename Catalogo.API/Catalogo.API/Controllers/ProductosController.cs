using Catalogo.Api.Controllers.Models;
using Catalogo.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly CatalogoDbContext _context;

    public ProductosController(CatalogoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Producto>>> Listar()
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .ToListAsync();

        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> Consultar(int id)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto is null)
            return NotFound();

        return Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> Crear(Producto producto)
    {
        var error = await ValidarProducto(producto);

        if (error is not null)
            return BadRequest(error);

        producto.IdProducto = 0;
        _context.Productos.Add(producto);

        var errorAlGuardar = await GuardarProducto();

        if (errorAlGuardar is not null)
            return errorAlGuardar;

        return CreatedAtAction(
            nameof(Consultar),
            new { id = producto.IdProducto },
            producto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, Producto producto)
    {
        if (id != producto.IdProducto)
            return BadRequest("El ID de la ruta debe coincidir con el ID del cuerpo.");

        var existente = await _context.Productos.FindAsync(id);

        if (existente is null)
            return NotFound();

        var error = await ValidarProducto(producto);

        if (error is not null)
            return BadRequest(error);

        existente.IdCategoria = producto.IdCategoria;
        existente.IdMarca = producto.IdMarca;
        existente.Nombre = producto.Nombre;
        existente.SKU = producto.SKU;
        existente.Precio = producto.Precio;
        existente.Estado = producto.Estado;

        var errorAlGuardar = await GuardarProducto();

        if (errorAlGuardar is not null)
            return errorAlGuardar;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto is null)
            return NotFound();

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> ValidarProducto(Producto producto)
    {
        if (producto.Precio <= 0 ||
            producto.Precio > 9999999999999999.99m ||
            decimal.Round(producto.Precio, 2) != producto.Precio)
        {
            return "El precio debe ser positivo, caber en decimal(18,2) y admitir hasta dos decimales.";
        }

        if (!await _context.Categorias.AnyAsync(c => c.IdCategoria == producto.IdCategoria))
            return "La categoría indicada no existe.";

        if (!await _context.Marcas.AnyAsync(m => m.IdMarca == producto.IdMarca))
            return "La marca indicada no existe.";

        return null;
    }

    private async Task<ActionResult?> GuardarProducto()
    {
        try
        {
            await _context.SaveChangesAsync();
            return null;
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Ya existe un producto con ese SKU.");
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 547 })
        {
            return BadRequest("La categoría o la marca indicada ya no está disponible.");
        }
    }
}