using npascu_api_v1.Models.DTOs.Order;

namespace npascu_api_v1.Models.DTOs.User
{
    public class UserDto : ModelBaseDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
