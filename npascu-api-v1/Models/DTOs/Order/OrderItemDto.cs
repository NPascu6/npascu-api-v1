using npascu_api_v1.Models.DTOs.Item;

namespace npascu_api_v1.Models.DTOs.Order
{
    public class OrderItemDto : ModelBaseDto
    {
        public int OrderId { get; set; }
        public OrderDto? Order { get; set; }
        public int ItemId { get; set; }
        public ItemDto? Item { get; set; }
        public int Quantity { get; set; }
    }
}
