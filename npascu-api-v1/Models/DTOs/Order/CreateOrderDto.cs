namespace npascu_api_v1.Models.DTOs.Order
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
        public decimal TotalPrice { get; set; }

        public CreateOrderDto() {

            TotalPrice = GetTotalPrice(OrderItems);
        }

        private decimal GetTotalPrice (ICollection<CreateOrderItemDto> OrderItems)
        {
            decimal total = 0;  
            foreach (var item in OrderItems)
            {
                total = (item.Price * item.Quantity);
            }

            return total;
        } 
    }
}
