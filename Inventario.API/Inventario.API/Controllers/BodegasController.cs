using Inventario.API.Data;
using Inventario.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventario.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BodegasController : ControllerBase
{
    private readonly InventarioDbContext _context;

    public BodegasController(InventarioDbContext context)
    {
        _context = context;
    }

    // GET /api/Bodegas
    [HttpGet]
    public async Task<ActionResult<List<Bodega>>> Listar()
    {
        var bodegas = await _context.Bodegas
            .AsNoTracking()
            .ToListAsync();

        return Ok(bodegas);
    }

    // GET /api/Bodegas/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Bodega>> Consultar(int id)
    {
        var bodega = await _context.Bodegas
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.IdBodega == id);

        if (bodega is null)
            return NotFound();

        return Ok(bodega);
    }

    // POST /api/Bodegas
    [HttpPost]
    public async Task<ActionResult<Bodega>> Crear(Bodega bodega)
    {
        bodega.IdBodega = 0;

        _context.Bodegas.Add(bodega);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Consultar),
            new { id = bodega.IdBodega },
            bodega);
    }

    // PUT /api/Bodegas/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, Bodega bodega)
    {
        if (id != bodega.IdBodega)
            return BadRequest("El ID de la ruta debe coincidir con el ID del cuerpo.");

        var existente = await _context.Bodegas.FindAsync(id);

        if (existente is null)
            return NotFound();

        existente.Nombre = bodega.Nombre;
        existente.Ciudad = bodega.Ciudad;
        existente.Direccion = bodega.Direccion;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/Bodegas/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var bodega = await _context.Bodegas.FindAsync(id);

        if (bodega is null)
            return NotFound();

        _context.Bodegas.Remove(bodega);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("La bodega tiene existencias, movimientos o reservas asociados.");
        }

        return NoContent();
    }
}