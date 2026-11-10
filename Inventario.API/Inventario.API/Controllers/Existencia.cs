using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Inventario.API.Controllers
{
    [Table("Existencia", Schema = "dbo")]
    public class Existencia
    {
        [Key]
        public int IdExistencia { get; set; }

        public int IdProducto { get; set; }
        public int IdBodega { get; set; }
        public int Stock { get; set; }

        [ConcurrencyCheck]
        public int Revision { get; set; }
    }
}
