using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogo.Api.Controllers.Models;

[Table("Producto", Schema = "dbo")]
public class Producto
{
    [Key]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdCategoria { get; set; }

    [Range(1, int.MaxValue)]
    public int IdMarca { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    public bool Estado { get; set; }
}