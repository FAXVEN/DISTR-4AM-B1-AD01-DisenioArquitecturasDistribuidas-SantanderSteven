using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogo.Api.Controllers.Models;

[Table("Categoria", Schema = "dbo")]
public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;
}