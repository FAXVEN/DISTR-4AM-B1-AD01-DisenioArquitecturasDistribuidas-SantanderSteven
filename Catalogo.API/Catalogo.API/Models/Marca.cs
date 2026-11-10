using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogo.Api.Controllers.Models;

[Table("Marca", Schema = "dbo")]
public class Marca
{
    [Key]
    public int IdMarca { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string PaisOrigen { get; set; } = string.Empty;
}