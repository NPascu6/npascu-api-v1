namespace npascu_api_v1.Models.Entities
{
    public class OrderItem : ModelBase
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public int Quantity { get; set; }
    }
}
