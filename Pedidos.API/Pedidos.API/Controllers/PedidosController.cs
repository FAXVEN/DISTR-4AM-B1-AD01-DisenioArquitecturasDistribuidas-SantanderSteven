using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedidos.API.Data;
using Pedidos.API.Models;

namespace Pedidos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private const decimal MaximoImporte = 9999999999999999.99m;
    private readonly PedidosDbContext _context;

    public PedidosController(PedidosDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Pedido>>> Listar()
    {
        var pedidos = await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Detalles)
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pedido>> Consultar(int id)
    {
        var pedido = await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido is null)
            return NotFound();

        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> Crear(EntradaPedido entrada)
    {
        if (!await _context.Clientes.AnyAsync(c => c.IdCliente == entrada.IdCliente))
            return NotFound("El cliente indicado no existe.");

        var pedido = new Pedido
        {
            IdCliente = entrada.IdCliente,
            Fecha = DateTime.UtcNow,
            Estado = "Registrado"
        };

        foreach (var linea in entrada.Detalles)
        {
            if (linea is null)
                return BadRequest("Cada detalle debe contener los datos del producto.");

            if (linea.PrecioUnitario <= 0 ||
                linea.PrecioUnitario > MaximoImporte ||
                decimal.Round(linea.PrecioUnitario, 2) != linea.PrecioUnitario)
            {
                return BadRequest("El precio unitario debe ser positivo y admitir hasta dos decimales.");
            }

            pedido.Total += linea.Cantidad * linea.PrecioUnitario;

            if (pedido.Total > MaximoImporte)
                return BadRequest("El total del pedido supera el máximo permitido.");

            pedido.Detalles.Add(new DetallePedido
            {
                IdProducto = linea.IdProducto,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.PrecioUnitario
            });
        }

        _context.Pedidos.Add(pedido);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException { Number: 547 })
        {
            return Conflict("No se pudo guardar el pedido. Verifica que el cliente siga existiendo.");
        }

        return CreatedAtAction(nameof(Consultar), new { id = pedido.IdPedido }, pedido);
    }
}