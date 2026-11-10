using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.API.Controllers
{
    [Table("MovimientoInventario", Schema = "dbo")]
    public class MovimientoInventario
    {
        [Key]
        public int IdMovimiento { get; set; }

        public int IdProducto { get; set; }
        public int IdBodega { get; set; }

        [StringLength(120)]
        public string Tipo { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
    }
}
