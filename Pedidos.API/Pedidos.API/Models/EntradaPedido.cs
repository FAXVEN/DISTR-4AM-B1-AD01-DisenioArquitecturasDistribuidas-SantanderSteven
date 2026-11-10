using System.ComponentModel.DataAnnotations;

namespace Pedidos.API.Models;

public record EntradaPedido(
    [Range(1, int.MaxValue)] int IdCliente,
    [Required, MinLength(1), MaxLength(50)] List<EntradaDetalle?> Detalles);

public record EntradaDetalle(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, int.MaxValue)] int Cantidad,
    decimal PrecioUnitario);