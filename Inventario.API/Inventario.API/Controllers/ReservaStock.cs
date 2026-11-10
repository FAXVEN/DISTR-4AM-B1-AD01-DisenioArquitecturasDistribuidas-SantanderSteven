using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.API.Controllers
{
    [Table("ReservaStock", Schema = "dbo")]
    public class ReservaStock
    {
        [Key]
        public int IdReserva { get; set; }

        public int IdProducto { get; set; }
        public int IdBodega { get; set; }
        public int Cantidad { get; set; }

        [StringLength(120)]
        public string Estado { get; set; } = string.Empty;
    }
}
