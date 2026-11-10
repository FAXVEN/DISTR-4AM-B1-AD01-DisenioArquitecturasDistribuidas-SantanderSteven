using System.ComponentModel.DataAnnotations;

namespace Inventario.API.Models;

public record EntradaMovimiento(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, int.MaxValue)] int IdBodega,
    [Required, StringLength(10)] string Tipo,
    [Range(1, int.MaxValue)] int Cantidad);

public record EntradaReserva(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, int.MaxValue)] int IdBodega,
    [Range(1, int.MaxValue)] int Cantidad);