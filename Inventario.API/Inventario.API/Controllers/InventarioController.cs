using Inventario.API.Data;
using Inventario.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventario.API.Controllers;

[ApiController]
[Route("api")]
public class InventarioController : ControllerBase
{
    private readonly InventarioDbContext _context;

    public InventarioController(InventarioDbContext context)
    {
        _context = context;
    }

    [HttpGet("Existencias")]
    public async Task<IActionResult> ListarExistencias()
    {
        var existencias = await _context.Existencias
            .AsNoTracking()
            .Select(e => new
            {
                e.IdExistencia,
                e.IdProducto,
                e.IdBodega,
                e.Stock,
                Disponible = e.Stock - (_context.Reservas
                    .Where(r => r.IdProducto == e.IdProducto &&
                                r.IdBodega == e.IdBodega &&
                                r.Estado == "Activa")
                    .Sum(r => (long?)r.Cantidad) ?? 0)
            })
            .ToListAsync();

        return Ok(existencias);
    }

    [HttpGet("Movimientos")]
    public async Task<IActionResult> ListarMovimientos()
    {
        return Ok(await _context.Movimientos.AsNoTracking().ToListAsync());
    }

    [HttpGet("Movimientos/{id:int}")]
    public async Task<ActionResult<MovimientoInventario>> ConsultarMovimiento(int id)
    {
        var movimiento = await _context.Movimientos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdMovimiento == id);

        if (movimiento is null)
            return NotFound();

        return Ok(movimiento);
    }

    [HttpPost("Movimientos")]
    public async Task<ActionResult<MovimientoInventario>> CrearMovimiento(
        EntradaMovimiento entrada)
    {
        var tipo = entrada.Tipo.Trim().ToUpperInvariant();

        if (tipo is not ("ENTRADA" or "SALIDA"))
            return BadRequest("El tipo debe ser Entrada o Salida.");

        if (!await _context.Bodegas.AnyAsync(b => b.IdBodega == entrada.IdBodega))
            return NotFound("La bodega indicada no existe.");

        var existencia = await BuscarExistencia(entrada.IdProducto, entrada.IdBodega);

        if (tipo == "SALIDA" &&
            (existencia is null || await CalcularDisponible(existencia) < entrada.Cantidad))
        {
            return Conflict("No hay suficiente stock disponible.");
        }

        if (existencia is null)
        {
            existencia = new Existencia
            {
                IdProducto = entrada.IdProducto,
                IdBodega = entrada.IdBodega
            };

            _context.Existencias.Add(existencia);
        }

        var nuevoStock = tipo == "ENTRADA"
            ? (long)existencia.Stock + entrada.Cantidad
            : (long)existencia.Stock - entrada.Cantidad;

        if (nuevoStock > int.MaxValue)
            return BadRequest("La entrada supera el máximo de stock.");

        existencia.Stock = (int)nuevoStock;
        existencia.Revision++;

        var movimiento = new MovimientoInventario
        {
            IdProducto = entrada.IdProducto,
            IdBodega = entrada.IdBodega,
            Tipo = tipo,
            Cantidad = entrada.Cantidad,
            Fecha = DateTime.UtcNow
        };

        _context.Movimientos.Add(movimiento);

        var error = await GuardarCambios();
        if (error is not null)
            return error;

        return CreatedAtAction(
            nameof(ConsultarMovimiento),
            new { id = movimiento.IdMovimiento },
            movimiento);
    }

    [HttpGet("Reservas")]
    public async Task<IActionResult> ListarReservas()
    {
        return Ok(await _context.Reservas.AsNoTracking().ToListAsync());
    }

    [HttpGet("Reservas/{id:int}")]
    public async Task<ActionResult<ReservaStock>> ConsultarReserva(int id)
    {
        var reserva = await _context.Reservas.AsNoTracking()
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva is null)
            return NotFound();

        return Ok(reserva);
    }

    [HttpPost("Reservas")]
    public async Task<ActionResult<ReservaStock>> CrearReserva(EntradaReserva entrada)
    {
        if (!await _context.Bodegas.AnyAsync(b => b.IdBodega == entrada.IdBodega))
            return NotFound("La bodega indicada no existe.");

        var existencia = await BuscarExistencia(entrada.IdProducto, entrada.IdBodega);

        if (existencia is null || await CalcularDisponible(existencia) < entrada.Cantidad)
            return Conflict("No hay suficiente stock disponible para la reserva.");

        var reserva = new ReservaStock
        {
            IdProducto = entrada.IdProducto,
            IdBodega = entrada.IdBodega,
            Cantidad = entrada.Cantidad,
            Estado = "Activa"
        };

        existencia.Revision++;
        _context.Reservas.Add(reserva);

        var error = await GuardarCambios();
        if (error is not null)
            return error;

        return CreatedAtAction(
            nameof(ConsultarReserva),
            new { id = reserva.IdReserva },
            reserva);
    }

    [HttpPut("Reservas/{id:int}/cancelacion")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);

        if (reserva is null)
            return NotFound();

        if (reserva.Estado == "Cancelada")
            return NoContent();

        var existencia = await BuscarExistencia(reserva.IdProducto, reserva.IdBodega);

        if (existencia is null)
            return Conflict("La reserva no tiene una existencia asociada.");

        reserva.Estado = "Cancelada";
        existencia.Revision++;

        var error = await GuardarCambios();
        if (error is not null)
            return error;

        return NoContent();
    }

    private Task<Existencia?> BuscarExistencia(int idProducto, int idBodega)
    {
        return _context.Existencias.SingleOrDefaultAsync(e =>
            e.IdProducto == idProducto && e.IdBodega == idBodega);
    }

    private async Task<long> CalcularDisponible(Existencia existencia)
    {
        var reservado = await _context.Reservas
            .Where(r => r.IdProducto == existencia.IdProducto &&
                        r.IdBodega == existencia.IdBodega &&
                        r.Estado == "Activa")
            .SumAsync(r => (long)r.Cantidad);

        return existencia.Stock - reservado;
    }

    private async Task<ActionResult?> GuardarCambios()
    {
        try
        {
            await _context.SaveChangesAsync();
            return null;
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("El inventario cambió. Consulta el stock e intenta de nuevo.");
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Ya se creó una existencia para ese producto y bodega.");
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("No se pudo guardar el cambio. Verifica la bodega y el stock.");
        }
    }
}