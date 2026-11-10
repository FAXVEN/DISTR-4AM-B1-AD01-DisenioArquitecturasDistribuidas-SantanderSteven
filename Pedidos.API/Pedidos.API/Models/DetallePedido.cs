using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pedidos.API.Models
{
    [Table("DetallePedido", Schema = "dbo")]
    public class DetallePedido
    {
        [Key]
        public int IdDetalle { get; set; }

        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

    }
}
