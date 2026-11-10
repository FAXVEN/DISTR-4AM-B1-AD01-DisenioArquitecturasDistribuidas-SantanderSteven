using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedidos.API.Data;
using Pedidos.API.Models;

namespace Pedidos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly PedidosDbContext _context;

    public ClientesController(PedidosDbContext context)
    {
        _context = context;
    }

    // GET /api/Clientes
    [HttpGet]
    public async Task<ActionResult<List<Cliente>>> Listar()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .ToListAsync();

        return Ok(clientes);
    }

    // GET /api/Clientes/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> Consultar(int id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCliente == id);

        if (cliente is null)
            return NotFound();

        return Ok(cliente);
    }

    // POST /api/Clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> Crear(Cliente cliente)
    {
        cliente.IdCliente = 0;
        _context.Clientes.Add(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Ya existe un cliente con esa cédula.");
        }

        return CreatedAtAction(
            nameof(Consultar),
            new { id = cliente.IdCliente },
            cliente);
    }

    // PUT /api/Clientes/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, Cliente cliente)
    {
        if (id != cliente.IdCliente)
            return BadRequest("El ID de la ruta debe coincidir con el ID del cuerpo.");

        var existente = await _context.Clientes.FindAsync(id);

        if (existente is null)
            return NotFound();

        existente.Nombres = cliente.Nombres;
        existente.Cedula = cliente.Cedula;
        existente.Correo = cliente.Correo;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Ya existe un cliente con esa cédula.");
        }

        return NoContent();
    }

    // DELETE /api/Clientes/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente is null)
            return NotFound();

        _context.Clientes.Remove(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("El cliente tiene pedidos asociados.");
        }

        return NoContent();
    }
}