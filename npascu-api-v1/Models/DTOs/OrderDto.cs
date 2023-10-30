namespace npascu_api_v1.Models.DTOs
{
    public class OrderDto: ModelBaseDto
    {
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal TotalPrice { get; set; }
    }
}
