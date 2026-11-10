using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.API.Models;

[Table("Bodega", Schema = "dbo")]
public class Bodega
{
    [Key]
    public int IdBodega { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Ciudad { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Direccion { get; set; } = string.Empty;
}