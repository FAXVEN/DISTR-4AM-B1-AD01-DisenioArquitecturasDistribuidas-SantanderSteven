using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.API.Models;

[Table("Cliente", Schema = "dbo")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Required, StringLength(120)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Cedula { get; set; } = string.Empty;

    [Required, StringLength(254), EmailAddress]
    public string Correo { get; set; } = string.Empty;
}