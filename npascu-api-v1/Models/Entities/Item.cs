namespace npascu_api_v1.Models.Entities
{
    public class Item: ModelBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ICollection<UserItem>? Users { get; set; }
        public ICollection<OrderItem>? Orders { get; set; }
    }
}
