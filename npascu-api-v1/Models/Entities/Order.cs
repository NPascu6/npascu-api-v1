namespace npascu_api_v1.Models.Entities
{
    public class Order: ModelBase
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalPrice { get; set; }
    }
}
