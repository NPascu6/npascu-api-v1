using System.ComponentModel.DataAnnotations.Schema;

namespace npascu_api_v1.Models.Entities
{
    public class Order : ModelBase
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
