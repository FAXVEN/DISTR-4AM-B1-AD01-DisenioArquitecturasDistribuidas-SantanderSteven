using Catalogo.Api.Controllers.Models;
using Catalogo.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly CatalogoDbContext _context;

    public CategoriasController(CatalogoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Categoria>>> Listar()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> Consultar(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCategoria == id);
        if (categoria == null)
        {
            return NotFound();
        }
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> Crear(Categoria categoria)
    {
        categoria.IdCategoria = 0;
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(
            nameof(Consultar),
            new { id = categoria.IdCategoria },
            categoria);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Categoria>> Actualizar(int id, Categoria categoria)
    {
        if (id != categoria.IdCategoria)
            return BadRequest("El ID de la ruta debe coincidir con el ID del cuerpo.");

        var existente = await _context.Categorias.FindAsync(id);

        if (existente is null)
            return NotFound();

        existente.Nombre = categoria.Nombre;
        existente.Descripcion = categoria.Descripcion;

        await _context.SaveChangesAsync();

        return NoContent();

    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria is null)
            return NotFound();

        _context.Categorias.Remove(categoria);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("La categoría está asociada a un producto.");
        }

        return NoContent();
    }
}