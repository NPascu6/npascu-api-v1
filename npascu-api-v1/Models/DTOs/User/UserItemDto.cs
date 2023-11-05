using npascu_api_v1.Models.DTOs.Item;

namespace npascu_api_v1.Models.DTOs.User
{
    public class UserItemDto : ModelBaseDto
    {
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int ItemId { get; set; }
        public ItemDto? Item { get; set; }
    }
}
