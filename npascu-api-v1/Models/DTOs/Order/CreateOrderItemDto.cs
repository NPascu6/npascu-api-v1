namespace npascu_api_v1.Models.DTOs.Order
{
    public class CreateOrderItemDto
    {
        public int ItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
