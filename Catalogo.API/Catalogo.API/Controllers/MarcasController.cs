using Catalogo.Api.Controllers.Models;
using Catalogo.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.API.Controllers;

    [ApiController]
[Route("api/[controller]")]
public class MarcasController : ControllerBase
{
    private readonly CatalogoDbContext _context;

    public MarcasController(CatalogoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Marca>>> Listar()
    {
        var marcas = await _context.Marcas
            .AsNoTracking()
            .ToListAsync();

        return Ok(marcas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Marca>> Consultar(int id)
    {
        var marca = await _context.Marcas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdMarca == id);
        if (marca == null)
        {
            return NotFound();
        }
        return Ok(marca);
    }

    [HttpPost]
    public async Task<ActionResult<Marca>> Crear(Marca marca)
    {
        marca.IdMarca = 0;
        _context.Marcas.Add(marca);
        await _context.SaveChangesAsync();
        return CreatedAtAction(
            nameof(Consultar),
            new { id = marca.IdMarca },
            marca);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Marca>> Actualizar(int id, Marca marca)
    {
        if (id != marca.IdMarca)
            return BadRequest("El ID de la ruta debe coincidir con el ID del cuerpo.");

        var existente = await _context.Marcas.FindAsync(id);

        if (existente is null)
            return NotFound();

        existente.Nombre = marca.Nombre;
        existente.PaisOrigen = marca.PaisOrigen;

        await _context.SaveChangesAsync();

        return NoContent();

    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var marca = await _context.Marcas.FindAsync(id);

        if (marca is null)
            return NotFound();

        _context.Marcas.Remove(marca);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("La marca está asociada a un producto.");
        }

        return NoContent();
    }
} 
