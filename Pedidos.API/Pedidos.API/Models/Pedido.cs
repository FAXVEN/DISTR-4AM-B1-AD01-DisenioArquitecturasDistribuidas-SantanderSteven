using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pedidos.API.Models
{
    [Table("Pedido", Schema = "dbo")]
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }
        public DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required,StringLength(120)]
        public string Estado { get; set; } = string.Empty;

        public List<DetallePedido> Detalles { get; set; } = new();

    }
}
